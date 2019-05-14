using System;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.SO;
using PX.Objects.CS;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using PX.Objects.CR;
using PX.Objects.AR;
using PX.Objects.IN;
using System.Collections;
using System.Linq;
using System.Xml.Serialization;
using PX.Common;

namespace AmazonIntegration
{
    [PXProjection(typeof(Select2<SOShipment, InnerJoin<SOOrderShipment,
                                             On<SOOrderShipment.shipmentNbr, Equal<SOShipment.shipmentNbr>,
                                             And<SOShipment.status, Equal<SOShipmentStatus.confirmed>>>,
                                             InnerJoin<SOOrder, On<SOOrderShipment.orderType, Equal<SOOrder.orderType>,
                                             And<SOOrderShipment.orderNbr, Equal<SOOrder.orderNbr>,
                                             And<SOOrderAmazonExt.usrAmazonOrderID, IsNotNull,
                                             And2<Where<SOShipmentAmazonExt.usrSubmitFeedupdate, Equal<False>,
                                             Or<SOShipmentAmazonExt.usrSubmitFeedupdate, IsNull>>,
                                             And<Where<SOOrder.status, Equal<SOOrderStatus.completed>,
                                             Or<SOOrder.status, Equal<SOOrderStatus.backOrder>>>>>>>>,
                                             InnerJoin<SOProcessOrder, On<SOProcessOrder.amazonOrderID, Equal<SOOrderAmazonExt.usrAmazonOrderID>
                                             >>>>>))]
    [Serializable]
    public class ProjectionShipmentDAC : IBqlTable
    {
        #region Selected
        public abstract class selected : IBqlField
        {
        }
        protected bool? _Selected = false;
        [PXBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected
        {
            get
            {
                return _Selected;
            }
            set
            {
                _Selected = value;
            }
        }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, BqlField = typeof(SOShipment.status))]
        [PXUIField(DisplayName = "Status")]
        [SOShipmentStatus.List()]
        public virtual string Status { get; set; }
        public abstract class status : IBqlField { }

        #endregion

        #region ShipmentNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, BqlField = typeof(SOShipment.shipmentNbr))]
        [PXUIField(DisplayName = "Shipment Nbr.")]
        public virtual string ShipmentNbr { get; set; }
        public abstract class shipmentNbr : IBqlField { }

        #endregion

        #region ShipDate

        [PXDBDate(BqlField = typeof(SOShipment.shipDate))]
        [PXUIField(DisplayName = "Shipment Date")]
        public virtual DateTime? ShipDate { get; set; }
        public abstract class shipDate : IBqlField { }
        #endregion

        #region CustomerID        

        [PXDBInt(BqlField = typeof(SOShipment.customerID))]
        [PXUIField(DisplayName = "Customer")]
        [PXSelector(typeof(Customer.bAccountID), new Type[] { typeof(Customer.acctCD), typeof(Customer.acctName) },
            SubstituteKey = typeof(Customer.acctCD), DescriptionField = typeof(BAccount.acctName))]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : IBqlField { }

        #endregion

        #region Shipped Quantity        

        [PXDBDecimal(BqlField = typeof(SOShipment.shipmentQty))]
        [PXUIField(DisplayName = "Shipped Quantity")]
        public virtual decimal? ShipmentQty { get; set; }
        public abstract class shipmentQty : IBqlField { }

        #endregion

        #region SiteID

        [PXDBInt(BqlField = typeof(SOShipment.siteID))]
        [PXUIField(DisplayName = "Warehouse")]
        [PXSelector(typeof(Search<INSite.siteID, Where<INSite.active, Equal<True>>>), typeof(INSite.siteCD), typeof(INSite.descr), SubstituteKey = typeof(INSite.siteCD))]
        public virtual int? SiteID { get; set; }
        public abstract class siteID : IBqlField { }

        #endregion

        #region ShipVia

        [PXDBString(15, IsUnicode = true, BqlField = typeof(SOShipment.shipVia))]
        [PXUIField(DisplayName = "Ship Via")]
        public virtual string ShipVia { get; set; }
        public abstract class shipVia : IBqlField { }

