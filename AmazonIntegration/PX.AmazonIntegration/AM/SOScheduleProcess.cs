using System;
using System.Collections.Generic;
using System.Linq;
using MarketplaceWebServiceOrders.Model;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.SO;
using PX.Objects.CS;

namespace AmazonIntegration
{
    public class SOScheduleProcess : PXGraph<SOScheduleProcess>
    {
        #region Select
        public PXCancel<SOImportProcess.SOImportFilter> Cancel;
        public PXFilter<SOImportProcess.SOImportFilter> Filter;
        [PXFilterable]
        public PXFilteredProcessing<SOPrepareAndImport, SOImportProcess.SOImportFilter> prepareAndImport;
        #endregion      

        #region Variables
        //public static readonly MarketplaceWebService.MarketplaceWebService clientReports;
        public static readonly MarketplaceWebServiceOrders.MarketplaceWebServiceOrders clientOrder;
        PrepareAndImportOrdersParams objScheduleParams = null;
        public static InvokeServicesCallResponse objSyncOrderResponse = null;
        public static List<OrderItem> amwLineItems = null;
        public static List<Order> amwOrders = null;
        ServiceCallParameters objServiceCallParams = null;
        #endregion

        public SOScheduleProcess()
        {
            prepareAndImport.SetProcessVisible(false);
            prepareAndImport.SetProcessAllCaption(SOConstants.btnPrepareAndImport);
            prepareAndImport.SetProcessDelegate(
                delegate (List<SOPrepareAndImport> list)
                {
                    PrepareAndImportRecords(list);
                });
        }
        public static void PrepareAndImportRecords(List<SOPrepareAndImport> listOfPrepareAndImport)
        {
            SOScheduleProcess graph = PXGraph.CreateInstance<SOScheduleProcess>();
            graph.PrepareAndImportAmazonRecords(graph, listOfPrepareAndImport);
        }
        public void PrepareAndImportAmazonRecords(SOScheduleProcess graph, List<SOPrepareAndImport> listOfPrepareAndImport)
        {
            objSyncOrderResponse = new InvokeServicesCallResponse();
            amwOrders = new List<Order>();
            amwLineItems = new List<OrderItem>();
            SOAmazonSetup objSOAmazonSetup = null;
            SOPartialMaint logGraph = PXGraph.CreateInstance<SOPartialMaint>();
            SOOrderEntry orderEntry = PXGraph.CreateInstance<SOOrderEntry>();
            ARPaymentEntry paymentGraph = PXGraph.CreateInstance<ARPaymentEntry>();
            bool isErrorOccured = false;           
            DateTime? businessDateTime = PX.Common.PXTimeZoneInfo.Now;
            List<string> liCarriers = PXSelect<Carrier>.Select(graph).RowCast<Carrier>().Select(c => c.CarrierID).ToList();
            try
            {
                SOSetupAmazonExt objSOOSetupext = orderEntry.sosetup.Current.GetExtension<SOSetupAmazonExt>();
                if (SOHelper.IsSOPreferencesDetailsExist(orderEntry, objSOOSetupext))
                {
                    foreach (SOPrepareAndImport currentRecord in listOfPrepareAndImport)
                    {
                        objSOAmazonSetup = null;
                        if (SOHelper.MarketplaceConfigurations(graph, currentRecord.IntegrationID, out objSOAmazonSetup))
                        {
                            objSyncOrderResponse = objSyncOrderResponse != null ? null : objSyncOrderResponse;
                            if (amwOrders != null && amwOrders.Count > 0)
                                amwOrders.Clear();
                            if (amwLineItems != null && amwLineItems.Count > 0)
                                amwLineItems.Clear();
                            orderEntry.Clear();
                            logGraph.Clear();
                            paymentGraph.Clear();
                            objServiceCallParams = new ServiceCallParameters();
                            objServiceCallParams.objSOAmazonSetup = objSOAmazonSetup;
                            objServiceCallParams.amwOrderID = string.Empty;
                            objServiceCallParams.methodCall = SOConstants.invokeListOrders;
                            objServiceCallParams.fromDate = currentRecord.LastSyncDate;
                            objServiceCallParams.toDate = currentRecord.ToDate;
                            // Prepare action is invoked
                            objSyncOrderResponse = new SOOrdersServiceCall(clientOrder).InvokeServicesCalls(graph, objServiceCallParams);
                            if (amwLineItems != null && amwLineItems.Count > 0)
                                amwLineItems.Clear();
                            if (objSyncOrderResponse != null && objSyncOrderResponse.objListOrderResponse != null
                                        && objSyncOrderResponse.objListOrderResponse.ListOrdersResult != null &&
                                        objSyncOrderResponse.objListOrderResponse.ListOrdersResult.Orders != null && objSyncOrderResponse.objListOrderResponse.ListOrdersResult.Orders.Count > 0)
                            {
                                amwOrders = objSyncOrderResponse.objListOrderResponse.ListOrdersResult.Orders.ToList();
                                // Saving the prepare action response
                                objScheduleParams = new PrepareAndImportOrdersParams();
                                objScheduleParams.objSOPrepareAndImport = currentRecord;
                                objScheduleParams.objSOAmazonSetup = objSOAmazonSetup;
                                objScheduleParams.objSOOrderEntry = orderEntry;
                                objScheduleParams.objSOPartialMaint = logGraph;
                                objScheduleParams.businessDateTime = businessDateTime;
                                objScheduleParams.paymentGraph = paymentGraph;
                                objScheduleParams.listOfCarriers = liCarriers;
                                objScheduleParams.amwOrders = amwOrders;
                                SOHelper.PrepareRecord(graph, objScheduleParams, ref isErrorOccured);
                                if (objSyncOrderResponse.objListOrderResponse.ListOrdersResult.NextToken != null)
                                {
                                    objScheduleParams.objNextToken = objSyncOrderResponse.objListOrderResponse.ListOrdersResult.NextToken;
                                    objScheduleParams.paymentGraph = paymentGraph;
                                    GetAmazonOrdersbyNextToken(graph, objScheduleParams, ref isErrorOccured);
                                }
                            }
                            else
                                SOLogService.LogImportCount(null, currentRecord.IntegrationID, logGraph, currentRecord.ProcessID,
                                                      SOConstants.scheduleimportorders, true);
                        }
                        else
                            throw new PXException(SOMessages.apidetailsMissing);
                    }
                }
                else
                    throw new PXException(SOMessages.configMissing);
            }
            catch (Exception ex)
            {
                SOLogService.LogImportStatus(objScheduleParams, false, ex.Message);
                throw new PXException(ex.Message);
            }
        }
        public virtual void GetAmazonOrdersbyNextToken(SOScheduleProcess graph, PrepareAndImportOrdersParams objScheduleParams, ref bool isErrorOccured)
        {
            objSyncOrderResponse = new InvokeServicesCallResponse();
            amwOrders = new List<Order>();
            amwLineItems = new List<OrderItem>();
            objServiceCallParams = new ServiceCallParameters();
            objServiceCallParams.objSOAmazonSetup = objScheduleParams.objSOAmazonSetup;
            objServiceCallParams.amwOrderID = string.Empty;
            objServiceCallParams.methodCall = SOConstants.invokeListOrdersByNextToken;
            objServiceCallParams.fromDate = objScheduleParams.objSOPrepareAndImport.LastSyncDate;
            objServiceCallParams.toDate = objScheduleParams.objSOPrepareAndImport.ToDate;
            objServiceCallParams.nextToken = objScheduleParams.objNextToken;
            objSyncOrderResponse = new SOOrdersServiceCall(clientOrder).InvokeServicesCalls(graph, objServiceCallParams);
            if (objSyncOrderResponse != null && objSyncOrderResponse.objListOrdersByNextToken != null &&
                objSyncOrderResponse.objListOrdersByNextToken.ListOrdersByNextTokenResult != null &&
                objSyncOrderResponse.objListOrdersByNextToken.ListOrdersByNextTokenResult.Orders.Count > 0)
            {
                amwOrders = objSyncOrderResponse.objListOrdersByNextToken.ListOrdersByNextTokenResult.Orders.ToList();
                objScheduleParams.amwOrders = amwOrders;
                SOHelper.PrepareRecord(graph, objScheduleParams, ref isErrorOccured);
                if (objSyncOrderResponse.objListOrdersByNextToken.ListOrdersByNextTokenResult.NextToken != null)
                {
                    objScheduleParams.objNextToken = objSyncOrderResponse.objListOrdersByNextToken.ListOrdersByNextTokenResult.NextToken;
                    GetAmazonOrdersbyNextToken(graph, objScheduleParams, ref isErrorOccured);
                }
            }
        }

