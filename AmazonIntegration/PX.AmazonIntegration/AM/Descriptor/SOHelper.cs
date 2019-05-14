using System;
using System.Collections.Generic;
using PX.Data;
using MarketplaceWebServiceOrders.Model;
using PX.Objects.CS;
using PX.Objects.SO;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq;
using PX.SM;

namespace AmazonIntegration
{
    #region Helper methods
    public class SOHelper
    {
        #region Variables
        public static readonly MarketplaceWebServiceOrders.MarketplaceWebServiceOrders clientOrder;
        #endregion 

        internal static string GetIntegrationType(string integrationId, string request)
        {
            switch (integrationId)
            {
                case SOConstants.AMIntegrationType.FBM:
                case SOConstants.AMIntegrationType.FBA:
                    return request == SOConstants.domainName ? SOConstants.usDomain : request == SOConstants.serviceUrl ? SOConstants.serviceUrlNA : null;
                case SOConstants.AMIntegrationType.CAFBA:
                    return request == SOConstants.domainName ? SOConstants.caDomain : request == SOConstants.serviceUrl ? SOConstants.serviceUrlNA : null;
                case SOConstants.AMIntegrationType.UKFBA:
                    return request == SOConstants.domainName ? SOConstants.ukDomain : request == SOConstants.serviceUrl ? SOConstants.serviceUrlEU : null;
                case SOConstants.AMIntegrationType.JPFBA:
                    return request == SOConstants.domainName ? SOConstants.jpDomain : request == SOConstants.serviceUrl ? SOConstants.serviceUrlJP : null;
                case SOConstants.AMIntegrationType.DEFBA:
                    return request == SOConstants.domainName ? SOConstants.deDomain : request == SOConstants.serviceUrl ? SOConstants.serviceUrlEU : null;
                case SOConstants.AMIntegrationType.INFBA:
                    return request == SOConstants.domainName ? SOConstants.inDomain : request == SOConstants.serviceUrl ? SOConstants.serviceUrlIN : null;
                default:
                    return null;
            }
        }
        internal static void GetFilterValues(string intergrationType, out List<string> liFulfillmentChannel, out List<string> liOrderStatus)
        {
            liFulfillmentChannel = new List<string>();
            liOrderStatus = new List<string>();
            switch (intergrationType)
            {
                case SOConstants.AMIntegrationType.FBA:
                case SOConstants.AMIntegrationType.CAFBA:
                case SOConstants.AMIntegrationType.UKFBA:
                case SOConstants.AMIntegrationType.JPFBA:
                case SOConstants.AMIntegrationType.DEFBA:
                case SOConstants.AMIntegrationType.INFBA:
                    liFulfillmentChannel.Add(SOConstants.fulfillmentChannelAFN);
                    liOrderStatus.Add(SOConstants.shipped);
                    break;
                case SOConstants.AMIntegrationType.FBM:
                    liFulfillmentChannel.Add(SOConstants.fulfillmentChannelMFN);
                    liOrderStatus.Add(SOConstants.unshipped);
                    liOrderStatus.Add(SOConstants.partiallyShipped);
                    break;
            }
        }
        internal static bool CheckOrderExist(PXGraph graph, string integrationId, string amazonOrderId)
        {
            SOProcessOrder recordExist = PXSelectReadonly<SOProcessOrder, Where<SOProcessOrder.integrationID, Equal<Required<SOProcessOrder.integrationID>>,
                                                                             And<SOProcessOrder.amazonOrderID, Equal<Required<SOProcessOrder.amazonOrderID>>>>>
                                                                             .Select(graph, integrationId, amazonOrderId);
            return recordExist != null;
        }
        internal static void PrepareRecord(SOScheduleProcess graph, PrepareAndImportOrdersParams objScheduleParams, ref bool isErrorOccured)
        {
            SOProcessOrder objprocessorder = null;         
            foreach (Order currentOrder in objScheduleParams.amwOrders)
            {
                try
                {
                    objScheduleParams.objSOOrderEntry.Clear();
                    objScheduleParams.objSOPartialMaint.Clear();
                    objScheduleParams.paymentGraph.Clear();
                    if (!CheckOrderExist(objScheduleParams.objSOOrderEntry, objScheduleParams.objSOPrepareAndImport.IntegrationID, currentOrder.AmazonOrderId))
                    {
                        objprocessorder = new SOProcessOrder();
                        objprocessorder.ProcessID = objScheduleParams.objSOPrepareAndImport != null &&
                                                    objScheduleParams.objSOPrepareAndImport.ProcessID != null ? objScheduleParams.objSOPrepareAndImport.ProcessID : 1;
                        objprocessorder.IntegrationID = objScheduleParams.objSOPrepareAndImport != null &&
                                                        objScheduleParams.objSOPrepareAndImport.IntegrationID != null ? objScheduleParams.objSOPrepareAndImport.IntegrationID
                                                        : string.Empty;
                        objprocessorder.AmazonOrderID = currentOrder.AmazonOrderId;
                        objprocessorder.BuyerEmailID = currentOrder.BuyerEmail;
                        objprocessorder.AmazonOrderDate = currentOrder.PurchaseDate != null ? currentOrder.PurchaseDate : (DateTime?)null;
                        objprocessorder.AmazonOrderLastUpdated = currentOrder.LastUpdateDate != null ? currentOrder.LastUpdateDate : (DateTime?)null;
                        objprocessorder.OrderAmount = (currentOrder.OrderTotal != null && currentOrder.OrderTotal.Amount != null)
                                                         ? Convert.ToDecimal(currentOrder.OrderTotal.Amount) : 0m;
                        objprocessorder.AmazonStatus = currentOrder.OrderStatus;
                        objprocessorder.SynDatetime = objScheduleParams.businessDateTime;
                        objprocessorder.PrepareStatus = true;
                        objprocessorder.ImportStatus = false;
                        string xmlFeedContent = ListToXMLConverstion(currentOrder);
                        objprocessorder.OrderSchema = xmlFeedContent;
                        objScheduleParams.objSOPartialMaint.ProcessOrder.Cache.Insert(objprocessorder);
                        objScheduleParams.objSOPartialMaint.Actions.PressSave();
                        SOLogService.LogImportCount(null, objprocessorder.IntegrationID, objScheduleParams.objSOPartialMaint, objprocessorder.ProcessID,
                                                    SOConstants.scheduleimportorders, false);

                        SOProcessOrder processRecord = PXSelect<SOProcessOrder,
                                                        Where<SOProcessOrder.integrationID, Equal<Required<SOProcessOrder.integrationID>>,
                                                        And<SOProcessOrder.processID, Equal<Required<SOProcessOrder.processID>>,
                                                        And<SOProcessOrder.amazonOrderID, Equal<Required<SOProcessOrder.amazonOrderID>>>>>,
                                                        OrderBy<Desc<SOProcessOrder.lineNumber>>>.Select(objScheduleParams.objSOOrderEntry,
                        objScheduleParams.objSOPrepareAndImport.IntegrationID,
                        objScheduleParams.objSOPrepareAndImport.ProcessID, currentOrder.AmazonOrderId);
                        objScheduleParams.amazonOrderID = objprocessorder.AmazonOrderID;
                        objScheduleParams.ObjCurrentOrder = currentOrder;
                        objScheduleParams.objSOProcessOrderRecord = processRecord;
                        GetAmazonOrderItems(graph, objScheduleParams);
                    }
                    else
                    {
                        SOLogService.LogImportCount(null, objScheduleParams.objSOAmazonSetup.IntegrationID, objScheduleParams.objSOPartialMaint, objScheduleParams.objSOPrepareAndImport.ProcessID,
                                              SOConstants.scheduleimportorders, true);
                    }
                }
                catch (Exception ex)
                {
                    isErrorOccured = true;
                    SOLogService.LogImportStatus(objScheduleParams, false, ex.Message);
                }
            }
        }
        public static void GetAmazonOrderItems(SOScheduleProcess graph, PrepareAndImportOrdersParams objScheduleParams)
        {
            List<SOFieldMapping> liUsrMapping = new List<SOFieldMapping>();
            InvokeServicesCallResponse objSyncOrderResponse = new InvokeServicesCallResponse();
            List<OrderItem> amwLineItems = new List<OrderItem>();
            objScheduleParams.objSOOrderEntry.Clear();
            objScheduleParams.objSOPartialMaint.Clear();
            objSyncOrderResponse = new InvokeServicesCallResponse();
            amwLineItems = new List<OrderItem>();
            ServiceCallParameters objServiceCallParams = new ServiceCallParameters();
            objServiceCallParams.objSOAmazonSetup = objScheduleParams.objSOAmazonSetup;
            objServiceCallParams.amwOrderID = objScheduleParams.amazonOrderID;
            objServiceCallParams.methodCall = SOConstants.invokeListOrderItems;
            objSyncOrderResponse = new SOOrdersServiceCall(clientOrder).InvokeServicesCalls(graph, objServiceCallParams);
            amwLineItems = objSyncOrderResponse != null && objSyncOrderResponse.objListOrderItemsResponse != null &&
            objSyncOrderResponse.objListOrderItemsResponse.ListOrderItemsResult != null && objSyncOrderResponse.objListOrderItemsResponse.ListOrderItemsResult.OrderItems.Count > 0 ? objSyncOrderResponse.objListOrderItemsResponse.ListOrderItemsResult.OrderItems : amwLineItems;
            if (objScheduleParams.ObjCurrentOrder != null)
            {
                objScheduleParams.objamwLineItems = amwLineItems;
                objScheduleParams.objliUsrMapping = liUsrMapping;
                CreateSO.CreateSalesOrderandPayments(objScheduleParams);
            }
        }

