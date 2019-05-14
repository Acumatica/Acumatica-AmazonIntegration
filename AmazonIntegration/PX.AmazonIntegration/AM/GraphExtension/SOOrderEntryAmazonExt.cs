using PX.Data;
using PX.Objects.SO;

namespace AmazonIntegration
{
    public class SOOrderEntryAmazonExt : PXGraphExtension<SOOrderEntry>
    {
        #region Event Handlers
        protected virtual void SOOrder_RowSelected(PXCache sender, PXRowSelectedEventArgs e, PXRowSelected BaseEvent)
        {
            SOOrder row = e.Row as SOOrder;
            if (BaseEvent != null)
                BaseEvent(sender, e);

            if (row == null) return;
            SOOrderAmazonExt objSOAmazonOrderext = row.GetExtension<SOOrderAmazonExt>();
            if (objSOAmazonOrderext != null && !string.IsNullOrEmpty(objSOAmazonOrderext.UsrAmazonOrderID))
            {
                PXUIFieldAttribute.SetVisible<SOOrderAmazonExt.usrSyncNote>(sender, null, true);
                PXUIFieldAttribute.SetEnabled<SOOrder.customerRefNbr>(sender, row, false);
            }
        }
        #endregion
    }
}