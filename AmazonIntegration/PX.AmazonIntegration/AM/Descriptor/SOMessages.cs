using PX.Common;

namespace AmazonIntegration
{
    [PXLocalizable(prefix)]
    public class SOMessages
    {
        #region Validation and Processing Messages
        public const string prefix = "Amazon Integration Errors";
        #region DAC Cache Names
        public const string marketplaceConfiguration = "Marketplace Configuration";
        public const string fieldMapping = "Field Mapping";
        public const string importedRecords = "Imported Records";
        public const string processLog = "Import Process Log";
        public const string importlog = "Import Log";
        public const string prepareAndImport = "Prepare and Import";
        public const string processOrder = "Process Order";
        public const string schemafiledetails = "Schema File Details";
        public const string feedProcessLog = "Feed Process Log";
        public const string throttleConstants = "Throttle Constants";
        #endregion

        #region Integration Type List
        public const string fBA = "Amazon FBA";
        public const string caFBA = "Amazon.ca FBA";
        public const string ukFBA = "Amazon.co.uk FBA";
        public const string deFBA = "Amazon.de FBA";
        public const string jpFBA = "Amazon.jp FBA";
        public const string inFBA = "Amazon.in FBA";
        public const string fBM = "Amazon FBM";
        #endregion

        #region Configuration Messages
        public const string testConnection = "Test Connection";
        public const string connectionSuccess = "Connection Success.";
        public const string connectionFailed = "Connection Failed.";
        public const string errorMsg = "MarketplaceID does not exist for this Integration Type in our records.";
        public const string getSchema = "Get Schema";
        public const string warningMsg = "Schema already exists. Do you really want to update the schema? ";
        public const string AmwOrderDoesnotExist = "Amazon Order ID does not exist for this Seller ID. Please Review.";
        public const string AmwOrderCannotbeEmpty = "Amazon Order ID cannot be empty. Please Review.";
        public const string DateValidation = "Initial From Date cannot be greater than Business Date. Please Review.";
        #endregion

        #region SO Sync Messages
        public const string prepareRecords = "Prepare";
        public const string validationIntegrationIdandprocessall = "Either Integration ID or Process All to be selected. Please Review.";
        public const string validateIntegrationId = "Integration ID cannot be empty.";
        public const string validateToDate = "To Date cannot be empty. Please Review.";
        public const string validateFilter = " Integration ID and To Date  cannot be empty.";
        public const string validationFromandTodate = "To Date must be greater than From Date. Please Review. ";
        public const string validationTodateandBusinessDate = "To Date cannot be greater than Business Date. Please Review.";
        public const string schemaDoesnotExist = "Schema does not exist for this Order.";
        public const string syncNote = "This is an order imported from Amazon. Please note that any changes to this order may have inadvertent issues and will not be reflected back to Amazon. ";
        public const string InitialFromDateNote = "Baseline Date beyond which Orders will be Synced from Amazon into Acumatica. Please note that Initial From Date field cannot be changed as soon as an order is imported into Acumatica";
        public const string InvMappingErrorMsg = "InventoryID is not mapped in FieldMapping Configuration. Please Review.";
        #endregion

        #region LogMessages

        public const string sucess = "Order successfully imported";
        public const string ErrorOrderTotalNotMatch = "Amazon order amount doesn't match.";
        public const string logRecordDeleted = "Log record is deleted successfully.";
        public const string showErrorMsgOrders = "One or more Orders are not processed";
        public const string showErrorMsgFeeds = "One or more Feeds are not processed";

        #endregion

        #region Prepare and Import

        public const string msgProcessError = " We cannot process the Integration ID and Process All at a time. Please Review.";
        public const string requestThrottled = "requestthrottled";

        #endregion

        #region CreateSo

        public const string fbaOrder = "Amazon FBA Order";
        public const string fbmOrder = "Amazon FBM Order";
        public const string apidetailsMissing = "MarketPlace Configuration details are missing. Please Review.";
        public const string configMissing = "Sales Order Preferences Configuration is not set. Please Review.";
        public const string FieldMappingEmptyErrMsg = "User Field Mapping is empty.";
        public const string recordAlreadyImported = "This Order is already imported. Please Review";
        public const string guestCustomerErrorMsg = "Guest Customer is not configured at Sales Order Preferences. Please Review.";
        public const string inventoryItemNotExists = "Inventory Item not exist in the System.";
        public const string inputStringWas = "input string was ";
        public const string notRecognized = "Not recognized.";
        public const string chkFieldMapping = "Please check the field mapping for: ";
        public const string dataTypeNotMatching = " Data type not matching.";
        public const string inventoryMappedtoOrderItem = "Inventory must be mapped to Order Item. Please Review.";
        public const string fbmNote = "Orders with Completed and Backorder statuses only will be considered for submit feeds.";

        #endregion

        #region SubmitFeed
        public const string shipmentDetailsMissing = "Shipment/Package details are not available. Please Review";
        #endregion

        #region Tracking Info Messages
                
        public const string errorReportInfo = "Unable to retrieve the tracking information. Please try again.";

        public const string ReportDateValidation = "Date difference should not exceed more than 30 days.Please Review.";

        #endregion
        #endregion
    }
}