        internal static string GetTrackingForFBAOrders(string amazonOrderId, List<ShipmentInformarion> liReportsRes, out string carrier)
        {
            string trackingNbr = string.Empty;
            carrier = string.Empty;
            foreach (ShipmentInformarion items in liReportsRes.AsEnumerable().Where(x => x.amazonOrderID.Trim() == amazonOrderId.Trim()))
            {
                trackingNbr = items.shipmentTrackingNbr;
                carrier = items.shipmentCarrier;
                break;
            }
            return trackingNbr;
        }
        internal static string GetStateID(SOOrderEntry graph, string stateName, string countryId)
        {
            State objstate = PXSelect<State, Where2<Where<State.countryID, Equal<Required<State.countryID>>>,
                                                      And<Where<State.stateID, Equal<Required<State.stateID>>,
                                                      Or<State.name, Equal<Required<State.name>>>>>>>.Select(graph, countryId, stateName, stateName);
            return (objstate != null && !string.IsNullOrEmpty(objstate.StateID) ? objstate.StateID : string.Empty);
        }

        internal static string ListToXMLConverstion(object obj)
        {
            var nms = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(obj.GetType());
            nms.Add(SOConstants.xsi, SOConstants.ordersURL);
            var settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Document,
                OmitXmlDeclaration = true,
                CloseOutput = true,
                Indent = true,
                IndentChars = "  ",
                NewLineHandling = NewLineHandling.Replace
            };
            using (StringWriter stream = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, obj, nms);
                }
                return Convert.ToString(stream);
            }
        }

        internal static List<SOFieldMapping> GetUsrFieldMapping(SOOrderEntry graph, string integrationID)
        {
            List<SOFieldMapping> liUsrMapping = new List<SOFieldMapping>();
            foreach (SOFieldMapping usrFieldMapping in PXSelect<SOFieldMapping, Where<SOFieldMapping.isActive, Equal<True>, And<SOFieldMapping.integrationID, Equal<Required<SOFieldMapping.integrationID>
                                                                          >>>>.Select(graph, integrationID))
            {
                liUsrMapping.Add(usrFieldMapping);
            }
            return liUsrMapping;
        }

        internal static int DelayProcess(PXGraph graph, string apiName)
        {
            int delayTime;
            SOThrottleConstants objDelayProcess = PXSelect<SOThrottleConstants,
                                                  Where<SOThrottleConstants.apiname, Equal<Required<SOThrottleConstants.apiname>>>>.Select(graph, apiName);
            if (objDelayProcess != null && objDelayProcess.DelayTime > 0)
            {
                delayTime = objDelayProcess.DelayTime;
            }
            else
            {
                throw new PXException(SOConstants.delayTImeNotFound);
            }
            return delayTime;
        }
        internal static Order SchemaDeserialization(PXGraph graph, string amazonOrderId)
        {
            Order orderSchema = null;
            SOProcessOrder objProcessOrder = PXSelect<SOProcessOrder, Where<SOProcessOrder.amazonOrderID, Equal<Required<SOProcessOrder.amazonOrderID>>>>.Select(graph, amazonOrderId);
            if (objProcessOrder != null)
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Order), new XmlRootAttribute(SOConstants.orderTag));
                using (TextReader reader = new StringReader(objProcessOrder.OrderSchema))
                {
                    orderSchema = (Order)deserializer.Deserialize(reader);
                }
                if (orderSchema == null)
                    throw new PXException(SOMessages.schemaDoesnotExist);
            }
            return orderSchema;
        }
        internal static bool MarketplaceConfigurations(PXGraph graph, string integrationId, out SOAmazonSetup objSOAmazonSetup)
        {
            objSOAmazonSetup = PXSelect<SOAmazonSetup, Where<SOAmazonSetup.integrationID, Equal<Required<SOAmazonSetup.integrationID>>,
                                                          And<SOAmazonSetup.status, Equal<True>>
                                                          >>.Select(graph, integrationId);
            if (objSOAmazonSetup != null && !string.IsNullOrEmpty(objSOAmazonSetup.IntegrationID) && !string.IsNullOrEmpty(objSOAmazonSetup.AccessKey) &&
                !string.IsNullOrEmpty(objSOAmazonSetup.SecretKey) && !string.IsNullOrEmpty(objSOAmazonSetup.SellerId) && !string.IsNullOrEmpty(objSOAmazonSetup.AuthToken)
                && !string.IsNullOrEmpty(objSOAmazonSetup.MarketplaceId) && !string.IsNullOrEmpty(objSOAmazonSetup.IntegrationType))
            {
                return true;
            }
            return false;
        }

        internal static int? GetProcessID(PXGraph graph)
        {
            SOOrderProcessLog objProcess = PXSelectGroupBy<SOOrderProcessLog, Aggregate<Max<SOOrderProcessLog.processID>>>.Select(graph);
            return objProcess != null && objProcess.ProcessID != null ? (objProcess.ProcessID + 1) : 1;
        }

        public static string ObjectToXMLConversion(dynamic obj, string tagName, bool omitXml)
        {
            var nms = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(obj.GetType());
            if (tagName == SOConstants.amazonEnvelope)
            {
                nms.Add(SOConstants.xsi, SOConstants.xmlSchema);
                nms.Add(SOConstants.noNamespaceSchemaLocation, SOConstants.amznEnvelope);
            }
            else
            {
                nms.Add("", "");
            }
            tagName = string.Empty;
            var settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Document,
                OmitXmlDeclaration = omitXml,
                CloseOutput = true,
                Indent = true,
                IndentChars = "  ",
                NewLineHandling = NewLineHandling.Replace
            };
            using (var stream = new Utf8StringWriter())
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, obj, nms);
                }
                return Convert.ToString(stream);
            }
        }
        internal static bool IsSOPreferencesDetailsExist(SOOrderEntry orderEntry,  SOSetupAmazonExt objSOOSetupext)
        {
            if (orderEntry.sosetup.Current != null && objSOOSetupext != null && objSOOSetupext.UsrGuestCustID.HasValue 
                && !string.IsNullOrEmpty(objSOOSetupext.UsrAmazonTaxZoneID) && !string.IsNullOrEmpty(objSOOSetupext.UsrAmazonTaxID) 
                && !string.IsNullOrEmpty(objSOOSetupext.UsrAmazonShipVia) && !string.IsNullOrEmpty(objSOOSetupext.UsrAmazonPaymentMethodID) && objSOOSetupext.UsrAmazonInitialFromDate !=null)
                return true;
            else
                return false;
        }
    }
    #endregion
}