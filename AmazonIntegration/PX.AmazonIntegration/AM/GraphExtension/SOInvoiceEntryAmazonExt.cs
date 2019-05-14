using PX.Data;
using PX.Objects.SO;
using PX.Objects.TX;

namespace AmazonIntegration
{
    public class SOInvoiceEntryAmazonExt : PXGraphExtension<SOInvoiceEntry>
    {
        public class SOInvoiceTaxCstAttribute : SOInvoiceTaxAttribute
        {
            protected override TaxDetail CalculateTaxSum(PXCache sender, object taxrow, object row)
            {
                TaxRev taxrev = PXResult.Unwrap<TaxRev>(taxrow);
                Tax tax = PXResult.Unwrap<Tax>(taxrow);
                bool propagateCustomRate = false;
                var origTaxRate = taxrev.TaxRate;
                if (taxrev.TaxID != null && tax != null)
                {
                    SOTaxAmazonExt taxExt = tax.GetExtension<SOTaxAmazonExt>();
                    if (taxExt.UsrAMPropagateTaxAmt == true)
                    {
                        SOTaxTran soTax = PXResult<SOTaxTran>.Current;
                        if (soTax != null && taxrev.TaxID == soTax.TaxID && soTax.CuryTaxableAmt.GetValueOrDefault() > 0)
                        {
                            var taxRate = soTax.CuryTaxAmt / soTax.CuryTaxableAmt * 100;
                            if (taxRate != origTaxRate && taxRate > 0)
                            {
                                taxrev.TaxRate = taxRate;
                                propagateCustomRate = true;
                            }
                        }
                    }
                }
                TaxDetail result = base.CalculateTaxSum(sender, taxrow, row);
                if (result != null && propagateCustomRate)
                {
                    result.TaxRate = origTaxRate;
                    taxrev.TaxRate = origTaxRate;
                }
                return result;
            }
        }
        #region Event Handlers

        [PXRemoveBaseAttribute(typeof(SOInvoiceTaxAttribute))]
        [SOInvoiceTaxCst]
        protected void ARTran_TaxCategoryID_CacheAttached(PXCache sender)
        {
        }
        #endregion
    }
}