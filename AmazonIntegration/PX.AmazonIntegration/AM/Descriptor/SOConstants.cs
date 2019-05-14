using PX.Data;
using PX.Objects.SO;

namespace AmazonIntegration
{
    public class SOConstants
    {
        # region Constants
        public const string serviceUrlNA = "https://mws.amazonservices.com";
        public const string serviceUrlEU = "https://mws-eu.amazonservices.com";
        public const string serviceUrlIN = "https://mws.amazonservices.in";
        public const string serviceUrlJP = "https://mws.amazonservices.jp";
        public const string ordersURL = "https://mws.amazonservices.com/Orders/2013-09-01";

        public const string usDomain = "amazon.com";
        public const string caDomain = "amazon.ca";
        public const string ukDomain = "amazon.uk";
        public const string jpDomain = "amazon.jp";
        public const string deDomain = "amazon.de";
        public const string inDomain = "amazon.in";

        public const string fulfillmentChannelAFN = "AFN";
        public const string fulfillmentChannelMFN = "MFN";
        public const string FBM = "FBM";
        public const string FBA = "FBA";

        public const string shipped = "Shipped";
        public const string unshipped = "Unshipped";
        public const string partiallyShipped = "PartiallyShipped";

        public const string appName = "Acumatica";
        public const string version = "1.0";
        public const string ScreenID = "SO301000";

        public const string serviceUrl = "ServiceURL";
        public const string domainName = "DomainName";
        public const string invokeListOrders = "invokeListOrders";
        public const string invokeListOrdersByNextToken = "invokeListOrdersByNextToken";
        public const string invokeGetOrder = "invokeGetOrder";
        public const string invokeListOrderItems = "invokeListOrderItems";
        public const string btnImport = "Import";
        public const string btnImportAll = "Import ALL";
        public const string btnPrepareAndImport = "Prepare & Import";
        public const string btnPrepare = "Prepare";
        public const string FieldMapping = "FieldMapping";

        // Throttle Constants
        public const string apiListOrders = "ListOrders";
        public const string apiListOrdersByNextToken = "ListOrdersByNextToken";
        public const string apiGetOrder = "GetOrder";
        public const string apiGetListOrderItems = "ListOrderItems";
        public const string apiSubmitFeed = "SubmitFeed";
        public const string apiGetFeedSubmissionResult = "GetFeedSubmissionResult";
        public const string apiGetRequestReportList = "GetReportRequestList";
        public const string apiRequestReport = "RequestReport";
        public const string apiGetReport = "GetReport";
        public const string getGeneratedReportID = "GetGeneratedReportID";
        public const string apiFeedResultWaiting = "FeedResultWaiting";

        public const string feedProcessingResultNotReady = "feedprocessingresultnotready";
        public const string delayTImeNotFound = "Delay time not found for Throttle Management. Please Review";
        public const string xmlExtenstion = ".xml";
        public const string btnSubmitFeed = "Submit Feed";
        public const string btnSubmitFeedALL = "Submit Feed ALL";
        public const string space = " ";
        public const string inventoryID = "InventoryID";
        public const string amazonFulfilledShipmentData = "_GET_AMAZON_FULFILLED_SHIPMENTS_DATA_";
        public const string amazonOrderidColumn = "amazon-order-id";
        public const string trackingNumberColumn = "tracking-number";
        public const string carrierColumn = "carrier";
        public const string importorders = "importorders";
        public const string scheduleimportorders = "scheduleimportorders";
        public const string orderTag = "Order";
        public const string orderItemTag = "OrderItem";
        public const string customerId = "customerid";
        public const string ordertype = "ordertype";
        public const string orderItem = "orderitem";
        public const string order = "order";
        public const string childNodes = "ChildNodes";
        public const string count = "Count";
        public const string itemPrice = "itemprice";
        public const string itemTax = "itemtax";
        public const string promotionDiscount = "promotiondiscount";
        public const string ShippingPrice = "shippingprice";
        public const string shippingDiscount = "shippingdiscount";
        public const string shippingTax = "shippingtax";
        public const string giftWrapPrice = "giftwrapprice";
        public const string giftWrapTax = "giftwraptax";
        public const string pointsGranted = "pointsgranted";
        public const string codFee = "codfee";
        public const string codFeeDiscount = "codfeeDiscount";
        public const string invoiceData = "invoicedata";
        public const string buyerCustomizedInfo = "buyercustomizedinfo";
        public const string shippingAddress = "shippingaddress";
        public const string orderTotal = "ordertotal";
        public const string paymentExecutiondetailItem = "paymentexecutiondetailitem";
        public const string documentVersion = "1.01";
        public const string orderFulfillment = "OrderFulfillment";
        public const string xsi = "xsi";
        public const string xmlSchema = "http://www.w3.org/2001/XMLSchema-instance";
        public const string noNamespaceSchemaLocation = "noNamespaceSchemaLocation";
        public const string amznEnvelope = "amzn-envelope.xsd";
        public const string feedTypePostOrderFulfillment = "_POST_ORDER_FULFILLMENT_DATA_";
        public const string Submitted = "SUBMITTED";
        public const string feedSubmitted = "Feed Submitted Successfully";
        public const string credentialsinvalid = "Credentials are not recognized";
        public const string exceptionIsEmpty = "Exception is empty.";
        public const string Amount = "amount";

