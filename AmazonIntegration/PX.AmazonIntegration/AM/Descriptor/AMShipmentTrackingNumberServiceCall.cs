using System;
using MarketplaceWebService;
using MarketplaceWebService.Model;
using PX.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace AmazonIntegration
{
    public class AMShipmentTrackingNumberServiceCall
    {
        public readonly MarketplaceWebService.MarketplaceWebService clientReports;
        private List<ReportRequestInfo> liReportRequestInfo;
        private ReportParameters reportParams;

        public AMShipmentTrackingNumberServiceCall(MarketplaceWebService.MarketplaceWebService reports)
        {
            this.clientReports = reports;
        }
 
        public virtual InvokeReporstServicesCallResponse InvokeReportsServiceCalls(PXGraph graph, SOAmazonSetup objSOAmazonSetup, DateTime? fromDate, DateTime? toDate)
        {
            InvokeReporstServicesCallResponse objShipInfo = new InvokeReporstServicesCallResponse();
            MarketplaceWebService.MarketplaceWebService serviceConfig = this.Configurations(objSOAmazonSetup.IntegrationType.Trim(), objSOAmazonSetup.AccessKey, objSOAmazonSetup.SecretKey);
            if (serviceConfig == null) return objShipInfo;
            ReportRequestInfo reportInfo = new ReportRequestInfo();
            List<ShipmentInformarion> liShipmentInfo = new List<ShipmentInformarion>();
            reportParams = new ReportParameters();
            reportParams.objSOAmazonSetup = objSOAmazonSetup;
            reportParams.reportRequestId = string.Empty;
            reportParams.reportInfo = reportInfo;
            reportParams.liShipmentInfo = liShipmentInfo;
            reportParams.serviceConfig = serviceConfig;
            reportInfo = new AMShipmentTrackingNumberServiceCall(reportParams.serviceConfig).InvokeGetReportRequestList(graph, reportParams);
            reportParams.fromDate = fromDate;
            reportParams.toDate = toDate;

            try
            {
                if (reportInfo != null && reportInfo.GeneratedReportId != null)
                {
                    if (Convert.ToInt32((TimeZoneInfo.ConvertTimeToUtc(DateTime.Now) - TimeZoneInfo.ConvertTimeToUtc(reportInfo.CompletedDate)).TotalMinutes) <= SOConstants.limitForReports)
                        throw new PXException(SOMessages.errorReportInfo);
                    else
                        liShipmentInfo = RequestAndGetReportData(graph, reportParams);
                    objShipInfo.objShipmentResponse = liShipmentInfo;
                }
                else                
                    liShipmentInfo = RequestAndGetReportData(graph, reportParams);
            }
            catch (Exception ex)
            {
                throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                    ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : SOConstants.exceptionIsEmpty);
            }
            return objShipInfo;
        }

        private static List<ShipmentInformarion> RequestAndGetReportData(PXGraph graph, ReportParameters reportParams)
        {
            RequestReportResponse requestReportResponse = new AMShipmentTrackingNumberServiceCall(reportParams.serviceConfig).InvokeRequestReport(graph, reportParams);
            if (requestReportResponse != null && requestReportResponse.RequestReportResult != null && requestReportResponse.RequestReportResult.ReportRequestInfo != null && requestReportResponse.RequestReportResult.ReportRequestInfo.ReportRequestId != null)
            {
                reportParams.reportRequestId = requestReportResponse.RequestReportResult.ReportRequestInfo.ReportRequestId;
                reportParams.reportInfo = new AMShipmentTrackingNumberServiceCall(reportParams.serviceConfig).InvokeGetReportRequestList(graph, reportParams);
                if (reportParams.reportInfo != null && reportParams.reportInfo.GeneratedReportId != null)
                {
                    reportParams.generatedReportId = reportParams.reportInfo.GeneratedReportId;
                    reportParams.liShipmentInfo = GetShipmentReportData(graph, reportParams);
                }
            }
            return reportParams.liShipmentInfo;
        }

        private static List<ShipmentInformarion> GetShipmentReportData(PXGraph graph, ReportParameters reportParams)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new AMShipmentTrackingNumberServiceCall(reportParams.serviceConfig).InvokeGetReport(graph, reportParams, ms);
                using (StreamReader sr = new StreamReader(ms))
                {
                    int orderIndex = 0;
                    int trackingNbrIndex = 0;
                    int carrierIndex = 0;
                    int checkCount = 1;
                    List<string> oStreamDataValues = null;
                    while (!sr.EndOfStream)
                    {
                        string oStreamRowData = sr.ReadLine().Trim();
                        if (oStreamRowData.Length > 0)
                        {
                            oStreamDataValues = Regex.Split(oStreamRowData, "\t").ToList();
                            if (checkCount == 1)
                            {
                                orderIndex = oStreamDataValues.FindIndex(x => x == SOConstants.amazonOrderidColumn);
                                trackingNbrIndex = oStreamDataValues.FindIndex(x => x == SOConstants.trackingNumberColumn);
                                carrierIndex = oStreamDataValues.FindIndex(x => x == SOConstants.carrierColumn);
                            }
                        }
                        if (checkCount != 1)
                        {
                            ShipmentInformarion objNew = new ShipmentInformarion();
                            objNew.amazonOrderID = Convert.ToString(oStreamDataValues[orderIndex]);
                            objNew.shipmentTrackingNbr = Convert.ToString(oStreamDataValues[trackingNbrIndex]);
                            objNew.shipmentCarrier = Convert.ToString(oStreamDataValues[carrierIndex]);
                            reportParams.liShipmentInfo.Add(objNew);
                        }
                        checkCount++;
                    }
                }
            }
            return reportParams.liShipmentInfo;
        }

        private MarketplaceWebService.MarketplaceWebService Configurations(string intergrationType, string accessKey, string secretKey)
        {
            MarketplaceWebService.MarketplaceWebServiceConfig config = new MarketplaceWebService.MarketplaceWebServiceConfig();
            config.ServiceURL = SOHelper.GetIntegrationType(intergrationType.Trim(), SOConstants.serviceUrl);
            MarketplaceWebService.MarketplaceWebService serviceConfig = new MarketplaceWebService.MarketplaceWebServiceClient(accessKey, secretKey, SOConstants.appName, SOConstants.version, config);
            return serviceConfig;
        }

        private RequestReportResponse InvokeRequestReport(PXGraph graph, ReportParameters reportParams)
        {
            RequestReportResponse reqReportResponse = new RequestReportResponse();
            try
            {               
                RequestReportRequest request = new RequestReportRequest();              
                DateTime? businessDateTime = PX.Common.PXTimeZoneInfo.Now;
                request.Merchant = reportParams.objSOAmazonSetup.SellerId;
                request.MWSAuthToken = reportParams.objSOAmazonSetup.AuthToken;
                request.ReportType = SOConstants.amazonFulfilledShipmentData;
                request.StartDate = Convert.ToDateTime(reportParams.fromDate);
                request.EndDate = Convert.ToDateTime(reportParams.toDate).Date >= businessDateTime.Value.Date ? businessDateTime.Value.AddMinutes(-2) : Convert.ToDateTime(reportParams.toDate).AddDays(1);
                reqReportResponse = clientReports.RequestReport(request);
            }
            catch (Exception ex)
            {
                if (ex is MarketplaceWebServiceException)
                {
                    MarketplaceWebServiceException exception = ex as MarketplaceWebServiceException;
                    if (!string.IsNullOrEmpty(exception.ErrorCode) && exception.ErrorCode.ToLower().Trim() == SOMessages.requestThrottled)
                    {
                        Thread.Sleep(SOHelper.DelayProcess(graph, SOConstants.apiRequestReport));
                        reqReportResponse = InvokeRequestReport(graph, reportParams);
                    }
                    else
                    {
                        throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                             ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                             : SOConstants.exceptionIsEmpty);
                    }
                }
                else
                {
                    throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                             ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                             : SOConstants.exceptionIsEmpty);
                }
            }
            return reqReportResponse;
        }

        private ReportRequestInfo InvokeGetReportRequestList(PXGraph graph, ReportParameters reportParams)
        {
            GetReportRequestListResponse reqReportListResponse = new GetReportRequestListResponse();
            reportParams.reportInfo = new ReportRequestInfo();
            try
            {
                GetReportRequestListRequest request = new GetReportRequestListRequest();
                request.Merchant = reportParams.objSOAmazonSetup.SellerId;
                request.MWSAuthToken = reportParams.objSOAmazonSetup.AuthToken;
                request.MaxCount = 1;
                if (!string.IsNullOrEmpty(reportParams.reportRequestId))
                {
                    IdList liReportIds = new IdList();
                    liReportIds.Id.Add(reportParams.reportRequestId);
                    request.ReportRequestIdList = liReportIds;
                }
                TypeList liReportTypes = new TypeList();
                liReportTypes.Type.Add(SOConstants.amazonFulfilledShipmentData); 
                request.ReportTypeList = liReportTypes;
                StatusList liReportStatus = new StatusList();
                liReportStatus.Status.Add(SOConstants.statusDone);
                request.ReportProcessingStatusList = liReportStatus;
                reqReportListResponse = clientReports.GetReportRequestList(request);

                if (reqReportListResponse != null && reqReportListResponse.GetReportRequestListResult != null && reqReportListResponse.GetReportRequestListResult.ReportRequestInfo != null && reqReportListResponse.GetReportRequestListResult.ReportRequestInfo.Count > 0)
                {
                    liReportRequestInfo = reqReportListResponse.GetReportRequestListResult.ReportRequestInfo;
                    foreach (ReportRequestInfo reportRequestInfoItem in liReportRequestInfo)
                    {
                        if (reportRequestInfoItem.GeneratedReportId == null && string.IsNullOrEmpty(reportRequestInfoItem.GeneratedReportId) && reportRequestInfoItem.ReportProcessingStatus == SOConstants.statusInProgress || reportRequestInfoItem.ReportProcessingStatus == SOConstants.statusSubmitted)
                        {
                            Thread.Sleep(SOHelper.DelayProcess(graph, SOConstants.getGeneratedReportID));
                            reportParams.reportInfo = InvokeGetReportRequestList(graph, reportParams);
                        }
                        else if (reportRequestInfoItem.GeneratedReportId != null && !string.IsNullOrEmpty(reportRequestInfoItem.GeneratedReportId) && reportRequestInfoItem.ReportProcessingStatus == SOConstants.statusDone)
                        {
                            reportParams.reportInfo = reportRequestInfoItem;
                            break;
                        }
                    }
                }
                else
                {
                    RequestAndGetReportData(graph, reportParams);
                }
            }
            catch (Exception ex)
            {
                if (ex is MarketplaceWebServiceException)
                {
                    MarketplaceWebServiceException exception = ex as MarketplaceWebServiceException;
                    if (!string.IsNullOrEmpty(exception.ErrorCode) && exception.ErrorCode.ToLower().Trim() == SOMessages.requestThrottled)
                    {
                        Thread.Sleep(SOHelper.DelayProcess(graph, SOConstants.apiGetRequestReportList));
                        reportParams.reportInfo = InvokeGetReportRequestList(graph, reportParams);
                    }
                    else
                    {
                        throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                             ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                             : SOConstants.exceptionIsEmpty);
                    }
                }
                else
                {
                    throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                             ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                             : SOConstants.exceptionIsEmpty);
                }
            }
            return reportParams.reportInfo;
        }

        public void InvokeGetReport(PXGraph graph, ReportParameters reportParams, MemoryStream ms)
        {
            try
            {
                GetReportRequest request = new GetReportRequest();
                request.Merchant = reportParams.objSOAmazonSetup.SellerId;
                request.MWSAuthToken = reportParams.objSOAmazonSetup.AuthToken;
                request.ReportId = reportParams.generatedReportId;
                request.Report = ms;
                clientReports.GetReport(request);
            }
            catch (Exception ex)
            {
                if (ex is MarketplaceWebServiceException)
                {
                    MarketplaceWebServiceException exception = ex as MarketplaceWebServiceException;
                    if (!string.IsNullOrEmpty(exception.ErrorCode) && exception.ErrorCode.ToLower().Trim() == SOMessages.requestThrottled)
                    {
                        Thread.Sleep(SOHelper.DelayProcess(graph, SOConstants.apiGetReport));
                        InvokeGetReport(graph, reportParams, ms);
                    }
                    else
                    {
                        throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                             ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                             : SOConstants.exceptionIsEmpty);
                    }
                }
                else
                {
                    throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                             ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                             : SOConstants.exceptionIsEmpty);
                }
            }
        }
    }
    public class ShipmentInformarion
    {
        public string amazonOrderID { get; set; }
        public string shipmentTrackingNbr { get; set; }
        public string shipmentCarrier { get; set; }
    }
    public class InvokeReporstServicesCallResponse
    {
        public List<ShipmentInformarion> objShipmentResponse;
    }
    public class ReportParameters
    {
        public SOAmazonSetup objSOAmazonSetup { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public MarketplaceWebService.MarketplaceWebService serviceConfig { get; set; }
        public string reportRequestId { get; set; }
        public ReportRequestInfo reportInfo { get; set; }
        public List<ShipmentInformarion> liShipmentInfo { get; set; }
        public string generatedReportId { get; set; }
    }
}