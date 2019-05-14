using PX.Data;
using PX.Objects.SO;

namespace AmazonIntegration
{
    public class SOPartialMaint : PXGraph<SOPartialMaint>
    {
        #region Select

        public PXSelect<SOOrderLevelProcessLog> OrderLevelProcessLog;
        public PXSelect<SOOrderProcessLog> OrderProcessLog;
        public PXSelect<SOProcessOrder> ProcessOrder;
        public PXSelect<SOImportedRecords> ImportedRecords;
        public PXSelect<SOSubmitProcessLog> SubmitProcesLog;
        public PXSelect<SOSubmitDetailedProcessLog> SubmitDetailedProcesLog;

        public PXSelect<SOOrderProcessLog, Where<SOOrderProcessLog.processID, IsNotNull>,
                   OrderBy<Desc<SOOrderProcessLog.processID>>> UpdateImportProcessLog;
            
        public PXSelect<SOSubmitProcessLog,Where<SOSubmitProcessLog.integrationID, Equal<Required<SOSubmitProcessLog.integrationID>>,
                        And<SOSubmitProcessLog.processID, Equal<Required<SOSubmitProcessLog.processID>>>>>UpdateFeedProcessLog;

        public PXSelect<SOOrder,Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                        And<SOOrderAmazonExt.usrAmazonOrderID, Equal<Required<SOOrderAmazonExt.usrAmazonOrderID>>>>> SOOrderView;

        public PXSelect<SOShipment, Where<SOShipment.shipmentType, Equal<Required<SOShipment.shipmentType>>,
                        And<SOShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>>>> Shipment;

        #endregion
    }
}