using PX.Data;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using System.Collections;

namespace AmazonIntegration
{
    public class OrderTypeSelector : PXCustomSelectorAttribute
    {
        public OrderTypeSelector(Type type)
            : base(type, typeof(SOOrderType.orderType), typeof(SOOrderType.descr), typeof(SOOrderType.template), typeof(SOOrderType.behavior))
        {
        }
        public virtual IEnumerable GetRecords()
        {
            SOAmazonSetup currentItem = (SOAmazonSetup)this._Graph.Views["setupview"].Cache.Current;
            if (currentItem == null) return null;
            PXSelectBase<SOOrderType> ordertype = new PXSelectJoin<SOOrderType, InnerJoin<SOOrderTypeOperation, On<SOOrderTypeOperation.orderType,
                Equal<SOOrderType.orderType>, And<SOOrderTypeOperation.operation, Equal<SOOrderType.defaultOperation>>>>>(this._Graph);
            switch (currentItem.IntegrationType.Trim())
            {
                case SOConstants.AMIntegrationType.FBA:
                case SOConstants.AMIntegrationType.CAFBA:
                case SOConstants.AMIntegrationType.UKFBA:
                case SOConstants.AMIntegrationType.JPFBA:
                case SOConstants.AMIntegrationType.DEFBA:
                case SOConstants.AMIntegrationType.INFBA:
                    ordertype.WhereAnd<Where<SOOrderType.template, Equal<SOConstants.InvoiceType>,
                                            And<SOOrderType.aRDocType, Equal<ARDocType.invoice>>>>();
                    return ordertype.Select();
                case SOConstants.AMIntegrationType.FBM:
                    ordertype.WhereAnd<Where<SOOrderType.template, Equal<SOConstants.SalesOrderType>>>();
                    return ordertype.Select();
            }
            return null;
        }
    }

    public class SOLineInventoryItemAmazonExtAttribute : SOLineInventoryItemAttribute
    {
        public static string GetInventoryCode(PXCache sender, string InventoryCode)
        {
            SOLineInventoryItemAmazonExtAttribute attrib = new SOLineInventoryItemAmazonExtAttribute();
            var foundAlternate = attrib.FindAlternate(sender, InventoryCode);
            return foundAlternate?.InventoryCD ?? InventoryCode;
        }
    }
}