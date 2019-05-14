using PX.Data;
using PX.Objects.SO;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AmazonIntegration
{
    public class SOGetFBATrackingNumberProcess : PXGraph<SOGetFBATrackingNumberProcess>
    {
        #region FilterClass
        [Serializable()]
        public partial class SOTrackingReportFilter : IBqlTable
        {
            #region IntegrationID

            [PXString(30, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Integration ID")]
            [PXSelector(typeof(Search<SOAmazonSetup.integrationID, Where<SOAmazonSetup.status, Equal<True>, And<SOAmazonSetup.integrationType, NotEqual<SOConstants.FBMIntegrationType>>>>), typeof(SOAmazonSetup.integrationID), typeof(SOAmazonSetup.description), typeof(SOAmazonSetup.integrationType))]
            public virtual string IntegrationID { get; set; }
            public abstract class integrationID : IBqlField { }

            #endregion

            #region Process All Types

            [PXBool()]
            [PXDefault(false)]
            [PXUIField(DisplayName = "Process All Integrations")]
            public virtual bool? ProcessAllTypes { get; set; }
            public abstract class processAllTypes : IBqlField { }

            #endregion     

            #region FromDate

            [PXDate()]
            [PXDefault()]
            [PXUIField(DisplayName = "From Date", Required = true)]
            public virtual DateTime? FromDate { get; set; }
            public abstract class fromDate : IBqlField { }

            #endregion

            #region TODate

            [PXDate()]           
            [PXUIField(DisplayName = "To Date", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
            public virtual DateTime? TODate { get; set; }
            public abstract class tODate : IBqlField { }

            #endregion 

            #region CurrentDay

            [PXBool()]
            [PXDefault(true)]
            [PXUIField(DisplayName = "Current Day")]
            public virtual bool? CurrentDay { get; set; }
            public abstract class currentDay : IBqlField { }

            #endregion     
        }
        #endregion

        #region Views
        public PXCancel<SOTrackingReportFilter> Cancel;
        public PXFilter<SOTrackingReportFilter> Filter;
        [PXFilterable]
        public PXFilteredProcessing<SOProcessOrder, SOTrackingReportFilter,
                                                   Where<SOProcessOrder.prepareStatus, Equal<True>, And<SOProcessOrder.importStatus, Equal<True>,
                                                   And<SOProcessOrder.integrationID, Equal<Current<SOTrackingReportFilter.integrationID>>,
                                                   And<SOProcessOrder.amazonTrackingNumber, IsNull,
                                                   And<Where2<Where<SOProcessOrder.amazonOrderDate, GreaterEqual<Current<SOTrackingReportFilter.fromDate>>,
                                                   And<SOProcessOrder.amazonOrderDate, Less<Current<SOTrackingReportFilter.tODate>>>>,
                                                   Or<Where<SOProcessOrder.amazonOrderLastUpdated, GreaterEqual<Current<SOTrackingReportFilter.fromDate>>,
                                                   And<SOProcessOrder.amazonOrderLastUpdated, Less<Current<SOTrackingReportFilter.tODate>>>>>>>
                                                   >>>>> TrackingNumberOrdersList;
        #endregion

        #region Variables        
        private static readonly MarketplaceWebService.MarketplaceWebService clientReports = null;
        #endregion
        protected virtual IEnumerable trackingNumberOrdersList()
        {
            SOTrackingReportFilter filter = Filter.Current;
            if (filter != null)
            {
                PXSelectBase<SOProcessOrder> preparedorder = new PXSelect<SOProcessOrder,
                                                                          Where<SOProcessOrder.prepareStatus, Equal<True>,
                                                                          And<SOProcessOrder.importStatus, Equal<True>,
                                                                          And<SOProcessOrder.amazonTrackingNumber, IsNull
                                                                          >>>>(this);
                if (filter.FromDate != null && filter.TODate != null &&
                    filter.CurrentDay == false && !string.IsNullOrEmpty(filter.IntegrationID) && filter.ProcessAllTypes == false)
                {
                    preparedorder.WhereAnd<Where<SOProcessOrder.integrationID, Equal<Current<SOTrackingReportFilter.integrationID>>,
                                            And<Where2<Where<SOProcessOrder.amazonOrderDate, GreaterEqual<Current<SOTrackingReportFilter.fromDate>>,
                                            And<SOProcessOrder.amazonOrderDate, Less<Required<SOTrackingReportFilter.tODate>>>>,
                                            Or<Where<SOProcessOrder.amazonOrderLastUpdated, GreaterEqual<Current<SOTrackingReportFilter.fromDate>>,
                                            And<SOProcessOrder.amazonOrderLastUpdated, Less<Required<SOTrackingReportFilter.tODate>>>>>>>>>();
                    return preparedorder.Select(filter.TODate.Value.AddDays(1), filter.TODate.Value.AddDays(1));
                }
                if (filter.FromDate == null && filter.TODate == null &&
                    filter.CurrentDay == true && !string.IsNullOrEmpty(filter.IntegrationID) && filter.ProcessAllTypes == false)
                {
                    preparedorder.WhereAnd<Where<SOProcessOrder.integrationID, Equal<Current<SOTrackingReportFilter.integrationID>>,
                                                                And<Where2<Where<SOProcessOrder.amazonOrderDate, GreaterEqual<Required<SOTrackingReportFilter.fromDate>>,
                                                                And<SOProcessOrder.amazonOrderDate, Less<Required<SOTrackingReportFilter.tODate>>>>,
                                                                Or<Where<SOProcessOrder.amazonOrderLastUpdated, GreaterEqual<Required<SOTrackingReportFilter.fromDate>>,
                                                                And<SOProcessOrder.amazonOrderLastUpdated, Less<Required<SOTrackingReportFilter.tODate>>>>>>>>>();
                    return preparedorder.Select(DateTime.Today, DateTime.Today.AddDays(1).AddTicks(-1), DateTime.Today, DateTime.Today.AddDays(1).AddTicks(-1));
                }
            }
            return null;
        }

        #region Constructor
        public SOGetFBATrackingNumberProcess()
        {
            SOTrackingReportFilter currentFilter = this.Filter.Current;
            TrackingNumberOrdersList.SetProcessDelegate(
                delegate (List<SOProcessOrder> list)
                {
                    GetTrackingNumber(list, currentFilter);
                });
        }
        #endregion

        #region Update Report Details

        public static void GetTrackingNumber(List<SOProcessOrder> list, SOTrackingReportFilter currentFilter)
        {
            SOGetFBATrackingNumberProcess graph = PXGraph.CreateInstance<SOGetFBATrackingNumberProcess>();
            graph.UpdateReportDetails(graph, list, currentFilter);
        }
        #endregion

        public virtual void UpdateReportDetails(SOGetFBATrackingNumberProcess graph, List<SOProcessOrder> list, SOTrackingReportFilter currentFilter)
        {
            if (list.Count < 0) return;
            if (currentFilter.CurrentDay == false && Convert.ToInt32((Convert.ToDateTime(currentFilter.TODate) - Convert.ToDateTime(currentFilter.FromDate)).TotalDays) > SOConstants.limitForReports)
            {
                throw new PXException(SOMessages.ReportDateValidation);
            }
            SOAmazonSetup objSOAmazonSetupDetails = null;
            SOOrderEntry orderEntry = PXGraph.CreateInstance<SOOrderEntry>();
            SOPartialMaint objPartialMaint = PXGraph.CreateInstance<SOPartialMaint>();
            orderEntry.Clear();
            InvokeReporstServicesCallResponse liShipmentResponse = null;
            bool isValidConfiguration = SOHelper.MarketplaceConfigurations(graph, currentFilter.IntegrationID, out objSOAmazonSetupDetails);
            if (currentFilter.IntegrationID != null && currentFilter.ProcessAllTypes == false)
            {
                if (isValidConfiguration)
                    liShipmentResponse = GetShipmentInformation(graph, currentFilter, objSOAmazonSetupDetails);
                UpdateTrackingNumberDetails(graph, list, objSOAmazonSetupDetails, orderEntry, objPartialMaint, liShipmentResponse);
            }
            else if (string.IsNullOrEmpty(currentFilter.IntegrationID) && currentFilter.ProcessAllTypes == true)
            {
                TrackingNumberOrdersList.Cache.Clear();
                foreach (SOAmazonSetup objSOAmazonSetup in PXSelectReadonly<SOAmazonSetup, Where<SOAmazonSetup.status, Equal<True>, And<SOAmazonSetup.integrationType, NotEqual<SOConstants.FBMIntegrationType>>>>.Select(graph))
                {
                    try
                    {
                        liShipmentResponse = GetShipmentInformation(graph, currentFilter, objSOAmazonSetup);
                        UpdateTrackingNumberDetails(graph, list, objSOAmazonSetupDetails, orderEntry, objPartialMaint, liShipmentResponse);
                    }
                    catch (Exception ex)
                    {
                        throw new PXException(ex.Message);
                    }
                }
            }
            graph.Actions.PressSave();
        }
        private static void UpdateTrackingNumberDetails(SOGetFBATrackingNumberProcess graph, List<SOProcessOrder> list, SOAmazonSetup objSOAmazonSetupDetails, SOOrderEntry orderEntry, SOPartialMaint objPartialMaint, InvokeReporstServicesCallResponse liShipmentResponse)
        {
            string trackingNumber = string.Empty;
            string carrier = string.Empty;
            foreach (SOProcessOrder currentRecord in list)
            {
                try
                {
                    if (liShipmentResponse != null && liShipmentResponse.objShipmentResponse != null && liShipmentResponse.objShipmentResponse.Count > 0)
                    {
                        trackingNumber = SOHelper.GetTrackingForFBAOrders(currentRecord.AmazonOrderID, liShipmentResponse.objShipmentResponse, out carrier);
                        currentRecord.AmazonTrackingNumber = !string.IsNullOrEmpty(trackingNumber) ? trackingNumber : null;
                        currentRecord.Carrier = !string.IsNullOrEmpty(carrier) ? carrier : null;
                        graph.TrackingNumberOrdersList.Cache.Update(currentRecord);
                        SOOrder objSOOrder = objPartialMaint.SOOrderView.Select(objSOAmazonSetupDetails.OrderType, currentRecord.AmazonOrderID);
                        if (objSOOrder != null)
                        {
                            PXNoteAttribute.SetNote(orderEntry.Document.Cache, objSOOrder, trackingNumber);
                            orderEntry.Document.Cache.Update(objSOOrder);
                            orderEntry.Actions.PressSave();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PXException(ex.Message);
                }
            }

        }
        private static InvokeReporstServicesCallResponse GetShipmentInformation(PXGraph graph, SOTrackingReportFilter currentFilter, SOAmazonSetup objSOAmazonSetupDetails)
        {
            InvokeReporstServicesCallResponse liShipmentResponse = new InvokeReporstServicesCallResponse();
            liShipmentResponse = new AMShipmentTrackingNumberServiceCall(clientReports).InvokeReportsServiceCalls(graph, objSOAmazonSetupDetails, currentFilter.FromDate, currentFilter.TODate);
            return liShipmentResponse;
        }
        protected virtual void SOTrackingReportFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            SOTrackingReportFilter row = e.Row as SOTrackingReportFilter;
            if (row != null)
            {
                PXUIFieldAttribute.SetEnabled<SOTrackingReportFilter.processAllTypes>(Filter.Cache, null, string.IsNullOrEmpty(row.IntegrationID));
                PXUIFieldAttribute.SetEnabled<SOTrackingReportFilter.integrationID>(Filter.Cache, null, (row.ProcessAllTypes != null && row.ProcessAllTypes != true));
                if (row.CurrentDay == true)
                {
                    row.FromDate = null;
                    row.TODate = null;
                    PXUIFieldAttribute.SetEnabled<SOTrackingReportFilter.fromDate>(Filter.Cache, null, false);
                    PXUIFieldAttribute.SetEnabled<SOTrackingReportFilter.tODate>(Filter.Cache, null, false);
                }
                else
                {
                    PXUIFieldAttribute.SetEnabled<SOTrackingReportFilter.fromDate>(Filter.Cache, null, true);
                    PXUIFieldAttribute.SetEnabled<SOTrackingReportFilter.tODate>(Filter.Cache, null, true);
                }
            }
        }
    }
}