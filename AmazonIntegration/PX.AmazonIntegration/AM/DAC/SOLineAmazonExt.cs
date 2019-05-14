using PX.Data;
using System;

namespace AmazonIntegration
{
    public class SOLineAmazonExt : PXCacheExtension<PX.Objects.SO.SOLine>
    {
        #region UsrAMOrderItemID

        public abstract class usrAMOrderItemID : IBqlField { }

        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "AM Order Item ID")]
        public virtual string UsrAMOrderItemID { get; set; }

        #endregion

        #region UsrAmazonFreightPrice

        public abstract class usrAmazonFreightAmt : IBqlField { }

        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Amazon Freight Amount")]
        [PXFormula(null, typeof(SumCalc<SOOrderAmazonExt.usrAmazonFreightTotal>))]
        public virtual Decimal? UsrAmazonFreightAmt { get; set; }

        #endregion

        #region UsrAmazonFreightDiscountAmt

        public abstract class usrAmazonFreightDiscountAmt : IBqlField { }

        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Amazon Freight Discount")]
        [PXFormula(null, typeof(SumCalc<SOOrderAmazonExt.usrAmazonFreightDiscountTotal>))]
        public virtual Decimal? UsrAmazonFreightDiscountAmt { get; set; }

        #endregion
    }
}