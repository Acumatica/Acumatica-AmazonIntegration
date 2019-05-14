using System;
using PX.Data;
using System.Collections.Generic;
using PX.Objects.SO;
using MarketplaceWebServiceOrders.Model;
using System.Reflection;
using System.Linq;
using PX.Objects.IN;
using PX.Objects.AR;
using System.Text.RegularExpressions;
using PX.Objects.TX;

namespace AmazonIntegration
{
    #region Importing Orders Methods

    public class CreateSO
    {
        private decimal taxtotal = decimal.Zero;
        private decimal taxableamount = decimal.Zero;
        private decimal taxRate = decimal.Zero;
        private decimal? orderTotal = decimal.Zero;
        private decimal shippingprice = decimal.Zero;

        public static void CreateSalesOrderandPayments(PrepareAndImportOrdersParams objParams)
        {
            try
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    if (objParams.objliUsrMapping.Where(X => X.IntegrationID == objParams.objSOProcessOrderRecord.IntegrationID).Count() == 0)
                    {
                        objParams.objliUsrMapping.Clear();
                        objParams.objliUsrMapping = SOHelper.GetUsrFieldMapping(objParams.objSOOrderEntry, objParams.objSOProcessOrderRecord.IntegrationID);
                    }
                    if (objParams.objliUsrMapping.Count > 0)
                    {
                        CreateSO objCreateSO = new CreateSO();
                        objCreateSO.SetSOHeaderObjectsData(objParams);
                        objCreateSO.SetSOLineObjectsData(objParams);
                        objCreateSO.SetBillingContactObjectsData(objParams);
                        objCreateSO.SetBillingAddressobjectsData(objParams);
                        objCreateSO.SetFinancialInformationobjectsData(objParams);
                        objCreateSO.SetShippingContactObjectsData(objParams);
                        objCreateSO.SetShippingAddressObjectsData(objParams);
                        objCreateSO.SetFreightData(objParams);
                        objCreateSO.SetDocumentLevelDiscountandTaxData(objParams);

                        //Validate order total - 
                        decimal dAmazonTotal = 0;
                        Decimal.TryParse(objParams.ObjCurrentOrder.OrderTotal.Amount, out dAmazonTotal);
                        if (dAmazonTotal !=
                            objParams.objSOOrderEntry.Document.Current.CuryOrderTotal.GetValueOrDefault(0))
                            throw new PXException(SOMessages.ErrorOrderTotalNotMatch);

                        objParams.objSOOrderEntry.Actions.PressSave();
                        
                        objCreateSO.CreatePaymentProcess(objParams);
                        if (objParams.objSOOrderEntry.Document.Current != null && !string.IsNullOrEmpty(objParams.objSOOrderEntry.Document.Current.OrderNbr))
                        {
                            ts.Complete();
                            SOLogService.LogImportStatus(objParams, true, SOMessages.sucess);
                            PXProcessing<SOProcessOrder>.SetInfo(objParams.CurrentOrderIndex, SOMessages.sucess);
                        }
                    }
                    else
                        throw new PXException(SOMessages.FieldMappingEmptyErrMsg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetSOHeaderObjectsData(PrepareAndImportOrdersParams orderParams)
        {
            SOOrder newOrder = new SOOrder();
            if (orderParams.objSOOrderEntry.sosetup.Current != null)
            {
                SOSetupAmazonExt objSOOSetupext = orderParams.objSOOrderEntry.sosetup.Current.GetExtension<SOSetupAmazonExt>();
                if (objSOOSetupext != null && objSOOSetupext.UsrGuestCustID.HasValue)
                {
                    newOrder.OrderType = orderParams.objSOAmazonSetup.OrderType;
                    newOrder = (SOOrder)orderParams.objSOOrderEntry.Document.Cache.Insert(newOrder);
                    newOrder.CustomerID = objSOOSetupext.UsrGuestCustID;
                }
                else
                    throw new PXException(SOMessages.guestCustomerErrorMsg);
            }
            AssignValueAsPerIntegrationMapping(orderParams.objliUsrMapping, newOrder, orderParams.ObjCurrentOrder, null, orderParams.objSOOrderEntry.Document.View.Name);
            newOrder.OrderDesc = !string.IsNullOrEmpty(orderParams.ObjCurrentOrder.FulfillmentChannel) ?
                                  orderParams.ObjCurrentOrder.FulfillmentChannel == SOConstants.fulfillmentChannelAFN ?
                                  string.IsNullOrEmpty(newOrder.OrderDesc) ? SOMessages.fbaOrder : newOrder.OrderDesc + " - " + SOMessages.fbaOrder :
                                  orderParams.ObjCurrentOrder.FulfillmentChannel == SOConstants.fulfillmentChannelMFN ?
                                  string.IsNullOrEmpty(newOrder.OrderDesc) ? SOMessages.fbmOrder : newOrder.OrderDesc + " - " + SOMessages.fbmOrder :
                                  newOrder.OrderDesc : newOrder.OrderDesc;
            newOrder.DefaultSiteID = orderParams.objSOAmazonSetup.DfltWarehouseID;
            SOOrderAmazonExt newOrderExt = newOrder.GetExtension<SOOrderAmazonExt>();
            if (newOrderExt != null)
            {
                newOrderExt.UsrAmazonOrderID = orderParams.ObjCurrentOrder.AmazonOrderId;
            }
            orderParams.objSOOrderEntry.Document.Current = newOrder;
            orderParams.objSOOrderEntry.Document.Update(orderParams.objSOOrderEntry.Document.Current);
        }

        private void SetSOLineObjectsData(PrepareAndImportOrdersParams orderParams)
        {
            IEnumerable<SOFieldMapping> objMapping = orderParams.objliUsrMapping.Where(x => x.TargetObject == orderParams.objSOOrderEntry.Transactions.View.Name &&
                                                                                        x.TargetField.ToLower() == SOConstants.inventoryID.ToLower());
            if (objMapping.ToList().Count > 0)
            {
                foreach (OrderItem currentitem in orderParams.objamwLineItems)
                {
                    SOLine newitems = (SOLine)orderParams.objSOOrderEntry.Transactions.Cache.Insert();
                    foreach (SOFieldMapping data in orderParams.objliUsrMapping.Where(x => x.TargetObject == orderParams.objSOOrderEntry.Transactions.View.Name &&
                                                                                           x.TargetField.ToLower() == SOConstants.inventoryID.ToLower()))
                    {
                        if (data.SourceObject.ToLower().Trim() == SOConstants.orderItemTag.ToLower())
                        {
                            string fieldValue = currentitem != null ? Convert.ToString(currentitem.GetType().GetProperty(data.SourceField).GetValue(currentitem, null)) : string.Empty;

                            string itemCode = SOLineInventoryItemAmazonExtAttribute.GetInventoryCode(orderParams.objSOOrderEntry.Transactions.Cache, fieldValue);

                            InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>,
                                                            And<InventoryItem.itemStatus, Equal<InventoryItemStatus.active>>>>.Select(orderParams.objSOOrderEntry, itemCode);
                            newitems.InventoryID = item != null && item.InventoryID.HasValue ? item.InventoryID : null;
                            orderParams.objSOOrderEntry.Transactions.Update(newitems);
                            if (newitems.InventoryID.HasValue)
                            {
                                AssignValueAsPerIntegrationMapping(orderParams.objliUsrMapping, newitems, orderParams.ObjCurrentOrder, currentitem, orderParams.objSOOrderEntry.Transactions.View.Name);
                                //Embedd GiftPrice
                                string sGiftWrappPrice = currentitem.GiftWrapPrice != null ? Convert.ToString(currentitem.GiftWrapPrice.Amount) : string.Empty;
                                newitems.CuryUnitPrice += String.IsNullOrEmpty(sGiftWrappPrice) ? 0m : Convert.ToDecimal(sGiftWrappPrice);

                                newitems.CuryUnitPrice = newitems.CuryUnitPrice > 0 && currentitem.QuantityOrdered > 0 ? newitems.CuryUnitPrice / currentitem.QuantityOrdered : newitems.CuryUnitPrice;
                                newitems.SiteID = orderParams.objSOAmazonSetup.DfltWarehouseID;
                                SOSetupAmazonExt objSOOSetupext = orderParams.objSOOrderEntry.sosetup.Current.GetExtension<SOSetupAmazonExt>();
                                if (objSOOSetupext != null && !string.IsNullOrEmpty(objSOOSetupext.UsrAmazonTaxID))
                                {
                                    TaxCategoryDet objTaxDetails = PXSelect<TaxCategoryDet, 
                                                                                      Where<TaxCategoryDet.taxID, Equal<Required<Tax.taxID>>>>
                                                                                   .Select(orderParams.objSOOrderEntry, objSOOSetupext.UsrAmazonTaxID);
                                    if (objTaxDetails != null)
                                        newitems.TaxCategoryID = objTaxDetails.TaxCategoryID;
                                    SOLineAmazonExt objnewitemsext = newitems.GetExtension<SOLineAmazonExt>();
                                    if (objnewitemsext != null)
                                        objnewitemsext.UsrAMOrderItemID = currentitem.OrderItemId;
                                    orderParams.objSOOrderEntry.Transactions.Update(newitems);
                                    FillDiscountObjectsData(orderParams.objSOOrderEntry, newitems, currentitem);

                                    taxtotal += currentitem.GiftWrapTax != null && !string.IsNullOrEmpty(currentitem.GiftWrapTax.Amount) && Convert.ToDecimal(currentitem.GiftWrapTax.Amount) > 0
                                             ? Convert.ToDecimal(currentitem.GiftWrapTax.Amount) : 0;

                                    taxtotal += currentitem.ItemTax != null && !string.IsNullOrEmpty(currentitem.ItemTax.Amount) && Convert.ToDecimal(currentitem.ItemTax.Amount) > 0
                                                 ? Convert.ToDecimal(currentitem.ItemTax.Amount) : 0;
                                    taxtotal += currentitem.ShippingTax != null && !string.IsNullOrEmpty(currentitem.ShippingTax.Amount) && Convert.ToDecimal(currentitem.ShippingTax.Amount) > 0
                                                ? Convert.ToDecimal(currentitem.ShippingTax.Amount) : 0;
                                    taxableamount += currentitem.ItemPrice != null && !string.IsNullOrEmpty(currentitem.ItemPrice.Amount) && Convert.ToDecimal(currentitem.ItemPrice.Amount) > 0
                                                    ? Convert.ToDecimal(currentitem.ItemPrice.Amount) : 0;
                                    shippingprice += currentitem.ShippingPrice != null && !string.IsNullOrEmpty(currentitem.ShippingPrice.Amount) && Convert.ToDecimal(currentitem.ShippingPrice.Amount) > 0
                                                    ? Convert.ToDecimal(currentitem.ShippingPrice.Amount) : 0;
                                    taxRate = (taxableamount > 0) ? (taxtotal * 100) / taxableamount : 0;
                                }
                                else
                                    throw new PXException(SOMessages.configMissing);
                            }
                            else
                                throw new PXException(SOMessages.inventoryItemNotExists);
                        }
                        else
                            throw new PXException(SOMessages.inventoryMappedtoOrderItem);
                    }
                }
            }
            else           
                throw new PXException(SOMessages.InvMappingErrorMsg);
        }

        private void SetBillingContactObjectsData(PrepareAndImportOrdersParams ordersParams)
        {
            ordersParams.objSOOrderEntry.Billing_Contact.Current = (SOBillingContact)ordersParams.objSOOrderEntry.Billing_Contact.Select();
            if (ordersParams.objSOOrderEntry.Billing_Contact.Current == null) return;
            if (ordersParams.objliUsrMapping.Where(x => (x.TargetObject == ordersParams.objSOOrderEntry.Billing_Contact.View.Name)).Count() > 0)
            {
                ordersParams.objSOOrderEntry.Billing_Contact.Current.OverrideContact = true;
                ordersParams.objSOOrderEntry.Billing_Contact.Update(ordersParams.objSOOrderEntry.Billing_Contact.Current);
                AssignValueAsPerIntegrationMapping(ordersParams.objliUsrMapping, ordersParams.objSOOrderEntry.Billing_Contact.Current, ordersParams.ObjCurrentOrder, null, ordersParams.objSOOrderEntry.Billing_Contact.View.Name);
                ordersParams.objSOOrderEntry.Billing_Contact.Update(ordersParams.objSOOrderEntry.Billing_Contact.Current);
            }
        }

        private void SetBillingAddressobjectsData(PrepareAndImportOrdersParams orderParams)
        {
            orderParams.objSOOrderEntry.Billing_Address.Current = (SOBillingAddress)orderParams.objSOOrderEntry.Billing_Address.Select();
            if (orderParams.objSOOrderEntry.Billing_Address.Current == null) return;
            if (orderParams.objliUsrMapping.Where(x => (x.TargetObject == orderParams.objSOOrderEntry.Billing_Address.View.Name)).Count() > 0)
            {
                orderParams.objSOOrderEntry.Billing_Address.Current.OverrideAddress = true;
                orderParams.objSOOrderEntry.Billing_Address.Update(orderParams.objSOOrderEntry.Billing_Address.Current);
                AssignValueAsPerIntegrationMapping(orderParams.objliUsrMapping, orderParams.objSOOrderEntry.Billing_Address.Current, orderParams.ObjCurrentOrder, null, orderParams.objSOOrderEntry.Billing_Address.View.Name);
                if (orderParams.objSOOrderEntry.Billing_Address.Current.State != null)
                {
                    orderParams.objSOOrderEntry.Billing_Address.Current.State = SOHelper.GetStateID(orderParams.objSOOrderEntry, orderParams.objSOOrderEntry.Billing_Address.Current.State, orderParams.objSOOrderEntry.Billing_Address.Current.CountryID);
                }
                orderParams.objSOOrderEntry.Billing_Address.Update(orderParams.objSOOrderEntry.Billing_Address.Current);
            }
        }

        private void SetFinancialInformationobjectsData(PrepareAndImportOrdersParams orderParams)
        {
            SOOrder financialInformation = (SOOrder)orderParams.objSOOrderEntry.Document.Current;
            if (financialInformation == null) return;
            if (orderParams.objSOOrderEntry.sosetup.Current != null)
            {
                bool isCarrierAvaliable = false;
                SOSetupAmazonExt objSOOSetupext = orderParams.objSOOrderEntry.sosetup.Current.GetExtension<SOSetupAmazonExt>();
                if (orderParams != null && orderParams.objSOProcessOrderRecord != null && (orderParams.objSOProcessOrderRecord.Carrier != null || !string.IsNullOrEmpty(orderParams.objSOProcessOrderRecord.Carrier)))
                    isCarrierAvaliable = orderParams.listOfCarriers.Any(x => x.ToLower().Trim() == orderParams.objSOProcessOrderRecord.Carrier.ToLower().Trim());
                if (isCarrierAvaliable)
                    financialInformation.ShipVia = orderParams.objSOProcessOrderRecord.Carrier;
                else if (objSOOSetupext != null && !string.IsNullOrEmpty(objSOOSetupext.UsrAmazonShipVia))
                    financialInformation.ShipVia = objSOOSetupext.UsrAmazonShipVia;
                orderParams.objSOOrderEntry.Document.Update(financialInformation);
            }
            AssignValueAsPerIntegrationMapping(orderParams.objliUsrMapping, financialInformation, orderParams.ObjCurrentOrder, null,  /*SOConstants.currentDocument*/ orderParams.objSOOrderEntry.CurrentDocument.View.Name);
            orderParams.objSOOrderEntry.Document.Update(financialInformation);
        }

        private void SetShippingContactObjectsData(PrepareAndImportOrdersParams orderParams)
        {
            orderParams.objSOOrderEntry.Shipping_Contact.Current = (SOShippingContact)orderParams.objSOOrderEntry.Shipping_Contact.Select();
            if (orderParams.objSOOrderEntry.Shipping_Contact.Current != null)
            {
                if (orderParams.objliUsrMapping.Where(x => (x.TargetObject == orderParams.objSOOrderEntry.Shipping_Contact.View.Name)).Count() > 0)
                {
                    orderParams.objSOOrderEntry.Shipping_Contact.Current.OverrideContact = true;
                    orderParams.objSOOrderEntry.Shipping_Contact.Update(orderParams.objSOOrderEntry.Shipping_Contact.Current);
                    AssignValueAsPerIntegrationMapping(orderParams.objliUsrMapping, orderParams.objSOOrderEntry.Shipping_Contact.Current, orderParams.ObjCurrentOrder, null, orderParams.objSOOrderEntry.Shipping_Contact.View.Name);
                    orderParams.objSOOrderEntry.Shipping_Contact.Update(orderParams.objSOOrderEntry.Shipping_Contact.Current);
                }
            }
        }

        private void SetShippingAddressObjectsData(PrepareAndImportOrdersParams orderParams)
        {
            orderParams.objSOOrderEntry.Shipping_Address.Current = (SOShippingAddress)orderParams.objSOOrderEntry.Shipping_Address.Select();
            if (orderParams.objSOOrderEntry.Shipping_Address.Current != null)
            {
                if (orderParams.objliUsrMapping.Where(x => (x.TargetObject == orderParams.objSOOrderEntry.Shipping_Address.View.Name)).Count() > 0)
                {
                    orderParams.objSOOrderEntry.Shipping_Address.Current.OverrideAddress = true;
                    orderParams.objSOOrderEntry.Shipping_Address.Update(orderParams.objSOOrderEntry.Shipping_Address.Current);
                    AssignValueAsPerIntegrationMapping(orderParams.objliUsrMapping, orderParams.objSOOrderEntry.Shipping_Address.Current, orderParams.ObjCurrentOrder, null, orderParams.objSOOrderEntry.Shipping_Address.View.Name);
                    if (orderParams.objSOOrderEntry.Shipping_Address.Current.State != null)
                    {
                        orderParams.objSOOrderEntry.Shipping_Address.Current.State = SOHelper.GetStateID(orderParams.objSOOrderEntry, orderParams.objSOOrderEntry.Shipping_Address.Current.State, orderParams.objSOOrderEntry.Shipping_Address.Current.CountryID);
                    }
                    orderParams.objSOOrderEntry.Shipping_Address.Update(orderParams.objSOOrderEntry.Shipping_Address.Current);
                }
            }
        }

        private void SetDocumentLevelDiscountandTaxData(PrepareAndImportOrdersParams orderParams)
        {
            if (taxtotal <= 0) return;
            SOOrder currentFin = (SOOrder)orderParams.objSOOrderEntry.Document.Current;
            if (currentFin != null && orderParams.objSOOrderEntry.sosetup.Current != null)
            {
                SOSetupAmazonExt objSOOSetupext = orderParams.objSOOrderEntry.sosetup.Current.GetExtension<SOSetupAmazonExt>();
                if (objSOOSetupext != null && !string.IsNullOrEmpty(objSOOSetupext.UsrAmazonTaxID))
                {
                    if (currentFin.TaxZoneID != objSOOSetupext.UsrAmazonTaxZoneID)
                    {
                        currentFin.OverrideTaxZone = true;
                        currentFin = orderParams.objSOOrderEntry.CurrentDocument.Update(currentFin);
                        currentFin.TaxZoneID = objSOOSetupext.UsrAmazonTaxZoneID;
                        currentFin = orderParams.objSOOrderEntry.CurrentDocument.Update(currentFin);
                    }
                    if (currentFin.TaxZoneID != objSOOSetupext.UsrAmazonTaxZoneID) return;

                    SOTaxTran orderTax = (SOTaxTran)orderParams.objSOOrderEntry.Taxes.Select();
                    if (orderTax == null && !string.IsNullOrEmpty(objSOOSetupext.UsrAmazonTaxID))
                    {
                        orderTax = (SOTaxTran)orderParams.objSOOrderEntry.Taxes.Cache.Insert();
                        orderTax.OrderType = orderParams.objSOOrderEntry.Document.Current.OrderType;
                        orderTax.OrderNbr = orderParams.objSOOrderEntry.Document.Current.OrderNbr;
                        orderTax.LineNbr = int.MaxValue;
                        orderTax.TaxID = objSOOSetupext.UsrAmazonTaxID;
                        orderParams.objSOOrderEntry.Taxes.Cache.Update(orderTax);
                        orderTax.CuryTaxableAmt = taxableamount;
                        orderTax.CuryTaxAmt = taxtotal;
                        orderTax.TaxRate = taxRate;
                        orderParams.objSOOrderEntry.Taxes.Cache.Update(orderTax);
                        orderParams.objSOOrderEntry.Document.SetValueExt<SOOrder.curyTaxTotal>(orderParams.objSOOrderEntry.Document.Current, taxtotal);
                        orderTotal = orderParams.objSOOrderEntry.Document.Current.OrderTotal + orderTax.CuryTaxAmt;
                        orderParams.objSOOrderEntry.Document.SetValueExt<SOOrder.curyOrderTotal>(orderParams.objSOOrderEntry.Document.Current, orderTotal);
                        orderParams.objSOOrderEntry.Document.Cache.Update(orderParams.objSOOrderEntry.Document.Current);
                    }
                    else if (orderTax != null && !string.IsNullOrEmpty(objSOOSetupext.UsrAmazonTaxID))
                    {
                        orderTax.CuryTaxAmt = taxtotal;
                        orderParams.objSOOrderEntry.Taxes.Cache.Update(orderTax);
                        orderParams.objSOOrderEntry.Document.SetValueExt<SOOrder.curyTaxTotal>(orderParams.objSOOrderEntry.Document.Current, taxtotal);
                        orderTotal = orderParams.objSOOrderEntry.Document.Current.OrderTotal + orderTax.CuryTaxAmt;
                        orderParams.objSOOrderEntry.Document.SetValueExt<SOOrder.curyOrderTotal>(orderParams.objSOOrderEntry.Document.Current, orderTotal);
                        orderParams.objSOOrderEntry.Document.Cache.Update(orderParams.objSOOrderEntry.Document.Current);
                    }

                }
            }
        }

        private void SetFreightData(PrepareAndImportOrdersParams orderParams)
        {
            //Update Premium Freight
            SOOrder document = orderParams.objSOOrderEntry.Document.Current;
            SOOrderAmazonExt documentExt = PXCache<SOOrder>.GetExtension<SOOrderAmazonExt>(document);
            document.CuryPremiumFreightAmt = documentExt.UsrAmazonFreightTotal.GetValueOrDefault(0) - documentExt.UsrAmazonFreightDiscountTotal.GetValueOrDefault(0);
            orderParams.objSOOrderEntry.Document.Update(document);
        }
        private void FillDiscountObjectsData(SOOrderEntry orderEntry, SOLine newitems, OrderItem currentitem)
        {
            if (currentitem != null && (currentitem != null))
            {
                orderEntry.Transactions.Current = newitems;
                if (orderEntry.Transactions.Current == null) return;
                decimal discount = decimal.Zero;
                discount += currentitem.CODFeeDiscount != null && !string.IsNullOrEmpty(currentitem.CODFeeDiscount.Amount) ? Convert.ToDecimal(currentitem.CODFeeDiscount.Amount) : 0;
                //discount += currentitem.ShippingDiscount != null && !string.IsNullOrEmpty(currentitem.ShippingDiscount.Amount) ? Convert.ToDecimal(currentitem.ShippingDiscount.Amount) : 0;

                discount += currentitem.PromotionDiscount != null && !string.IsNullOrEmpty(currentitem.PromotionDiscount.Amount) ? Convert.ToDecimal(currentitem.PromotionDiscount.Amount) : 0;

                if (discount > 0 && orderEntry.Transactions.Current != null)
                {
                    orderEntry.Transactions.Current.ManualDisc = true;
                    orderEntry.Transactions.Current.CuryDiscAmt = discount;
                    orderEntry.Transactions.Update(orderEntry.Transactions.Current);
                }
            }
        }

        public void CreatePaymentProcess(PrepareAndImportOrdersParams orderParams)
        {
            SOOrder order = orderParams.objSOOrderEntry.Document.Current;
            SOOrderAmazonExt OrderExt = order.GetExtension<SOOrderAmazonExt>();
            if (orderParams.objSOOrderEntry.sosetup.Current != null)
            {
                SOSetupAmazonExt objSOOSetupext = orderParams.objSOOrderEntry.sosetup.Current.GetExtension<SOSetupAmazonExt>();
                if (objSOOSetupext != null && !string.IsNullOrEmpty(objSOOSetupext.UsrAmazonPaymentMethodID))
                {
                    string paymentType = ARPaymentType.Payment;
                    orderParams.paymentGraph.Clear();
                    ARPayment payment = new ARPayment()
                    {
                        DocType = paymentType
                    };
                    payment = PXCache<ARPayment>.CreateCopy(orderParams.paymentGraph.Document.Insert(payment));
                    payment.CustomerID = order.CustomerID;
                    payment.CustomerLocationID = order.CustomerLocationID;
                    payment.PaymentMethodID = objSOOSetupext.UsrAmazonPaymentMethodID;
                    payment.PMInstanceID = order.PMInstanceID;
                    payment.CuryOrigDocAmt = 0m;
                    payment.ExtRefNbr = OrderExt != null ? OrderExt.UsrAmazonOrderID : order.OrderNbr;
                    payment.DocDesc = order.OrderDesc;
                    payment = orderParams.paymentGraph.Document.Update(payment);
                    SOAdjust adj = new SOAdjust()
                    {
                        AdjdOrderType = order.OrderType.Trim(),
                        AdjdOrderNbr = order.OrderNbr.Trim()
                    };
                    orderParams.paymentGraph.SOAdjustments.Insert(adj);
                    if (payment.CuryOrigDocAmt == 0m)
                    {
                        payment.CuryOrigDocAmt = payment.CurySOApplAmt;
                        payment = orderParams.paymentGraph.Document.Update(payment);
                    }
                    orderParams.paymentGraph.Actions.PressSave();
                    if (orderParams.paymentGraph.Actions.Contains("Release"))
                    {
                        orderParams.paymentGraph.Actions["Release"].Press();
                    }
                }
            }
        }

        private void AssignValueAsPerIntegrationMapping(List<SOFieldMapping> liUsrMapping, object newValue, Order currentOrder, OrderItem currentitem, string viewName)
        {
            string fieldValue = string.Empty;
            foreach (SOFieldMapping item in liUsrMapping.Where(x => x.TargetObject.Contains(viewName)))
            {
                if (!string.IsNullOrEmpty(item.SourceField) && !string.IsNullOrEmpty(item.TargetField) &&
                        item.TargetField.ToLower().Trim() != SOConstants.customerId.ToLower() &&
                        item.TargetField.ToLower().Trim() != SOConstants.inventoryID.ToLower() &&
                        item.TargetField.ToLower().Trim() != SOConstants.ordertype.ToLower())
                {
                    fieldValue = /*viewName == SOConstants.currentDocument*/ viewName.Contains("CurrentDocument") && item.SourceObject.ToLower().Contains(SOConstants.ShippingPrice) && item.SourceField.ToLower().Trim() == SOConstants.Amount ? Convert.ToString(shippingprice) : GetAmazonvalue(currentOrder, currentitem, item.SourceObject, item.SourceField);
                    SimulatePropertyValue(newValue, item.TargetField, fieldValue);
                }
            }
        }
 
        private string GetAmazonvalue(Order currentOrder, OrderItem currentitem, string sourceObject, string sourceField)
        {
            string fieldValue = string.Empty;
            string sourceName = string.Empty;
            string[] sourcevalues = null;
            string source = string.Empty;
            string field = string.Empty;
            sourcevalues = Regex.Split(sourceObject, "->");
            if (sourcevalues.Length > 0)
            {
                source = sourcevalues[0];
                field = sourcevalues[sourcevalues.Length - 1];
            }
            if (!string.IsNullOrEmpty(sourceObject))
            {
                if (!string.IsNullOrEmpty(source) && source.ToLower().Trim() == SOConstants.orderItem && currentitem != null)
                {
                    Type targetType = currentitem.GetType();
                    PropertyInfo targetproperty = targetType.GetProperty(field);
                    sourceName = targetproperty != null ? targetproperty.Name : string.Empty;
                    switch (sourceName.ToLower().Trim())
                    {
                        case SOConstants.itemPrice:
                            fieldValue = currentitem.ItemPrice != null ? Convert.ToString(currentitem.ItemPrice.GetType().GetProperty(sourceField).GetValue(currentitem.ItemPrice, null)) : string.Empty;
                            break;
                        case SOConstants.itemTax:
                            fieldValue = currentitem.ItemTax != null ? Convert.ToString(currentitem.ItemTax.GetType().GetProperty(sourceField).GetValue(currentitem.ItemTax, null)) : string.Empty;
                            break;
                        case SOConstants.promotionDiscount:
                            fieldValue = currentitem.PromotionDiscount != null ? Convert.ToString(currentitem.PromotionDiscount.GetType().GetProperty(sourceField).GetValue(currentitem.PromotionDiscount, null)) : string.Empty;
                            break;
                        case SOConstants.ShippingPrice:
                            fieldValue = currentitem.ShippingPrice != null ? Convert.ToString(currentitem.ShippingPrice.GetType().GetProperty(sourceField).GetValue(currentitem.ShippingPrice, null)) : string.Empty;
                            break;
                        case SOConstants.shippingDiscount:
                            fieldValue = currentitem.ShippingDiscount != null ? Convert.ToString(currentitem.ShippingDiscount.GetType().GetProperty(sourceField).GetValue(currentitem.ShippingDiscount, null)) : string.Empty;
                            break;
                        case SOConstants.shippingTax:
                            fieldValue = currentitem.ShippingTax != null ? Convert.ToString(currentitem.ShippingTax.GetType().GetProperty(sourceField).GetValue(currentitem.ShippingTax, null)) : string.Empty;
                            break;
                        case SOConstants.giftWrapPrice:
                            fieldValue = currentitem.GiftWrapPrice != null ? Convert.ToString(currentitem.GiftWrapPrice.GetType().GetProperty(sourceField).GetValue(currentitem.GiftWrapPrice, null)) : string.Empty;
                            break;
                        case SOConstants.giftWrapTax:
                            fieldValue = currentitem.GiftWrapTax != null ? Convert.ToString(currentitem.GiftWrapTax.GetType().GetProperty(sourceField).GetValue(currentitem.GiftWrapTax, null)) : string.Empty;
                            break;
                        case SOConstants.pointsGranted:
                            fieldValue = currentitem.PointsGranted != null ? Convert.ToString(currentitem.PointsGranted.GetType().GetProperty(sourceField).GetValue(currentitem.PointsGranted, null)) : string.Empty;
                            break;
                        case SOConstants.codFee:
                            fieldValue = currentitem.CODFee != null ? Convert.ToString(currentitem.CODFee.GetType().GetProperty(sourceField).GetValue(currentitem.CODFee, null)) : string.Empty;
                            break;
                        case SOConstants.codFeeDiscount:
                            fieldValue = currentitem.CODFeeDiscount != null ? Convert.ToString(currentitem.CODFeeDiscount.GetType().GetProperty(sourceField).GetValue(currentitem.CODFeeDiscount, null)) : string.Empty;
                            break;
                        case SOConstants.invoiceData:
                            fieldValue = currentitem.InvoiceData != null ? Convert.ToString(currentitem.InvoiceData.GetType().GetProperty(sourceField).GetValue(currentitem.InvoiceData, null)) : string.Empty;
                            break;
                        case SOConstants.buyerCustomizedInfo:
                            fieldValue = currentitem.BuyerCustomizedInfo != null ? Convert.ToString(currentitem.BuyerCustomizedInfo.GetType().GetProperty(sourceField).GetValue(currentitem.BuyerCustomizedInfo, null)) : string.Empty;
                            break;
                        default:
                            fieldValue = Convert.ToString(currentitem.GetType().GetProperty(sourceField).GetValue(currentitem, null));
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(source) && source.ToLower().Trim() == SOConstants.order && currentOrder != null)
                {
                    Type targetType = currentOrder.GetType();

                    PropertyInfo targetproperty = targetType.GetProperty(field);
                    sourceName = targetproperty != null ? targetproperty.Name : string.Empty;

                    switch (sourceName.ToLower().Trim())
                    {
                        case SOConstants.shippingAddress:
                            fieldValue = currentOrder.ShippingAddress != null ? Convert.ToString(currentOrder.ShippingAddress.GetType().GetProperty(sourceField).GetValue(currentOrder.ShippingAddress, null)) : string.Empty;
                            break;
                        case SOConstants.orderTotal:
                            fieldValue = currentOrder.OrderTotal != null ? Convert.ToString(currentOrder.OrderTotal.GetType().GetProperty(sourceField).GetValue(currentOrder.OrderTotal, null)) : string.Empty;
                            break;
                        case SOConstants.paymentExecutiondetailItem:
                            fieldValue = currentOrder.PaymentExecutionDetail != null ? Convert.ToString(currentOrder.PaymentExecutionDetail.GetType().GetProperty(sourceField).GetValue(currentOrder.PaymentExecutionDetail, null)) : string.Empty;
                            break;
                        default:
                            fieldValue = Convert.ToString(currentOrder.GetType().GetProperty(sourceField).GetValue(currentOrder, null));
                            break;
                    }
                }
            }
            return fieldValue;
        }

        private void SimulatePropertyValue(object to, string curfieldName, string value)
        {
            Type targetType = to.GetType();
            if (string.IsNullOrEmpty(curfieldName)) return;
            PropertyInfo toProp = targetType.GetProperty(curfieldName);

            //Get from Extensions if any
            if (toProp == null)
            {
                foreach(var extension in (to as IBqlTable).GetExtensions())
                {
                    toProp = extension.GetType().GetProperty(curfieldName);
                    if (toProp != null)
                    {
                        to = extension;
                        break;
                    }
                }
            }

            if (toProp != null && toProp.CanWrite && !string.IsNullOrEmpty(value))
            {
                value = value.Trim();
                try
                {
                    if (toProp.PropertyType == typeof(string))
                    {
                        toProp.SetValue(to, value, null);
                    }
                    else if (toProp.PropertyType == typeof(int))
                    {
                        toProp.SetValue(to, Convert.ToInt32(value), null);
                    }
                    else if (toProp.PropertyType == typeof(decimal))
                    {
                        toProp.SetValue(to, Convert.ToDecimal(value), null);
                    }
                    else if (toProp.PropertyType == typeof(DateTime))
                    {
                        toProp.SetValue(to, Convert.ToDateTime(value), null);
                    }
                    else if (toProp.PropertyType == typeof(bool))
                    {
                        toProp.SetValue(to, Convert.ToBoolean(value), null);
                    }
                    else if (toProp.PropertyType == typeof(int?))
                    {
                        toProp.SetValue(to, Convert.ToInt32(value), null);
                    }
                    else if (toProp.PropertyType == typeof(decimal?))
                    {
                        toProp.SetValue(to, Convert.ToDecimal(value), null);
                    }
                    else if (toProp.PropertyType == typeof(DateTime?))
                    {
                        toProp.SetValue(to, Convert.ToDateTime(value), null);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Contains(SOMessages.inputStringWas) || ex.Message.ToLower().Contains(SOMessages.notRecognized))
                        throw new Exception(SOMessages.chkFieldMapping + curfieldName + SOMessages.dataTypeNotMatching);
                }
            }
        }
    }
    #endregion
}