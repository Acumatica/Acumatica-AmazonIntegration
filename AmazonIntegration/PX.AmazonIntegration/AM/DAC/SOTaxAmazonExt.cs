using PX.Data;

namespace AmazonIntegration
{
    public class SOTaxAmazonExt : PXCacheExtension<PX.Objects.TX.Tax>
    {
        #region UsrAMPropagateTaxAmt

        [PXDBBool]
        [PXUIField(DisplayName = "Propagate Manually Set Tax Amount from Sales Orders to Invoices")]
        public virtual bool? UsrAMPropagateTaxAmt { get; set; }
        public abstract class usrAMPropagateTaxAmt : IBqlField { }

        #endregion
    }
}