        #region Events

        protected virtual void SOImportFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            SOImportProcess.SOImportFilter row = e.Row as SOImportProcess.SOImportFilter;
            if (row != null)
            {
                PXUIFieldAttribute.SetEnabled<SOImportProcess.SOImportFilter.lastSyncDate>(sender, row, false);
                PXUIFieldAttribute.SetEnabled<SOImportProcess.SOImportFilter.processAllTypes>(Filter.Cache, null, string.IsNullOrEmpty(row.IntegrationID));
                PXUIFieldAttribute.SetEnabled<SOImportProcess.SOImportFilter.integrationID>(Filter.Cache, null, (row.ProcessAllTypes != null && row.ProcessAllTypes != true));
            }
        }
        protected virtual void SOImportFilter_ToDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            LoadIntegrationsToPrepareAndImport(Filter.Current);
        }
        protected virtual void SOImportFilter_IntegrationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            LoadIntegrationsToPrepareAndImport(Filter.Current);
        }
        protected virtual void SOImportFilter_ProcessAllTypes_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            LoadIntegrationsToPrepareAndImport(Filter.Current);
        }
        private void LoadIntegrationsToPrepareAndImport(SOImportProcess.SOImportFilter row)
        {
            if (row != null)
            {
                SOOrderProcessLog getProcessId = PXSelectGroupBy<SOOrderProcessLog, Aggregate<Max<SOOrderProcessLog.processID>>>.Select(this);
                PXSelectBase<SOProcessOrder> objMaxOrder = new PXSelectGroupBy<SOProcessOrder, Where<SOProcessOrder.integrationID, Equal<Required<SOProcessOrder.integrationID>>>, Aggregate<Max<SOProcessOrder.synDatetime>>>(this);

                if (row.IntegrationID != null && row.ProcessAllTypes == true && row.LastSyncDate != null)
                {
                    throw new PXException(SOMessages.msgProcessError);
                }
                else
                {
                    SOPrepareAndImport processRecord = new SOPrepareAndImport();
                    prepareAndImport.Cache.Clear();
                    SOOrderEntry orderEntry = PXGraph.CreateInstance<SOOrderEntry>();
                    if (row.IntegrationID != null && row.ProcessAllTypes == false)
                    {
                        foreach (SOProcessOrder objLastOrder in objMaxOrder.Select(row.IntegrationID))
                        {
                            Filter.Cache.SetValue<SOImportProcess.SOImportFilter.lastSyncDate>(row, objLastOrder.SynDatetime ?? GetMaxOrderDate(orderEntry, row.IntegrationID));
                            processRecord.LastSyncDate = GetMaxOrderDate(orderEntry, row.IntegrationID);
                            processRecord.ToDate = Filter.Current.TODate;
                            processRecord.IntegrationID = row.IntegrationID;
                            processRecord.ProcessID = (getProcessId != null && getProcessId.ProcessID != null) ? getProcessId.ProcessID + 1 : 1;
                            prepareAndImport.Cache.Insert(processRecord);
                        }
                    }
                    else if (string.IsNullOrEmpty(row.IntegrationID) && row.ProcessAllTypes == true)
                    {
                        int? processId = (getProcessId != null && getProcessId.ProcessID != null) ? getProcessId.ProcessID + 1 : 1;
                        foreach (SOAmazonSetup integrationrecord in PXSelectReadonly<SOAmazonSetup, Where<SOAmazonSetup.status, Equal<True>>>.Select(this))
                        {
                            processRecord.LastSyncDate = GetMaxOrderDate(orderEntry, integrationrecord.IntegrationID);
                            processRecord.ToDate = row.TODate;
                            processRecord.IntegrationID = integrationrecord.IntegrationID;
                            processRecord.ProcessID = processId++;
                            prepareAndImport.Cache.Insert(processRecord);
                        }
                    }
                }
            }
        }
        private DateTime? GetMaxOrderDate(SOOrderEntry orderEntry, string intetgrationId)
        {
            DateTime? maxDate = null;
            SOProcessOrder objProcessOrder = new PXSelect<SOProcessOrder, Where<SOProcessOrder.integrationID, Equal<Required<SOProcessOrder.integrationID>>>,
                                OrderBy<Desc<SOProcessOrder.amazonOrderDate>>>(this).SelectSingle(intetgrationId);
            if (objProcessOrder == null || (objProcessOrder != null && objProcessOrder.AmazonOrderDate == null))
            {
                orderEntry.Clear();
                SOSetupAmazonExt objSOOSetupext = orderEntry.sosetup.Current.GetExtension<SOSetupAmazonExt>();
                if (objSOOSetupext != null && objSOOSetupext.UsrAmazonInitialFromDate != null)
                    maxDate = objSOOSetupext.UsrAmazonInitialFromDate;
            }
            else if (objProcessOrder != null && objProcessOrder.AmazonOrderDate.HasValue)
                maxDate = objProcessOrder.AmazonOrderDate;
            return maxDate;
        }
        #endregion
    }
    public class PrepareAndImportOrdersParams
    {
        //PrepareAndImportOrdersParams
        public SOPrepareAndImport objSOPrepareAndImport { get; set; }
        public SOAmazonSetup objSOAmazonSetup { get; set; }
        public string objNextToken { get; set; }
        public SOOrderEntry objSOOrderEntry { get; set; }
        public SOPartialMaint objSOPartialMaint { get; set; }
        public ARPaymentEntry paymentGraph { get; set; }
        public List<Order> amwOrders { get; set; }
        public DateTime? businessDateTime { get; set; }
        public SOProcessOrder objSOProcessOrderRecord { get; set; }
        public string amazonOrderID { get; set; }
        public Order ObjCurrentOrder { get; set; }
        public List<OrderItem> objamwLineItems { get; set; }
        public List<SOFieldMapping> objliUsrMapping { get; set; }
        public int CurrentOrderIndex { get; set; }
        public string orderSchema { get; set; }
        public List<string> listOfCarriers { get; set; }
    }
}