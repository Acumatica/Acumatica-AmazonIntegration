using PX.Data;

namespace AmazonIntegration
{
    public class SOShipmentAmazonExt : PXCacheExtension<PX.Objects.SO.SOShipment>
    {
        #region UsrSubmitFeedupdate

        [PXDBBool]
        [PXUIField(DisplayName = "Submit Feed update")]
        public virtual bool? UsrSubmitFeedupdate { get; set; }
        public abstract class usrSubmitFeedupdate : IBqlField { }

        #endregion

        #region UsrAmazonCarrierCode
        public abstract class usrAmazonCarrierCode : IBqlField { }

        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Amazon Carrier Code")]
        public virtual string UsrAmazonCarrierCode { get; set; }
        #endregion
    }
}
