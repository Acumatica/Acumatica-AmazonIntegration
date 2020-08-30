using PX.Data;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.Objects.TX;
using System;

namespace AmazonIntegration
{
    public sealed class SOSetupAmazonExt : PXCacheExtension<PX.Objects.SO.SOSetup>
    {     
        #region UsrAmazonTaxID

        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax ID")]
        [PXSelector(typeof(Search<Tax.taxID, Where<SOTaxAmazonExt.usrAMPropagateTaxAmt, Equal<True>>>), DescriptionField = typeof(Tax.descr))]
        public string UsrAmazonTaxID { get; set; }
        public abstract class usrAmazonTaxID : IBqlField { }

        #endregion

        #region UsrAmazonPaymentMethodID

        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Payment Method ID")]
        [PXSelector(typeof(Search<PaymentMethod.paymentMethodID, Where<PaymentMethod.isActive, Equal<True>,
                   And<PaymentMethod.useForAR, Equal<True>>>>), DescriptionField = typeof(PaymentMethod.descr))]
        public string UsrAmazonPaymentMethodID { get; set; }
        public abstract class usrAmazonPaymentMethodID : IBqlField { }

        #endregion

        #region UsrAmazonTaxZoneID

        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Zone ID")]
        [PXSelector(typeof(Search3<TaxZone.taxZoneID, OrderBy<Asc<TaxZone.taxZoneID>>>))]
        public string UsrAmazonTaxZoneID { get; set; }
        public abstract class usrAmazonTaxZoneID : IBqlField { }

        #endregion

        #region UsrAmazonShipVia

        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Ship Via")]
        [PXSelector(typeof(Search<Carrier.carrierID>))]
        public string UsrAmazonShipVia { get; set; }
        public abstract class usrAmazonShipVia : IBqlField { }

        #endregion

        #region UsrAmazonInitiazlFromDate

        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Initial From date", Required = true)]
        public DateTime? UsrAmazonInitialFromDate { get; set; }
        public abstract class usrAmazonInitialFromDate : IBqlField { }

        #endregion

        #region UsrInitialFromDateNote

        [PXString(200, IsUnicode = true)]
        [PXUIField(DisplayName = "Note", IsReadOnly = true)]
        [PXFormula(typeof(Switch<Case<Where<SOSetupAmazonExt.usrAmazonInitialFromDate, IsNotNull>, SOConstants.InitialFromDate>, StringEmpty>))]
        public string UsrInitialFromDateNote { get; set; }
        public abstract class usrInitialFromDateNote : IBqlField { }

        #endregion
    }
}