        #endregion      

        #region ShipmentWeight

        [PXDBDecimal(6, BqlField = typeof(SOShipment.shipmentWeight))]
        [PXUIField(DisplayName = "Shipped Weight")]
        public virtual decimal? ShipmentWeight { get; set; }
        public abstract class shipmentWeight : IBqlField { }

        #endregion

        #region ShipmentVolume

        [PXDBDecimal(6, BqlField = typeof(SOShipment.shipmentVolume))]
        [PXUIField(DisplayName = "Shipped Volume")]
        public virtual decimal? ShipmentVolume { get; set; }
        public abstract class shipmentVolume : IBqlField { }

        #endregion

        #region AmazonOrderID

        [PXDBString(50, IsUnicode = true, InputMask = "", BqlField = typeof(SOProcessOrder.amazonOrderID))]
        [PXUIField(DisplayName = "Amazon Order ID")]
        public virtual string AmazonOrderID { get; set; }
        public abstract class amazonOrderID : IBqlField { }

        #endregion

        #region IntegrationID

        [PXDBString(30, IsUnicode = true, InputMask = "", BqlField = typeof(SOProcessOrder.integrationID))]
        [PXUIField(DisplayName = "Integration ID")]
        public virtual string IntegrationID { get; set; }
        public abstract class integrationID : IBqlField { }

        #endregion

        #region OrderQty

        [PXDBDecimal(BqlField = typeof(SOOrder.orderQty))]
        [PXUIField(DisplayName = "Ordered Qty.", Visible = false)]
        public virtual decimal? OrderQty { get; set; }
        public abstract class orderQty : IBqlField { }
        #endregion

        #region AcmOrderNbr

        [PXDBString(15, IsKey = true, IsUnicode = true, BqlField = typeof(SOOrder.orderNbr))]
        [PXUIField(DisplayName = "Order Nbr.", Visible = false)]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : IBqlField { }
        #endregion

        #region ShipmentType

