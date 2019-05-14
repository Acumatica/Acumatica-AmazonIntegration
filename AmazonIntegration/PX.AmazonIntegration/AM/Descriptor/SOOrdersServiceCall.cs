using System;
using PX.Data;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceOrders.Model;
using System.Collections.Generic;
using System.Threading;

namespace AmazonIntegration
{
    # region Amazon response variables

    public class InvokeServicesCallResponse
    {
        public ListOrdersResponse objListOrderResponse;
        public GetOrderResponse objGetOrderResponse;
        public ListOrderItemsResponse objListOrderItemsResponse;
        public ListOrdersByNextTokenResponse objListOrdersByNextToken;
        public List<ListOrderItems> liListOrderCollections = new List<ListOrderItems>();
    }

    public class ListOrderItems
    {
        public string amazonOrderID { get; set; }
        public string buyerEmailID { get; set; }
        public DateTime? amazonOrderDate { get; set; }
        public DateTime? amazonOrderLastUpdated { get; set; }
        public decimal? orderAmount { get; set; }
        public string amazonOrderStatus { get; set; }
        public string amazonTracking { get; set; }
        public string amazonOrderSchema { get; set; }
    }

    public class ServiceCallParameters
    {
        public SOAmazonSetup objSOAmazonSetup { get; set; }
        public string amwOrderID { get; set; }
        public string methodCall { get; set; }
        public Order curretOrder { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public string nextToken { get; set; }
        public List<ListOrderItems> liListOrderResponse { get; set; }
    }
    #endregion

    #region Amazon Order Service Call Methods
    public class SOOrdersServiceCall
    {
        #region Variables
        private readonly MarketplaceWebServiceOrders.MarketplaceWebServiceOrders clientOrder;
        DateTime? businessDateTime = null;
        #endregion

        #region Constructors

        public SOOrdersServiceCall(MarketplaceWebServiceOrders.MarketplaceWebServiceOrders order)
        {
            this.clientOrder = order;
        }
        #endregion

        #region Amazon Service Call

        private MarketplaceWebServiceOrders.MarketplaceWebServiceOrders Configurations(string intergrationType, string accessKey, string secretKey)
        {
            MarketplaceWebServiceOrdersConfig config = new MarketplaceWebServiceOrdersConfig();
            config.ServiceURL = SOHelper.GetIntegrationType(intergrationType.Trim(), SOConstants.serviceUrl);
            MarketplaceWebServiceOrders.MarketplaceWebServiceOrders serviceOrder = new MarketplaceWebServiceOrdersClient(accessKey, secretKey, SOConstants.appName, SOConstants.version, config);
            return serviceOrder;
        }

        public virtual InvokeServicesCallResponse InvokeServicesCalls(PXGraph graph, ServiceCallParameters objParams)
        {
            InvokeServicesCallResponse objSyncOrderResponse = new InvokeServicesCallResponse();
            try
            {
                if (objParams.objSOAmazonSetup != null)
                {
                    MarketplaceWebServiceOrders.MarketplaceWebServiceOrders serviceConfig = this.Configurations(objParams.objSOAmazonSetup.IntegrationType.Trim(), objParams.objSOAmazonSetup.AccessKey, objParams.objSOAmazonSetup.SecretKey);
                    switch (objParams.methodCall)
                    {
                        case SOConstants.invokeListOrders:
                            objSyncOrderResponse.objListOrderResponse = new SOOrdersServiceCall(serviceConfig).InvokeListOrders(graph, objParams);
                            break;
                        case SOConstants.invokeGetOrder:
                            objSyncOrderResponse.objGetOrderResponse = new SOOrdersServiceCall(serviceConfig).InvokeGetOrder(graph, objParams);
                            break;
                        case SOConstants.invokeListOrderItems:
                            objSyncOrderResponse.objListOrderItemsResponse = new SOOrdersServiceCall(serviceConfig).GetSalesOrderLineItems(graph, objParams);
                            break;
                        case SOConstants.invokeListOrdersByNextToken:
                            objSyncOrderResponse.objListOrdersByNextToken = new SOOrdersServiceCall(serviceConfig).InvokeListOrdersByNextToken(graph, objParams);
                            break;
                    }
                }
                return objSyncOrderResponse;
            }
            catch (Exception ex)
            {
                throw new PXException(ex.Message);
            }
        }

