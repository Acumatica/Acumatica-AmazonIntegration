using PX.Data;
using System;

namespace AmazonIntegration
{
    public class SOOrderAmazonExt : PXCacheExtension<PX.Objects.SO.SOOrder>
    {
        #region UsrAmazonOrderID

        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Amazon Order ID")]
        public virtual string UsrAmazonOrderID { get; set; }
        public abstract class usrAmazonOrderID : IBqlField { }

        #endregion

        #region UsrSyncNote

        [PXString(200, IsUnicode = true)]
        [PXUIField(DisplayName = "Note", IsReadOnly = true, Visible = false)]
        [PXFormula(typeof(Switch<Case<Where<SOOrderAmazonExt.usrAmazonOrderID, IsNotNull>, SOConstants.AMSyncNote>, StringEmpty>))]
        public virtual string UsrSyncNote { get; set; }
        public abstract class usrSyncNote : IBqlField { }

        #endregion

        #region UsrAmazonFreightTotal

        public abstract class usrAmazonFreightTotal : IBqlField { }

        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Amazon Freight Amount", IsReadOnly = true)]
        public virtual Decimal? UsrAmazonFreightTotal { get; set; }

        #endregion

        #region UsrAmazonFreightDiscountTotal

        public abstract class usrAmazonFreightDiscountTotal : IBqlField { }

        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Amazon Freight Discount", IsReadOnly = true)]
        public virtual Decimal? UsrAmazonFreightDiscountTotal { get; set; }

        #endregion
    }
}