        [PXDBString(1, IsFixed = true, BqlField = typeof(SOShipment.shipmentType))]
        [PXUIField(DisplayName = "Shipment Type.", Visible = false)]
        public virtual string ShipmentType { get; set; }
        public abstract class shipmentType : IBqlField { }
        #endregion
    }

    public class SOSubmitFeedProcess : PXGraph<SOSubmitFeedProcess>
    {
        #region View
        public PXCancel<SubmitFeedFilter> Cancel;
        public PXFilter<SubmitFeedFilter> Filter;
        [PXFilterable]

        public PXFilteredProcessing<ProjectionShipmentDAC, SubmitFeedFilter,
                                        Where<ProjectionShipmentDAC.shipDate, LessEqual<Current<SubmitFeedFilter.tODate>>,
                                        And<ProjectionShipmentDAC.integrationID, Equal<Current<SubmitFeedFilter.integrationID>>>
                                        >> SubmitFeedList;
        #endregion

        #region Variables
        private static readonly MarketplaceWebService.MarketplaceWebService clientFeed = null;
        AmazonEnvelope objEnvelope;
        Header objHeader;

        #endregion

        public SOSubmitFeedProcess()
        {
            SubmitFeedList.SetProcessCaption(SOConstants.btnSubmitFeed);
            SubmitFeedList.SetProcessAllCaption(SOConstants.btnSubmitFeedALL);
            SubmitFeedFilter currentFilter = this.Filter.Current;
            SubmitFeedList.SetProcessDelegate(
           delegate (List<ProjectionShipmentDAC> list)
           {
               SubmitRecords(list, currentFilter);
           });
        }

        public static void SubmitRecords(List<ProjectionShipmentDAC> list, SubmitFeedFilter currentFilter)
        {
            SOSubmitFeedProcess graph = PXGraph.CreateInstance<SOSubmitFeedProcess>();
            if (currentFilter != null)
            {
                if (string.IsNullOrEmpty(currentFilter.IntegrationID))
                    throw new PXException(SOMessages.validateIntegrationId);
                if (currentFilter.TODate == null)
                    throw new PXException(SOMessages.validateToDate);
            }
            else
                throw new PXException(SOMessages.validateFilter);
            graph.AmazonSubmitFeed(list, currentFilter);
        }
        public virtual void AmazonSubmitFeed(List<ProjectionShipmentDAC> list, SubmitFeedFilter currentFilter)
        {
            SOPartialMaint logGraph = PXGraph.CreateInstance<SOPartialMaint>();
            int? processId = list.Count > 0 ? SOLogService.LogSubmitCount(logGraph, currentFilter.IntegrationID, list.Count) : null;
            if (!(processId > 0)) return;
            SubmitFeedParamaters objSubmitFeedParams = null;
            StringBuilder sbXmlFeedData = new StringBuilder();
            objEnvelope = new AmazonEnvelope();
            objHeader = new Header();
            objHeader.DocumentVersion = SOConstants.documentVersion;
            objEnvelope.MessageType = SOConstants.orderFulfillment;
            objEnvelope.Header = objHeader;
            List<Message> liMsgContent = new List<Message>();
            UTF8Encoding encoding = new UTF8Encoding();
            double megaBytesLength = 0.0d;
            bool isErrorOccured = false;
            Regex xmlEmptytagsRemove = new Regex(@"(\s)*<(\w:\w|\w)*(\s)*/>");
            AMSubmitFeedServiceCall.AmazonEnvelope resAMEnv = null;
            List<Item> listItems = null;
            foreach (ProjectionShipmentDAC currentRecord in list)
            {
                SOAmazonSetup objSOAmazonSetup = null;
                Item objitem = null;
                SOHelper.MarketplaceConfigurations(logGraph, currentFilter.IntegrationID, out objSOAmazonSetup);
                objHeader.MerchantIdentifier = objSOAmazonSetup.SellerId;
                objSubmitFeedParams = new SubmitFeedParamaters();
                listItems = new List<Item>();
                try
                {
                    logGraph.Clear();
                    foreach (PXResult<SOShipment, SOShipLine, Carrier, SOPackageDetail> result in PXSelectJoin<SOShipment, InnerJoin<SOShipLine, On<SOShipment.shipmentType, Equal<SOShipLine.shipmentType>,
                                                            And<SOShipment.shipmentNbr, Equal<SOShipLine.shipmentNbr>>>,
                                                            InnerJoin<Carrier, On<SOShipment.shipVia, Equal<Carrier.carrierID>>,
                                                            LeftJoin<SOPackageDetail, On<SOPackageDetail.shipmentNbr, Equal<SOShipLine.shipmentNbr>>>>>,
                                                            Where<SOShipLine.shipmentNbr, Equal<Required<SOShipLine.shipmentNbr>>>,
                                                            OrderBy<Desc<SOPackageDetail.lineNbr>>>.Select(logGraph, currentRecord.ShipmentNbr))
                    {
                        SOShipment objShipment = (SOShipment)result;
                        SOShipLine objShipLine = (SOShipLine)result;
                        Carrier objCarrier = (Carrier)result;
                        SOPackageDetail objPackageDetails = (SOPackageDetail)result;
                        objitem = null;
                        if (objShipment != null && objShipLine != null && objPackageDetails != null && objPackageDetails.Confirmed != null && objPackageDetails.Confirmed == true && !string.IsNullOrEmpty(objPackageDetails.TrackNumber))
                        {
                            SOShipmentAmazonExt objShipmentExt = PXCache<SOShipment>.GetExtension<SOShipmentAmazonExt>(objShipment);
                            objSubmitFeedParams.soType = objShipLine.OrigOrderType;
                            objSubmitFeedParams.acmOrderNbr = objShipLine.OrigOrderNbr;
                            objSubmitFeedParams.shipmentType = objShipment.ShipmentType;
                            objSubmitFeedParams.shipmentNbr = objShipment.ShipmentNbr;
                            objSubmitFeedParams.shipmentDate = objShipment.ShipDate;
                            objSubmitFeedParams.carrierCode = !String.IsNullOrEmpty(objShipmentExt.UsrAmazonCarrierCode) ?
                                                            objShipmentExt.UsrAmazonCarrierCode : objShipment.ShipVia;
                            objSubmitFeedParams.shipVia = (objCarrier == null || String.IsNullOrEmpty(objCarrier.PluginMethod)) ?
                                                                objShipment.ShipVia : objCarrier.PluginMethod;
                            objSubmitFeedParams.trackingNbr = objPackageDetails.TrackNumber;
                            SOShipLineAmazonExt objShLineExt = objShipLine.GetExtension<SOShipLineAmazonExt>();
                            objSubmitFeedParams.amazonOrderID = objShLineExt != null && !string.IsNullOrEmpty(objShLineExt.UsrAmazonOrderID) ?
                                objShLineExt.UsrAmazonOrderID : string.Empty;
                            objitem = new Item();
                            objitem.AmazonOrderItemCode = objShLineExt != null && !string.IsNullOrEmpty(objShLineExt.UsrAMOrderItemID) ?
                                objShLineExt.UsrAMOrderItemID : string.Empty;
                            objitem.Quantity = objShipLine != null ? Convert.ToInt32(objShipLine.ShippedQty) : 0;
                            listItems.Add(objitem); 
                        }
                        else
                            throw new PXException(SOMessages.shipmentDetailsMissing);
                    }
                    objSubmitFeedParams.objPartialMaint = logGraph;
                    objSubmitFeedParams.indexOfCurrentOrder = list.IndexOf(currentRecord);
                    objSubmitFeedParams.objSOAmazonSetup = objSOAmazonSetup;
                    objSubmitFeedParams.processID = processId;
                    if (listItems != null && listItems.Count > 0)
                    {
                        objSubmitFeedParams.liShipItem = listItems;
                    }
                    Message objMessage = GetListOfMessagesContent(objSubmitFeedParams);
                    if (objMessage.MessageID != null)
                        liMsgContent.Add(objMessage);
                }
                catch (Exception ex)
                {
                    isErrorOccured = true;
                    objSubmitFeedParams.feedMessage = ex.Message;
                    objSubmitFeedParams.importOrderStatus = false;
                    objSubmitFeedParams.xmlMessage = string.Empty;
                    SOLogService.LogSubmitStatus(objSubmitFeedParams);
                    PXProcessing<ProjectionShipmentDAC>.SetError(list.IndexOf(currentRecord), ex.Message);
                }
            }
            objEnvelope.MessageBody = SOConstants.messagebody;
            string resultFeed = SOHelper.ObjectToXMLConversion(objEnvelope, SOConstants.amazonEnvelope, false);

            if (liMsgContent.Count > 0)
            {
                foreach (Message listobj in liMsgContent)
                {
                    if (megaBytesLength >= SOConstants.feedSize)
                        break;
                    sbXmlFeedData.Append(SOHelper.ObjectToXMLConversion(listobj, SOConstants.messageBody, true) + SOConstants.getNewLine);
                    var byteData = encoding.GetBytes(sbXmlFeedData.ToString());
                    megaBytesLength = (byteData.Length / 1024f) / 1024f;
                }
                string xmlFeedContent = xmlEmptytagsRemove.Replace(resultFeed.Replace(SOConstants.msgbodyTag, Convert.ToString(sbXmlFeedData)), string.Empty);
                PXTrace.WriteInformation(xmlFeedContent);
                resAMEnv = new AMSubmitFeedServiceCall(clientFeed).InvokeServicesCalls(objSubmitFeedParams, xmlFeedContent);
            }
            if (resAMEnv != null && resAMEnv.Message != null && resAMEnv.Message.ProcessingReport != null && resAMEnv.Message.ProcessingReport.ProcessingSummary != null
                && resAMEnv.Message.ProcessingReport.ProcessingSummary.MessagesProcessed == resAMEnv.Message.ProcessingReport.ProcessingSummary.MessagesSuccessful
                && resAMEnv.Message.ProcessingReport.ProcessingSummary.MessagesWithError == 0)
            {
                foreach (ProjectionShipmentDAC currentRecord in list)
                {
                    UpdateSubmitLogDetails(list, objSubmitFeedParams, currentRecord, null);
                }
            }
            else if (resAMEnv != null && resAMEnv.Message != null && resAMEnv.Message.ProcessingReport != null
                     && (resAMEnv.Message.ProcessingReport.ProcessingSummary.MessagesWithError > 0
                     || resAMEnv.Message.ProcessingReport.ProcessingSummary.MessagesSuccessful > 0))
            {
                bool isSubmitted;
                foreach (ProjectionShipmentDAC currentRecord in list)
                {
                    isSubmitted = false;
                    foreach (var item in resAMEnv.Message.ProcessingReport.Result.AsEnumerable().Where(x => x.MessageID == currentRecord.ShipmentNbr))
                    {
                        isSubmitted = true;
                        isErrorOccured = true;
                        UpdateSubmitLogDetails(list, objSubmitFeedParams, currentRecord, item);
                    }
                    if (!isSubmitted)
                        UpdateSubmitLogDetails(list, objSubmitFeedParams, currentRecord, null);
                }
            }
            if (isErrorOccured)
                throw new PXException(SOMessages.showErrorMsgFeeds);

        }
        private static void UpdateSubmitLogDetails(List<ProjectionShipmentDAC> list, SubmitFeedParamaters objSubmitFeedParams, ProjectionShipmentDAC currentRecord, AMSubmitFeedServiceCall.Result item)
        {
            objSubmitFeedParams.acmOrderNbr = currentRecord.OrderNbr;
            objSubmitFeedParams.amazonOrderID = currentRecord.AmazonOrderID;
            objSubmitFeedParams.objSOAmazonSetup.IntegrationID = currentRecord.IntegrationID;
            objSubmitFeedParams.shipmentNbr = currentRecord.ShipmentNbr;
            objSubmitFeedParams.shipmentType = currentRecord.ShipmentType;

            if (item != null)
            {
                objSubmitFeedParams.importOrderStatus = false;
                objSubmitFeedParams.xmlMessage = item.ResultDescription;
                objSubmitFeedParams.feedMessage = item.ResultDescription;
                PXProcessing<ProjectionShipmentDAC>.SetError(list.IndexOf(currentRecord), item.ResultDescription);
            }
            else
            {
                objSubmitFeedParams.importOrderStatus = true;
                objSubmitFeedParams.xmlMessage = SOConstants.feedSubmitted;
                objSubmitFeedParams.feedMessage = SOConstants.feedSubmitted;
                PXProcessing<ProjectionShipmentDAC>.SetInfo(list.IndexOf(currentRecord), SOConstants.feedSubmitted);
            }
            SOLogService.LogSubmitStatus(objSubmitFeedParams);
        }
        private static Message GetListOfMessagesContent(SubmitFeedParamaters objSubmitFeedParams)
        {
            Message objMessage = new Message();
            objMessage.MessageID = objSubmitFeedParams.shipmentNbr;
            OrderFulfillment objOrderFulfillment = new OrderFulfillment();
            FulfillmentData objFulfillmentData = new FulfillmentData();
            objOrderFulfillment.AmazonOrderID = objSubmitFeedParams.amazonOrderID;
            
            objOrderFulfillment.FulfillmentDate = new DateTimeOffset(Convert.ToDateTime(objSubmitFeedParams.shipmentDate),
                                                      PXTimeZoneInfo.FindSystemTimeZoneById("GMTM0800A").UtcOffset).ToString("yyyy-MM-ddTHH:mm:sszzz");

            PXTrace.WriteInformation(String.Format("Shipment# {0} FulfillmentDate {1}",
                objMessage.MessageID, objOrderFulfillment.FulfillmentDate));
            objFulfillmentData.CarrierName = objSubmitFeedParams.carrierCode;
            objFulfillmentData.ShippingMethod = objSubmitFeedParams.shipVia;
            objFulfillmentData.ShipperTrackingNumber = objSubmitFeedParams.trackingNbr;
            objOrderFulfillment.FulfillmentData = objFulfillmentData;
            if (objSubmitFeedParams.liShipItem != null && objSubmitFeedParams.liShipItem.Count > 0)
                objOrderFulfillment.Item = objSubmitFeedParams.liShipItem;
            objMessage.OrderFulfillment = objOrderFulfillment;
            return objMessage;
        }

        public PXAction<SubmitFeedFilter> ViewShipment;
        [PXUIField(DisplayName = SOConstants.shipment, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true, Visible = false)]
        [PXButton(OnClosingPopup = PXSpecialButtonType.Refresh)]
        public virtual IEnumerable viewShipment(PXAdapter adapter)
        {
            if (SubmitFeedList.Current != null)
            {
                SOShipmentEntry graph = PXGraph.CreateInstance<SOShipmentEntry>();
                SOShipment shipment = PXSelect<SOShipment, Where<SOShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>>>.Select(graph, SubmitFeedList.Current.ShipmentNbr);
                graph.Document.Current = shipment;
                PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
            }
            return adapter.Get();
        }
    }

    #region FilterClass
    [Serializable()]
    public partial class SubmitFeedFilter : IBqlTable
    {
        #region IntegrationID

        [PXString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Integration ID")]
        [PXSelector(typeof(Search<SOAmazonSetup.integrationID, Where<SOAmazonSetup.status, Equal<True>, And<SOAmazonSetup.integrationType, Equal<SOConstants.IntegrationType>>>>),
            typeof(SOAmazonSetup.integrationID), typeof(SOAmazonSetup.description), typeof(SOAmazonSetup.integrationType))]
        public virtual string IntegrationID { get; set; }
        public abstract class integrationID : IBqlField { }

        #endregion

        public abstract class tODate : IBqlField
        {
        }
        [PXDate]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "To Date")]
        public virtual DateTime? TODate
        {
            get;
            set;
        }

        #region TotalRecordsToSubmit

        [PXInt]
        [PXUIField(DisplayName = "Total Records To Submit", Enabled = false)]
        public virtual int? TotalRecordsToSubmit { get; set; }
        public abstract class totalRecordsToSubmit : IBqlField { }

        #endregion

        #region TotalSubmittedRecords

        [PXInt]
        [PXUIField(DisplayName = "Total Submitted Records", Enabled = false)]
        public virtual int? TotalSubmittedRecords { get; set; }
        public abstract class totalSubmittedRecords : IBqlField { }

        #endregion

        #region TotalFailedRecords

        [PXInt]
        [PXUIField(DisplayName = "Total Failed Records", Enabled = false)]
        public virtual int? TotalFailedRecords { get; set; }
        public abstract class totalFailedRecords : IBqlField { }

        #endregion
    }
    #endregion

    public class SubmitFeedParamaters
    {
        public SOPartialMaint objPartialMaint { get; set; }
        public int indexOfCurrentOrder { get; set; }
        public SOAmazonSetup objSOAmazonSetup { get; set; }
        public string amazonOrderID { get; set; }
        public string soType { get; set; }
        public string acmOrderNbr { get; set; }
        public string shipmentType { get; set; }
        public string shipmentNbr { get; set; }
        public DateTime? shipmentDate { get; set; }
        public string carrierCode { get; set; }
        public string shipVia { get; set; }
        public string trackingNbr { get; set; }
        public List<Item> liShipItem { get; set; }
        public int? processID { get; set; }
        public bool importOrderStatus { get; set; }
        public string feedMessage { get; set; }
        public string xmlMessage { get; set; }
    }

    #region XmlSchema Structure Declaration

    public class AmazonEnvelope
    {
        public Header Header;
        public string MessageType;
        public Message Message;
        public string MessageBody;

    }
    public class Header
    {
        public string DocumentVersion;
        public string MerchantIdentifier;
    }
    public class Message
    {
        public string MessageID;
        public OrderFulfillment OrderFulfillment;

    }
    public class OrderFulfillment
    {
        public string AmazonOrderID;
        public string FulfillmentDate;
        public FulfillmentData FulfillmentData;
        [XmlElement(SOConstants.Item)]
        public List<Item> Item;
    }
    public class FulfillmentData
    {
        public string CarrierName;
        public string ShippingMethod;
        public string ShipperTrackingNumber;
    }
    public class Item
    {
        public string AmazonOrderItemCode;
        public int Quantity;
    }
    #endregion

    #region Utf8StringWriter
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
    #endregion
}