        public const string messagebody = "Messagebody";
        public const string amazonEnvelope = "AmazonEnvelope";
        public const string messageBody = "MessageBody";
        public const string msgbodyTag = "<MessageBody>Messagebody</MessageBody>";
        public const string getNewLine = "\n";
        public const string statusDone = "_DONE_";
        public const string statusInProgress = "_IN_PROGRESS_";
        public const string statusSubmitted = "_SUBMITTED_";
        public const double feedSize = 9.5;
        public const string timeFormat = "s";
        public const int limitForReports = 30;
        public const string shipment = "Shipment";
        public const string Result = "Result";
        public const string btnClearLog = "Clear Log";
        public const string btnClearAllLog = "Clear All Log";
        public const string Item = "Item";
        public const string FBMType = "FBM  ";      

        public class AMSyncNote : Constant<string>
        {
            public AMSyncNote() : base(SOMessages.syncNote) { }
        }

        #endregion

        #region Bql constants
        public class InvoiceType : Constant<string>
        {
            public InvoiceType() : base("IN") { }
        }
        public class SalesOrderType : Constant<string>
        {
            public SalesOrderType() : base("SO") { }
        }
        public class IntegrationType : Constant<string>
        {
            public IntegrationType() : base(AMIntegrationType.FBM) { }
        }

        public class FBMMessage : Constant<string>
        {
            public FBMMessage() : base(SOMessages.fbmNote) { }
        }

        public class InitialFromDate : Constant<string>
        {
            public InitialFromDate() : base(SOMessages.InitialFromDateNote) { }
        }
        public class FBMIntegrationType : Constant<string>
        {
            public FBMIntegrationType() : base(SOConstants.FBMType) { }
        }        

        #endregion

        #region Integration Type Constants

        public class AMIntegrationType
        {
            public class ListAttribute : PXStringListAttribute
            {
                public ListAttribute()
                    : base(
                        new string[] { FBA, CAFBA, UKFBA, DEFBA, JPFBA, INFBA, FBM },
                        new string[] { SOMessages.fBA, SOMessages.caFBA, SOMessages.ukFBA, SOMessages.deFBA, SOMessages.jpFBA, SOMessages.inFBA, SOMessages.fBM })
                { }
            }
            public const string FBA = "FBA";
            public const string CAFBA = "CAFBA";
            public const string UKFBA = "UKFBA";
            public const string DEFBA = "DEFBA";
            public const string JPFBA = "JPFBA";
            public const string INFBA = "INFBA";
            public const string FBM = "FBM";

            public class fBA : Constant<string>
            {
                public fBA() : base(FBA) { }
            }
            public class caFBA : Constant<string>
            {
                public caFBA() : base(CAFBA) { }
            }
            public class ukFBA : Constant<string>
            {
                public ukFBA() : base(UKFBA) { }
            }
            public class dEFBA : Constant<string>
            {
                public dEFBA() : base(DEFBA) { }
            }
            public class jPFBA : Constant<string>
            {
                public jPFBA() : base(JPFBA) { }
            }
            public class iNFBA : Constant<string>
            {
                public iNFBA() : base(INFBA) { }
            }
            public class fBM : Constant<string>
            {
                public fBM() : base(FBM) { }
            }
        }
        public class SalesOrderStatusCustom
        {
            public class ListAttribute : PXStringListAttribute
            {
                public ListAttribute() : base(
                    new string[] { SOOrderStatus.BackOrder, SOOrderStatus.Cancelled, SOOrderStatus.Completed, SOOrderStatus.CreditHold, SOOrderStatus.Invoiced, SOOrderStatus.Open, SOOrderStatus.Shipping },
                    new string[] { Messages.BackOrder, Messages.Cancelled, Messages.Completed, Messages.CreditHold, Messages.Invoiced, Messages.Open, Messages.Shipping })
                { }
            }
        }
        #endregion
    }
}