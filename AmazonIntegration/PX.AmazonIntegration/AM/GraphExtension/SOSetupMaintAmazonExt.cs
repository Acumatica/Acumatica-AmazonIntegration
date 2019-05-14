using PX.Data;
using PX.Objects.SO;
using System;

namespace AmazonIntegration
{
    public class SOSetupMaintAmazonExt : PXGraphExtension<SOSetupMaint>
    {
        #region events
        protected virtual void SOSetup_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            SOSetup row = e.Row as SOSetup;
            if (row != null)
            {
                SOSetupAmazonExt objSOSetupExt = row.GetExtension<SOSetupAmazonExt>();
                PXUIFieldAttribute.SetVisible<SOSetupAmazonExt.usrInitialFromDateNote>(sender, row, objSOSetupExt != null && objSOSetupExt.UsrAmazonInitialFromDate != null);
                SOProcessOrder objSOOrder = PXSelect<SOProcessOrder>.Select(Base);
                if (objSOOrder != null)
                    PXUIFieldAttribute.SetEnabled<SOSetupAmazonExt.usrAmazonInitialFromDate>(sender, row, false);
                else
                    PXUIFieldAttribute.SetEnabled<SOSetupAmazonExt.usrAmazonInitialFromDate>(sender, row, true);
            }
        }

        protected virtual void SOSetup_UsrAmazonInitialFromDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            SOSetup row = e.Row as SOSetup;
            if (row == null) return;
            SOSetupAmazonExt objSOSetupExt = row.GetExtension<SOSetupAmazonExt>();
          if (objSOSetupExt != null && e.NewValue != null && Convert.ToDateTime(e.NewValue) > Base.Accessinfo.BusinessDate)
                sender.RaiseExceptionHandling<SOSetupAmazonExt.usrAmazonInitialFromDate>(e.Row, null, new PXSetPropertyException(SOMessages.DateValidation));
        }

        #endregion
    }
}