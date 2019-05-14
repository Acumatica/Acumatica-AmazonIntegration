using PX.Data;
using PX.Objects.SO;

namespace AmazonIntegration
{
    public class SOShipLineAmazonExt : PXCacheExtension<PX.Objects.SO.SOShipLine>
    {
        #region UsrAmazonOrderID

        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Amazon Order ID")]
        [PXDefault(typeof(Search<SOOrderAmazonExt.usrAmazonOrderID,
                              Where<SOOrder.orderType, Equal<Current<SOShipLine.origOrderType>>,
                              And<SOOrder.orderNbr, Equal<Current<SOShipLine.origOrderNbr>>>>>),
                              PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrAmazonOrderID { get; set; }
        public abstract class usrAmazonOrderID : IBqlField { }

        #endregion

        #region UsrAMOrderItemID

        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "AM Order Item ID")]
        [PXDefault(typeof(Search<SOLineAmazonExt.usrAMOrderItemID,
                              Where<SOLine.orderType, Equal<Current<SOShipLine.origOrderType>>,
                              And<SOLine.orderNbr, Equal<Current<SOShipLine.origOrderNbr>>,
                              And<SOLine.lineNbr, Equal<Current<SOShipLine.origLineNbr>>>>>>),
                              PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrAMOrderItemID { get; set; }
        public abstract class usrAMOrderItemID : IBqlField { }

        #endregion
    }
}
