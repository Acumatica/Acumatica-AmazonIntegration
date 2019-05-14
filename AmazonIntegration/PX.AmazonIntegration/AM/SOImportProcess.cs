using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MarketplaceWebServiceOrders.Model;
using PX.Common;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.SO;
using PX.Objects.CS;

namespace AmazonIntegration
{
    public class SOImportProcess : PXGraph<SOImportProcess>
    {
        #region FilterClass
        [Serializable()]
        public partial class SOImportFilter : PX.Data.IBqlTable
        {
            #region IntegrationID

            [PXString(30, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Integration ID")]
            [PXSelector(typeof(Search<SOAmazonSetup.integrationID, Where<SOAmazonSetup.status, Equal<True>>>), typeof(SOAmazonSetup.integrationID), typeof(SOAmazonSetup.description), typeof(SOAmazonSetup.integrationType))]
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

            #region LastSyncDate

            [PXDate()]
            [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
            [PXUIField(DisplayName = "From Date", Required = true)]
            public virtual DateTime? LastSyncDate { get; set; }
            public abstract class lastSyncDate : IBqlField { }

            #endregion

            #region TODate

            [PXDate()]
            [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
            [PXUIField(DisplayName = "To Date", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
            public virtual DateTime? TODate { get; set; }
            public abstract class tODate : IBqlField { }

            #endregion

            #region TotalRecordsToImport

            [PXInt]
            [PXUIField(DisplayName = "Total Records To Import", Enabled = false)]
            public virtual int? TotalRecordsToImport { get; set; }
            public abstract class totalRecordsToImport : IBqlField { }

            #endregion

            #region TotalImportedRecords

            [PXInt]
            [PXUIField(DisplayName = "Total Imported Records", Enabled = false)]
            public virtual int? TotalImportedRecords { get; set; }
            public abstract class totalImportedRecords : IBqlField { }

            #endregion

            #region TotalFailedRecords

            [PXInt]
            [PXUIField(DisplayName = "Total Failed Records", Enabled = false)]
            public virtual int? TotalFailedRecords { get; set; }
            public abstract class totalFailedRecords : IBqlField { }

            #endregion

            #region FromDate

            [PXDate()]
            [PXUIField(DisplayName = "From Date")]
            public virtual DateTime? FromDate { get; set; }
            public abstract class fromDate : IBqlField { }

            #endregion           
        }
        #endregion
        #region Views
        public PXCancel<SOImportFilter> Cancel;
        public PXFilter<SOImportFilter> Filter;
        public PXAction<SOImportFilter> PrepareRecords;
        [PXFilterable]
        public PXFilteredProcessing<SOProcessOrder, SOImportFilter,
                                                   Where<SOProcessOrder.prepareStatus, Equal<True>, And<SOProcessOrder.importStatus, Equal<False>,
                                                   And<SOProcessOrder.integrationID, Equal<Current<SOImportFilter.integrationID>>,
                                                   And<Where2<Where<SOProcessOrder.amazonOrderDate, GreaterEqual<Current<SOImportFilter.lastSyncDate>>,
                                                   And<SOProcessOrder.amazonOrderDate, Less<Current<SOImportFilter.tODate>>>>,
                                                   Or<Where<SOProcessOrder.amazonOrderLastUpdated, GreaterEqual<Current<SOImportFilter.lastSyncDate>>,
                                                   And<SOProcessOrder.amazonOrderLastUpdated, Less<Current<SOImportFilter.tODate>>>>>>>
                                                   >>>> ImportOrderList;
        #endregion
        #region Variables
        private static readonly MarketplaceWebServiceOrders.MarketplaceWebServiceOrders clientOrder = null;
        ServiceCallParameters objServiceCallParams = null;
        OrdersParameters objOrdersParameters = null;
        #endregion

        protected virtual IEnumerable importOrderList()
        {
            SOImportFilter filter = Filter.Current;
            if (filter != null && filter.LastSyncDate != null && filter.TODate != null && (!string.IsNullOrEmpty(filter.IntegrationID) && filter.ProcessAllTypes == false ||
                                   string.IsNullOrEmpty(filter.IntegrationID) && filter.ProcessAllTypes == true))
            {
                PXSelectBase<SOProcessOrder> preparedorder = new PXSelect<SOProcessOrder,
                                                                          Where<SOProcessOrder.prepareStatus, Equal<True>, And<SOProcessOrder.importStatus, Equal<False>,
                                                                          And<Where2<Where<SOProcessOrder.amazonOrderDate, GreaterEqual<Current<SOImportFilter.lastSyncDate>>,
                                                                          And<SOProcessOrder.amazonOrderDate, Less<Required<SOImportFilter.tODate>>>>,
                                                                          Or<Where<SOProcessOrder.amazonOrderLastUpdated, GreaterEqual<Current<SOImportFilter.lastSyncDate>>,
                                                                          And<SOProcessOrder.amazonOrderLastUpdated, Less<Required<SOImportFilter.tODate>>>>>>>>>>(this);
                if (!string.IsNullOrEmpty(filter.IntegrationID) && filter.ProcessAllTypes == false)
                {
                    preparedorder.WhereAnd<Where<SOProcessOrder.integrationID, Equal<Current<SOImportFilter.integrationID>>>>();
                }
                return preparedorder.Select(filter.TODate.Value.AddDays(1).AddTicks(-1), filter.TODate.Value.AddDays(1).AddTicks(-1));
            }
            return null;
        }

        #region Constructor
        public SOImportProcess()
        {
            ImportOrderList.SetProcessCaption(SOConstants.btnImport);
            ImportOrderList.SetProcessAllCaption(SOConstants.btnImportAll);
            SOImportFilter currentFilter = this.Filter.Current;
            ImportOrderList.SetProcessDelegate(
                delegate (List<SOProcessOrder> list)
                {
                    ImportRecords(list, currentFilter);
                });
        }
        #endregion

        #region Import Methods
        public static void ImportRecords(List<SOProcessOrder> list, SOImportFilter currentFilter)
        {
            SOImportProcess graph = PXGraph.CreateInstance<SOImportProcess>();
            graph.ImportAmazonRecords(graph, list, currentFilter);
        }
        public virtual void ImportAmazonRecords(SOImportProcess graph, List<SOProcessOrder> list, SOImportFilter currentFilter)
        {
            if (list.Count < 0) return;
            List<Order> amwOrder = null;
            List<OrderItem> amwLineItems = null;
            string amwOrderID = string.Empty;
            List<SOAmazonSetup> liSOAmazonSetup = new List<SOAmazonSetup>();
            List<SOFieldMapping> liUsrMapping = new List<SOFieldMapping>();
            SOAmazonSetup objamwSetup = null;
            SOOrderProcessLog objProcessLog = null;
            SOPartialMaint logGraph = PXGraph.CreateInstance<SOPartialMaint>();
            if (currentFilter.ProcessAllTypes == true)
            {
                foreach (SOAmazonSetup objSetup in PXSelect<SOAmazonSetup, Where<SOAmazonSetup.status, Equal<True>>>.Select(graph))
                {
                    liSOAmazonSetup.Add(objSetup);
                }
            }
            else if (SOHelper.MarketplaceConfigurations(graph, currentFilter.IntegrationID, out objamwSetup))
            {
                liSOAmazonSetup.Add(objamwSetup);
            }
            if (liSOAmazonSetup.Count > 0)
            {
                ARPaymentEntry docgraph = PXGraph.CreateInstance<ARPaymentEntry>();
                SOOrderEntry orderEntry = PXGraph.CreateInstance<SOOrderEntry>();
                orderEntry.RowUpdated.AddHandler<SOOrder>((sender, e) =>
                {
                    if (!sender.ObjectsEqual<SOOrder.orderDate>(e.Row, e.OldRow))
                    {
                        SOOrder order = (SOOrder)e.Row;
                        order.OrderDate = order.OrderDate.HasValue ? order.OrderDate.Value.Date : order.OrderDate;
                    }
                });
                InvokeServicesCallResponse objSyncOrderResponse = null;
                PrepareAndImportOrdersParams objScheduleParams = null;
                objProcessLog = new SOOrderProcessLog();               
                objProcessLog.ProcessDate = PX.Common.PXTimeZoneInfo.Now;
                objProcessLog.TotalRecordstoImport = list.Count;
                objProcessLog.Operation = SOConstants.btnImport;
                objProcessLog.ImportedRecordsCount = 0;
                objProcessLog.FailedRecordsCount = 0;

                bool isErrorOccured = false;
                List<string> liCarriers = PXSelect<Carrier>.Select(graph).RowCast<Carrier>().Select(c => c.CarrierID).ToList();
                foreach (SOProcessOrder currentRecord in list)
                {
                    try
                    {
                        orderEntry.Clear();
                        logGraph.Clear();
                        docgraph.Clear();
                        objProcessLog.IntegrationID = currentFilter.IntegrationID != null ? currentFilter.IntegrationID : currentRecord.IntegrationID;
                        logGraph.OrderProcessLog.Insert(objProcessLog);
                        logGraph.Actions.PressSave();

                        foreach (SOAmazonSetup objSOAmazonSetup in liSOAmazonSetup.Where(x => x.IntegrationID == currentRecord.IntegrationID))
                        {
                            if (orderEntry.sosetup.Current != null)
                            {
                                if (!SOHelper.CheckOrderExist(graph, currentRecord.AmazonOrderID, currentRecord.IntegrationID))
                                {
                                    amwOrderID = currentRecord.AmazonOrderID;
                                    if (amwLineItems != null && amwLineItems.Count > 0)
                                        amwLineItems.Clear();
                                    amwOrder = new List<Order>();
                                    amwOrder.Add(SOHelper.SchemaDeserialization(graph, amwOrderID));
                                    objServiceCallParams = new ServiceCallParameters();
                                    objServiceCallParams.objSOAmazonSetup = objSOAmazonSetup;
                                    objServiceCallParams.amwOrderID = currentRecord.AmazonOrderID;
                                    objServiceCallParams.methodCall = SOConstants.invokeListOrderItems;
                                    objSyncOrderResponse = new InvokeServicesCallResponse();
                                    objSyncOrderResponse = new SOOrdersServiceCall(clientOrder).InvokeServicesCalls(graph, objServiceCallParams);
                                    amwLineItems = objSyncOrderResponse != null && objSyncOrderResponse.objListOrderItemsResponse != null &&
                                                   objSyncOrderResponse.objListOrderItemsResponse.ListOrderItemsResult != null &&
                                                   objSyncOrderResponse.objListOrderItemsResponse.ListOrderItemsResult.OrderItems.Count > 0 ?
                                                   objSyncOrderResponse.objListOrderItemsResponse.ListOrderItemsResult.OrderItems : amwLineItems;
                                    objScheduleParams = new PrepareAndImportOrdersParams();
                                    objScheduleParams.objSOPartialMaint = logGraph;
                                    objScheduleParams.objSOAmazonSetup = objSOAmazonSetup;
                                    objScheduleParams.objSOOrderEntry = orderEntry;
                                    objScheduleParams.paymentGraph = docgraph;
                                    objScheduleParams.objSOProcessOrderRecord = currentRecord;
                                    objScheduleParams.ObjCurrentOrder = amwOrder[0];
                                    objScheduleParams.objamwLineItems = amwLineItems;
                                    objScheduleParams.objliUsrMapping = liUsrMapping;
                                    objScheduleParams.listOfCarriers = liCarriers;
                                    objScheduleParams.CurrentOrderIndex = list.IndexOf(currentRecord);
                                    CreateSO.CreateSalesOrderandPayments(objScheduleParams);
                                }
                                else
                                {
                                    isErrorOccured = true;
                                    SOLogService.LogImportStatus(objScheduleParams, true, SOMessages.recordAlreadyImported);
                                    PXProcessing<SOProcessOrder>.SetInfo(list.IndexOf(currentRecord), SOMessages.recordAlreadyImported);
                                }
                            }
                            else
                            {
                                throw new PXException(SOMessages.configMissing);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        isErrorOccured = true;
                        SOLogService.LogImportStatus(objScheduleParams, false, ex.Message);
                        PXProcessing<SOProcessOrder>.SetError(list.IndexOf(currentRecord), ex.Message);
                    }
                }
                if (isErrorOccured)
                {
                    throw new PXException(SOMessages.showErrorMsgOrders);
                }
            }
        }
        #endregion

        #region Actions

        [PXButton]
        [PXUIField(DisplayName = SOMessages.prepareRecords, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        protected virtual void prepareRecords()
        {
            List<string> importedRecords = new List<string>();
            SOImportFilter currentFilter = Filter.Current;
            PXLongOperation.StartOperation(this, delegate ()
            {
                SOOrderEntry orderEntry = PXGraph.CreateInstance<SOOrderEntry>();
                SOImportProcess graph = PXGraph.CreateInstance<SOImportProcess>();
                SOPartialMaint logGraph = PXGraph.CreateInstance<SOPartialMaint>();
                if (currentFilter != null)
                {
                    if ((string.IsNullOrEmpty(currentFilter.IntegrationID) && currentFilter.ProcessAllTypes == false) || (!string.IsNullOrEmpty(currentFilter.IntegrationID) && currentFilter.ProcessAllTypes == true))
                    {
                        throw new PXException(SOMessages.validationIntegrationIdandprocessall);
                    }
                    if (currentFilter.TODate.Value.Date > graph.Accessinfo.BusinessDate.Value.Date)
                    {
                        throw new PXException(SOMessages.validationTodateandBusinessDate);
                    }
                    if (currentFilter.LastSyncDate > currentFilter.TODate)
                    {
                        throw new PXException(SOMessages.validationFromandTodate);
                    }
                    try
                    {
                        orderEntry.Clear();
                        SOSetupAmazonExt objSOOSetupext = orderEntry.sosetup.Current.GetExtension<SOSetupAmazonExt>();
                        if (SOHelper.IsSOPreferencesDetailsExist(orderEntry, objSOOSetupext))
                        {
                            SOAmazonSetup objSOAmazonSetupDetails = null;
                            objOrdersParameters = new OrdersParameters();
                            SOProcessOrder processrecord = null;
                            int? processedCount = 0;
                            bool isValidConfiguration = SOHelper.MarketplaceConfigurations(graph, currentFilter.IntegrationID, out objSOAmazonSetupDetails);

                            // To Get the Max date & time if the same date is processed again
                            //Dhiren-4/11/2019 - We need to revisit date range Delta logic. 
                            //SOProcessOrder objProcessOrder = new PXSelect<SOProcessOrder, Where<SOProcessOrder.integrationID, Equal<Required<SOProcessOrder.integrationID>>,
                            //                                                             And<SOProcessOrder.amazonOrderDate, Between<Required<SOProcessOrder.amazonOrderDate>, Required<SOProcessOrder.amazonOrderDate>>>>,
                            //                                                             OrderBy<Desc<SOProcessOrder.amazonOrderDate>>>(graph).SelectSingle(currentFilter.IntegrationID, currentFilter.LastSyncDate, currentFilter.TODate);

                            //currentFilter.FromDate = objProcessOrder != null && objProcessOrder.AmazonOrderDate.HasValue ? objProcessOrder.AmazonOrderDate : currentFilter.LastSyncDate;
                            currentFilter.FromDate = currentFilter.LastSyncDate;
                            Filter.Cache.Update(currentFilter);
                            if (currentFilter.IntegrationID != null && currentFilter.ProcessAllTypes == false)
                            {
                                if (isValidConfiguration)
                                {
                                    int? processId = SOHelper.GetProcessID(graph);
                                    PrepareAllRecords(graph, importedRecords, currentFilter, out processrecord, objSOAmazonSetupDetails, processId, out processedCount, logGraph);
                                    if (processedCount > 0)
                                        LogProcessCount(processedCount, currentFilter.IntegrationID, logGraph);
                                    graph.Actions.PressSave();
                                }
                                else
                                    throw new PXException(SOMessages.apidetailsMissing);
                            }
                            else if (string.IsNullOrEmpty(currentFilter.IntegrationID) && currentFilter.ProcessAllTypes == true)
                            {
                                ImportOrderList.Cache.Clear();
                                int? processId = SOHelper.GetProcessID(graph);
                                foreach (SOAmazonSetup objSOAmazonSetup in PXSelectReadonly<SOAmazonSetup, Where<SOAmazonSetup.status, Equal<True>>>.Select(graph))
                                {
                                    try
                                    {
                                        PrepareAllRecords(graph, importedRecords, currentFilter, out processrecord, objSOAmazonSetup, processId, out processedCount, logGraph);
                                        if (processedCount > 0)
                                            LogProcessCount(processedCount, objSOAmazonSetup.IntegrationID, logGraph);
                                        processId++;
                                        graph.Actions.PressSave();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                                                    ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                                                    : SOConstants.exceptionIsEmpty);
                                    }
                                }
                            }
                        }
                        else
                            throw new PXException(SOMessages.configMissing);
                    }
                    catch (Exception ex)
                    {
                        throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                            ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                            : SOConstants.exceptionIsEmpty);
                    }
                }
            });
        }

        private void PrepareAllRecords(SOImportProcess graph, List<string> importedRecords, SOImportFilter currentFilter, out SOProcessOrder processrecord, SOAmazonSetup objSOAmazonSetup, int? processID, out int? processedCount, SOPartialMaint logGraph)
        {
            processrecord = new SOProcessOrder();
            InvokeServicesCallResponse objSyncOrderResponse = new InvokeServicesCallResponse();
            List<ListOrderItems> liListOrderResponse = new List<ListOrderItems>();
            objOrdersParameters.objSOAmazonSetup = objSOAmazonSetup;
            objOrdersParameters.objSOImportFilter = currentFilter;
            objOrdersParameters.processrecord = processrecord;
            objOrdersParameters.objSyncOrderResponse = objSyncOrderResponse;
            objOrdersParameters.processID = processID;
            objOrdersParameters.importedRecords = importedRecords;
            objOrdersParameters.liListOrderResponse = liListOrderResponse;
            objOrdersParameters.objSOPartialMaint = logGraph;
            ListOrders(graph, objOrdersParameters);
            processedCount = Convert.ToInt32(graph.ImportOrderList.Cache.Inserted.Count());
        }
        #endregion

        #region Events

        protected virtual void SOImportFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            SOImportFilter row = e.Row as SOImportFilter;
            if (row != null)
            {
                PrepareRecords.SetEnabled(row.TODate != null && row.LastSyncDate != null && (!string.IsNullOrEmpty(row.IntegrationID) || row.ProcessAllTypes == true));
                PXUIFieldAttribute.SetEnabled<SOImportFilter.processAllTypes>(Filter.Cache, null, string.IsNullOrEmpty(row.IntegrationID));
                PXUIFieldAttribute.SetEnabled<SOImportFilter.integrationID>(Filter.Cache, null, (row.ProcessAllTypes != null && row.ProcessAllTypes != true));
            }
        }
        protected void ListOrders(SOImportProcess graph, OrdersParameters objOrderParams)
        {
            objServiceCallParams = new ServiceCallParameters();
            objServiceCallParams.objSOAmazonSetup = objOrderParams.objSOAmazonSetup;
            objServiceCallParams.amwOrderID = string.Empty;
            objServiceCallParams.methodCall = SOConstants.invokeListOrders;
            objServiceCallParams.fromDate = objOrderParams.objSOImportFilter.FromDate;
            objServiceCallParams.toDate = objOrderParams.objSOImportFilter.TODate;
            objOrderParams.objSyncOrderResponse = new SOOrdersServiceCall(clientOrder).InvokeServicesCalls(graph, objServiceCallParams);
            if (objOrderParams.objSyncOrderResponse != null && objOrderParams.objSyncOrderResponse.objListOrderResponse != null &&
                objOrderParams.objSyncOrderResponse.objListOrderResponse.ListOrdersResult != null && objOrderParams.objSyncOrderResponse.objListOrderResponse.ListOrdersResult.Orders.Count > 0)
            {
                objOrderParams.integrationID = objOrderParams.objSOAmazonSetup.IntegrationID;
                foreach (Order item in objOrderParams.objSyncOrderResponse.objListOrderResponse.ListOrdersResult.Orders)
                {
                    objOrderParams.objItem = item;
                    LoadDataToGrid(graph, objOrderParams);
                }
                if (objOrderParams.objSyncOrderResponse.objListOrderResponse.ListOrdersResult.NextToken != null)
                {
                    objOrderParams.nextToken = objOrderParams.objSyncOrderResponse.objListOrderResponse.ListOrdersResult.NextToken;
                    ListOrdersByNextToken(graph, objOrderParams);
                }
            }
            else
            {
                LogProcessCount(0, objOrderParams.objSOAmazonSetup.IntegrationID, objOrderParams.objSOPartialMaint);
            }
        }
        protected void ListOrdersByNextToken(SOImportProcess graph, OrdersParameters objOrderParams)
        {
            objServiceCallParams = new ServiceCallParameters();
            objServiceCallParams.objSOAmazonSetup = objOrderParams.objSOAmazonSetup;
            objServiceCallParams.amwOrderID = string.Empty;
            objServiceCallParams.methodCall = SOConstants.invokeListOrdersByNextToken;
            objServiceCallParams.fromDate = objOrderParams.objSOImportFilter.FromDate;
            objServiceCallParams.toDate = objOrderParams.objSOImportFilter.TODate;
            objServiceCallParams.nextToken = objOrderParams.nextToken;
            objServiceCallParams.liListOrderResponse = objOrderParams.liListOrderResponse;
            objOrderParams.objSyncOrderResponse = new SOOrdersServiceCall(clientOrder).InvokeServicesCalls(graph, objServiceCallParams);
            if (objOrderParams.objSyncOrderResponse != null && objOrderParams.objSyncOrderResponse.objListOrdersByNextToken != null && objOrderParams.objSyncOrderResponse.objListOrdersByNextToken.ListOrdersByNextTokenResult != null &&
                objOrderParams.objSyncOrderResponse.objListOrdersByNextToken.ListOrdersByNextTokenResult.Orders != null && objOrderParams.objSyncOrderResponse.objListOrdersByNextToken.ListOrdersByNextTokenResult.Orders.Count > 0)
            {
                objOrderParams.integrationID = objOrderParams.objSOAmazonSetup.IntegrationID;
                foreach (Order item in objOrderParams.objSyncOrderResponse.objListOrdersByNextToken.ListOrdersByNextTokenResult.Orders)
                {
                    objOrderParams.objItem = item;
                    LoadDataToGrid(graph, objOrderParams);
                }
                if (objOrderParams.objSyncOrderResponse.objListOrdersByNextToken.ListOrdersByNextTokenResult.NextToken != null)
                {
                    objOrderParams.nextToken = objOrderParams.objSyncOrderResponse.objListOrdersByNextToken.ListOrdersByNextTokenResult.NextToken;
                    ListOrdersByNextToken(graph, objOrderParams);
                }
            }
        }
        public virtual void LoadDataToGrid(SOImportProcess graph, OrdersParameters objOrderParams)
        {
            string integrationId = Filter.Current.IntegrationID == null ? objOrderParams.objSOAmazonSetup.IntegrationID : Filter.Current.IntegrationID;

            if (!SOHelper.CheckOrderExist(graph, integrationId, objOrderParams.objItem.AmazonOrderId))
            {
                objOrderParams.processrecord = new SOProcessOrder();
                objOrderParams.processrecord.ProcessID = objOrderParams.processID;
                objOrderParams.processrecord.AmazonOrderID = objOrderParams.objItem.AmazonOrderId;
                objOrderParams.processrecord.BuyerEmailID = objOrderParams.objItem.BuyerEmail;
                objOrderParams.processrecord.AmazonOrderDate = objOrderParams.objItem.PurchaseDate != null ? objOrderParams.objItem.PurchaseDate : (DateTime?)null;
                objOrderParams.processrecord.AmazonOrderLastUpdated = objOrderParams.objItem.LastUpdateDate != null ? objOrderParams.objItem.LastUpdateDate : (DateTime?)null;
                objOrderParams.processrecord.OrderAmount = objOrderParams.objItem.OrderTotal != null ? Convert.ToDecimal(objOrderParams.objItem.OrderTotal.Amount) : 0m;
                objOrderParams.processrecord.AmazonStatus = objOrderParams.objItem.OrderStatus;               
                objOrderParams.processrecord.SynDatetime = PX.Common.PXTimeZoneInfo.Now;
                objOrderParams.processrecord.IntegrationID = objOrderParams.integrationID;
                string xmlFeedContent = SOHelper.ListToXMLConverstion(objOrderParams.objItem);
                objOrderParams.processrecord.OrderSchema = xmlFeedContent;
                objOrderParams.processrecord.PrepareStatus = true;
                objOrderParams.processrecord.ImportStatus = false;
                graph.ImportOrderList.Cache.Insert(objOrderParams.processrecord);
            }
        }
        public void LogProcessCount(int? processCount, string integrationId, SOPartialMaint logGraph)
        {
            Filter.Current.TotalImportedRecords = 0;
            Filter.Current.TotalRecordsToImport = processCount;
            Filter.Current.TotalFailedRecords = 0;
            Filter.Update(Filter.Current);
            SOLogService.LogImportCount(Filter.Current, integrationId, logGraph, null, SOConstants.importorders, false);
        }
        #endregion
    }

    public class OrdersParameters
    {
        public SOAmazonSetup objSOAmazonSetup { get; set; }
        public SOImportProcess.SOImportFilter objSOImportFilter { get; set; }
        public SOProcessOrder processrecord { get; set; }
        public InvokeServicesCallResponse objSyncOrderResponse { get; set; }
        public string nextToken { get; set; }
        public List<ListOrderItems> liListOrderResponse { get; set; }
        public List<string> importedRecords { get; set; }
        public int? processID { get; set; }
        public Order objItem { get; set; }
        public string integrationID { get; set; }
        public SOPartialMaint objSOPartialMaint { get; set; }
    }
}