        private ListOrdersResponse InvokeListOrders(PXGraph graph, ServiceCallParameters objParams)
        {
            ListOrdersResponse response = new ListOrdersResponse();
            try
            {
                ListOrdersRequest request = new ListOrdersRequest();
                request.SellerId = objParams.objSOAmazonSetup.SellerId;
                request.MWSAuthToken = objParams.objSOAmazonSetup.AuthToken;
                List<string> marketplaceId = new List<string>();
                marketplaceId.Add(objParams.objSOAmazonSetup.MarketplaceId);
                request.MarketplaceId = marketplaceId;
                request.CreatedAfter = Convert.ToDateTime(objParams.fromDate);
                if (objParams.toDate.Value.Date < DateTime.Now.Date)
                    request.CreatedBefore = objParams.toDate.Value.AddDays(1).AddTicks(-1);
                PXTrace.WriteInformation("objParams.fromDate.Value.Date:" + objParams.fromDate.Value.Date.ToString());
                PXTrace.WriteInformation("objParams.toDate.Value.Date:" + objParams.toDate.Value.Date.ToString());
                PXTrace.WriteInformation("request.CreatedAfter:" + request.CreatedAfter.ToString());
                PXTrace.WriteInformation("request.CreatedBefore:" + request.CreatedBefore.ToString());
                List<string> liFulfillmentChannel = null;
                List<string> liOrderStatus = null;
                SOHelper.GetFilterValues(objParams.objSOAmazonSetup.IntegrationType.Trim(), out liFulfillmentChannel, out liOrderStatus);
                request.FulfillmentChannel = liFulfillmentChannel;
                request.OrderStatus = liOrderStatus;
                response = this.clientOrder.ListOrders(request);
            }
            catch (Exception ex)
            {
                if (ex is MarketplaceWebServiceOrdersException)
                {
                    MarketplaceWebServiceOrdersException exception = ex as MarketplaceWebServiceOrdersException;
                    if (!string.IsNullOrEmpty(exception.ErrorCode) && exception.ErrorCode.ToLower().Trim() == SOMessages.requestThrottled)
                    {
                        Thread.Sleep(SOHelper.DelayProcess(graph, SOConstants.apiListOrders));
                        response = InvokeListOrders(graph, objParams);
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
            return response;
        }

        public ListOrdersByNextTokenResponse InvokeListOrdersByNextToken(PXGraph graph, ServiceCallParameters objParams)
        {
            ListOrdersByNextTokenRequest request = new ListOrdersByNextTokenRequest();
            ListOrdersByNextTokenResponse responseOrderByNextToken = new ListOrdersByNextTokenResponse();
            try
            {
                request.SellerId = objParams.objSOAmazonSetup.SellerId;
                request.MWSAuthToken = objParams.objSOAmazonSetup.AuthToken;
                request.NextToken = objParams.nextToken;
                responseOrderByNextToken = this.clientOrder.ListOrdersByNextToken(request);
            }
            catch (Exception ex)
            {
                if (ex is MarketplaceWebServiceOrdersException)
                {
                    MarketplaceWebServiceOrdersException exception = ex as MarketplaceWebServiceOrdersException;
                    if (!string.IsNullOrEmpty(exception.ErrorCode) && exception.ErrorCode.ToLower().Trim() == SOMessages.requestThrottled)
                    {
                        Thread.Sleep(SOHelper.DelayProcess(graph, SOConstants.apiListOrdersByNextToken));
                        responseOrderByNextToken = InvokeListOrdersByNextToken(graph, objParams);
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
            return responseOrderByNextToken;
        }

        private GetOrderResponse InvokeGetOrder(PXGraph graph, ServiceCallParameters objParams)
        {
            GetOrderResponse getOrderRes = new GetOrderResponse();
            try
            {
                GetOrderRequest request = new GetOrderRequest();
                request.SellerId = objParams.objSOAmazonSetup.SellerId;
                request.MWSAuthToken = objParams.objSOAmazonSetup.AuthToken;
                List<string> liAmazonOrderId = new List<string>();
                if (!string.IsNullOrEmpty(objParams.amwOrderID))
                    liAmazonOrderId.Add(objParams.amwOrderID);
                request.AmazonOrderId = liAmazonOrderId;
                getOrderRes = this.clientOrder.GetOrder(request);
            }
            catch (Exception ex)
            {
                if (ex is MarketplaceWebServiceOrdersException)
                {
                    MarketplaceWebServiceOrdersException exception = ex as MarketplaceWebServiceOrdersException;
                    if (!string.IsNullOrEmpty(exception.ErrorCode) && exception.ErrorCode.ToLower().Trim() == SOMessages.requestThrottled)
                    {
                        Thread.Sleep(SOHelper.DelayProcess(graph, SOConstants.apiGetOrder));
                        getOrderRes = InvokeGetOrder(graph, objParams);
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
            return getOrderRes;
        }

        private ListOrderItemsResponse GetSalesOrderLineItems(PXGraph graph, ServiceCallParameters objParams)
        {
            ListOrderItemsResponse lineItemsRes = new ListOrderItemsResponse();
            try
            {
                ListOrderItemsRequest request = new ListOrderItemsRequest();
                request.SellerId = objParams.objSOAmazonSetup.SellerId;
                request.MWSAuthToken = objParams.objSOAmazonSetup.AuthToken;
                request.AmazonOrderId = objParams.amwOrderID;
                lineItemsRes = this.clientOrder.ListOrderItems(request);
            }
            catch (Exception ex)
            {
                if (ex is MarketplaceWebServiceOrdersException)
                {
                    MarketplaceWebServiceOrdersException exception = ex as MarketplaceWebServiceOrdersException;
                    if (!string.IsNullOrEmpty(exception.ErrorCode) && exception.ErrorCode.ToLower().Trim() == SOMessages.requestThrottled)
                    {
                        Thread.Sleep(SOHelper.DelayProcess(graph, SOConstants.apiGetListOrderItems));
                        lineItemsRes = GetSalesOrderLineItems(graph, objParams);
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
            return lineItemsRes;
            #endregion
        }
        #endregion
    }
}