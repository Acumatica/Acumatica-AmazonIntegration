[![Project Status](http://opensource.box.com/badges/deprecated.svg)](http://opensource.box.com/badges)

Amazon Marketplace Integration
==================================

# 🚫 This repository has been archived

As of 2023 R1; customers are advised to use native Amazon integration.

Please refer [Amazon Integration - Information and Managed Availability Sign-up](https://community.acumatica.com/retail-commerce-113/amazon-integration-information-and-managed-availability-sign-up-8235?tid=8235&fid=113) for additional information.

- - - -
- - - -

This integration supports Fulfillment by Amazon (FBA) and Fulfillment by Merchant (FBM) fulfillment channels. 

Using this integration user can import orders placed on Amazon Marketplace into Acumatica and can transfer order fulfillment details from Acumatica to Amazon Marketplace for FBM type of orders.

#### Fulfilled by Amazon (FBA): 
Seller sells and Amazon ships. Seller sends bulk products to Amazon’s fulfillment centers. When products are sold, Amazon picks, packs, and ships the orders. The integration imports FBA orders as invoice orders (Sales Orders which follow the type IN invoice workflow). Amazon Orders with status Shipped are retrieved during the import process.

#### Fulfilled by Merchant (FBM ): 
Seller lists the products on Amazon while handling storage and all aspects of order fulfillment. When products are sold, the integration imports FBM orders as sales orders. The seller is required to pick, pack, and ship products included on such orders. Shipment/Fulfilment details needs to be submitted back to Amazon using the integration. Amazon Orders with status Unshipped are retrieved during the import process.

### Prerequisites
* Acumatica 2018 R2 (18.204.0013+) or higher
* Inventory/Stock, Products and Customers has to be setup prior using this integration. Product/Stock Item needs to be setup in Acumatica by either having matching Inventory ID OR Cross-Reference (Alternate ID) same as Amazon SellerSKU/ASIN (specified in Marketplace Configuration Field Mapping) 
* This integration requires “Amazon Seller Professional Account” credentials for configuring integrations. Please visit (https://developer.amazonservices.com/) for more information on getting MWS Credentials.

Quick Start
-----------

### Installation

##### Install customization deployment package
1. Download PXAmazonIntegrationPkg.zip from this repository
2. In your Acumatica ERP instance, navigate to System -> Customization -> Customization Projects (SM204505), import PXAmazonIntegrationPkg.zip as a customization project
3. Publish customization project.

### Configuration

#### Sales Order Preferences

Navigate to Sales Order Preferences (SO101000) -> Amazon Configuration Tab and specify preferences which will be used during Amazon orders import and these settings are common for both FBA and FBM type of orders.

![Screenshot](/_ReadMeImages/SO101000.png)

##### Configuration Settings Summary

 | Element               | Description |
 | :---                  | :--- |
 | **Guest Customer ID** | All imported Amazon orders will be associated to this specified customer.  |
 | **Tax Zone ID**       | This Tax ID will be default Tax ID to all imported Amazon orders. The details will be shown in Sales Order form’s Tax details tab. <ul><li>Only taxes which are created with the option “Propagate Manually Set Tax Amount from Sales Orders to Invoice” can be selected as default Tax ID</ul></li> |
 | **Payment Method ID** | This Payment Method will be applied to all imported Amazon orders. The details will be shown in Sales Order form’s Payments tab. |
 | **Ship Via**          | This Ship Via method will be applied to all imported Amazon orders. The details will be shown in Sales Order form’s Shipping settings tab. |
 | **Initial From Date** | Cut-off date after which orders will be available for import in Schedule import orders screen. Assume that you have configured the date “1st Jan 2019”, then system will fetch amazon orders which are placed from 1st January 2019. |
 
#### Marketplace Configuration

Marketplace Configuration Screen is used to control various features of the Integration. 

![Screenshot](/_ReadMeImages/SO204000.png)

##### Marketplace Configuration Summary

| Element               | Description |
| :---                  | :--- |
| **Integration ID** | You can set a unique value that can be used to identify all the configuration details in other screens related to this integration ID. |
| **Status** | This field is used to mark a specific Integration as Active / Inactive. If the integration is marked as Inactive, then system will not display the details/logs related to this specific integration. |
| **Integration Type** | Available Integration Types : <ul><li>Amazon FBA</li><li>Amazon.ca FBA</li><li>Amazon.co.uk FBA</li><li>Amazon.de FBA</li><li>Amazon.jp FBA</li><li>Amazon.in FBA</li><li>Amazon FBM</li></ul> |
| **Warehouse** | All active warehouses will be loaded from the system to select specific warehouse in Acumatica for order processing. All the imported orders under this specific integration will be associated to this configured warehouse by default. |
| **Order Type** | All the active order types will be loaded from the system to select specific Order type. Based on the selected integration type these order type template will be loaded for selection. <ul><li>For FBA integrations types the system will display the “IN – Invoice” type templates for the selection, whereas for FBM Integration type it will display “SO – Sales Order” template order types.</li></ul> |
| **Seller ID** | This field is used to configure the Amazon Marketplace Seller ID. |
| **Auth Token** | This field is used to configure the Auth Token of the respective Seller. |
| **Access Key** | This field is used to configure the Access Key of the respective Seller. |
| **Secret Key** | This field is used to configure the Secret Key of the respective Seller. |
| **Marketplace ID** | This field is used to configure the Marketplace ID of the respective Seller. Each integration type will have its own Marketplace ID. |
| **Description** | This field is used to add the custom description about the Integration. |
| **Test Connection** | This button is used to validate the provided credentials and shows the result whether given configuration details are correct or not. If the provided credentials are incorrect then the system will show an error message. |
| **Field Mapping Configuration** | This grid is used to make the field mappings for importing the Amazon order values into required target fields of Acumatica Sales Order. Ex: We can map the configuration, “Amazon Order ID” to be displayed as Sales Order’s Customer Order number. "Marketplace Configuration.xlsx" can be used to setup mapping. |

#### Tax Configuration for Amazon Integration
Amazon Tax Amount will be brought into Acumatica at the time of order import (FBA & FBM). To match the order Taxes and Totals on both systems.

Following are the list of steps that need to be followed to enable manual taxes in Acumatica:

* Navigate to Tax Categories screen (TX205500) and create new tax category "AMAZONTC"

![Screenshot](/_ReadMeImages/TX205500.png)

* Navigate to Tax Zones screen (TX206000) and create new Tax Zone ID "AMAZONTZ" and assign default tax category created in prior step.

![Screenshot](/_ReadMeImages/TX206000.png)

* Navigate to Taxes Screen (TX205000) and create new Tax ID "AMAZONTAX" and assign an unlimited Tax Schedule. Make sure to check “Propagate Manually Set Tax….” check box.

![Screenshot](/_ReadMeImages/TX205000.png)

* In Taxes screen, select Tax Category "AMAZONTC" created in prior step.

![Screenshot](/_ReadMeImages/TX205000a.png)

#### Handling Discounts

Applicable discount is imported as Manual Discount for line item.

#### Handling Shipping/Freight Charges

Net Shipping charge (shipping charge – shipping discount) is stored in **Premium Freight Price** field in Totals tab of the Sales Order.
Import process imports shipping charge and shipping discount per order line. Aggregated value of these fields is stored in respective fields in Total tab as well. 

### Usage

#### Import Orders
This screen (SO509100) is used to import Amazon orders from Amazon Marketplace. 

![Screenshot](/_ReadMeImages/SO509100.png)

##### Import Orders Summary
Once the order is imported, you can view the order details from Sales Order screen for respective order type. You can also modify any details of the order once it is imported to Acumatica, but these modifications will not have any impacts at Amazon Marketplace side.

| Element               | Description |
| :---                  | :--- |
| **From Date** | Start date can be selected, from which date the orders should be pulled for importing into Acumatica. |
| **To Date** | End date can be selected, till what date the orders should be pulled for importing into Acumatica. <ul><li>The Amazon orders will be retrieved by clicking on the prepare button for the period between the selected "From" and "To" Dates.</li><ul> |
| **Integration ID** | This field will display the list of all active Marketplace Configurations. With the help of this field, you can select from which seller account orders required to be prepared and imported. Both FBA and FBM type of orders can be processed with this field. |
| **Process All Integrations** | This option is used to retrieve the orders from all active marketplace configurations at once. |
| **Records Grid** | After providing the required input parameters at header sections, such as giving the required period and selecting the required integration ID, the pulled records can be viewed by using this grid. |
| **Prepare** | This button is used to retrieve the Amazon orders for the given period and under selected Integration ID. We can perform this action for any number of times and each time it will fetch new records if any new orders are created under the given period. After successful prepare operation, the system will display all the fetched Amazon orders under results grid which will be ready for importing into Acumatica. |
| **Import** | This button is used to import the fetched Amazon orders into Acumatica. Only the selected orders will be imported into the system. Once an order is imported, then the same order will no longer available for importing again. During this import process, all the successful imports and failed to import details will be maintained in separate GI log screens. |
| **Import All** | Import button is used to import only the selected Amazon orders, whereas this "Import All" button is used to import all the available orders into Acumatica at once. One doesn’t have to select any specific order for processing all the records, by clicking on the "Import All" button the system will automatically selects all the available orders from the results grid and processes for importing. Start date can be selected, from which date the orders should be pulled for importing into Acumatica. |

#### Schedule Import Orders

With the help of this screen (SO509200), one can schedule “Prepare and Import” of orders from the last import date to required date. We can also schedule prepare and import process of all the active integrations at a time.

![Screenshot](/_ReadMeImages/SO509200.png)

##### Schedule Import Orders Summary

This screen is similar to Import Orders screen and the difference is that both Prepare and Import operations can be performed at once whereas these actions can be performed individually in Import Orders screen (SO509100).

| Element               | Description |
| :---                  | :--- |
| **From Date** | Start date can be selected, from which date the orders should be pulled for importing into Acumatica. |
| **To Date** | End date can be selected, till what date the orders should be pulled for importing into Acumatica. <ul><li>The Amazon orders will be retrieved by clicking on the prepare button for the period between the selected "From" and "To" Dates.</li><ul> |
| **Integration ID** | This field will display the list of all active Marketplace Configurations. With the help of this field, you can select from which seller account orders required to be prepared and imported. Both FBA and FBM type of orders can be processed with this field. |
| **Process All Integrations** | The above Integration ID field is used to retrieve the orders from one marketplace integration at a time, whereas this “Process All Integrations” option is used to retrieve the orders from all active marketplace configurations at once. |
| **Records Grid** | After providing the required input parameters at header sections, such as giving the required period and selecting the required integration ID, the pulled records can be viewed by using this grid. |
| **Prepare & Import** | This button is used to import (no prepare and import separate actions required) all the Amazon orders into Acumatica for the displayed period and under selected Integration ID(s). After successful operation, the user can check all the imported orders in Sales order screen. <ul><li>Once the operation completed, the respective scheduled rows will be disappeared from this screen.</li><li>For each schedule, the system will assign one process ID automatically and the user can check all the details in GI log screens with this process ID.</li><ul> |

#### Submit FBM Shipment Information

This screen (SO509300) is used to submit the feed notification to Amazon Marketplace from Acumatica application. All the FBM type of orders, which are in “Completed” status, are available for feed submit. That means, the fulfillment process will happen at Acumatica side and the shipping information / tracking number will be sent to Amazon Marketplace. 

![Screenshot](/_ReadMeImages/SO509300.png)

##### Submit FBM Shipment Information Summary

| Element               | Description |
| :---                  | :--- |
| **To Date** | This field considers the Shipment Date. All the shipments which are placed till the selected date, are retrieved and available for submitting the feed. |
| **Integration ID** | This field list outs all the available FBM integration types/sellers. Please select the required seller ID and all the orders which are completed by the shipment process under selected seller will be shown for feed submit. Only the FBM type of integration ID’s will be shown in this field. |
| **Results Grid** | Based on the selected date and FBM integration ID, all the matched records will be displayed in this grid. The user can select the required number of orders for submit feed. <ul><li>This grid will show the detailed shipment information of each amazon order.</li><ul> |
| **Submit Feed** | This button is used to submit the feed information from Acumatica to Amazon Marketplace. Only the selected orders will be submitted from the system. Once an order is submitted, then the same order will no longer be available for submitting again. During this Submit feed process, all the successful submits and failure details will be maintained in separate GI log screens. |
| **Submit Feed All** | Submit Feed button is used to submit only the selected Amazon orders, whereas this Submit Feed All button is used to submit all the available orders into Amazon Marketplace at once. We don't have to select any specific order for processing all the records, by clicking on the "Submit Feed All" button the system will automatically selects all the available orders from the results grid and processes. |

FBM Shipping information will be updated in Amazon as per below mapping:

| **Acumatica Field** | **Amazon Order -> Package -> Edit Shipment** |
| :--- | :--- |
| Shipment Date | Ship Date |
| Amazon Carrier Code <ul><li>Value is case sensitive and must match with one of the supported Carrier.</li><ul> | Carrier |
| Ship Via / Service Method | Shipping Service |
| Packages -> Tracking Number | Tracking ID |

#### Import FBA Tracking #

This screen (SO509400) is used to retrieve the tracking number to the imported FBA orders in Acumatica.

![Screenshot](/_ReadMeImages/SO509400.png)

Whenever you import FBA order from the marketplace, the orders will be imported and tracking number will not be assigned to the order. All the imported orders will be displayed in this screen and you can retrieve and update the tracking numbers to the imported order by using “Process / Process all” actions. Once the process action performed, the tracking numbers will be assigned to the respective order's note and those orders will be removed from this screen.

##### Import FBA Tracking # Summary

| Element | Description |
| :--- | :--- |
| **From Date** | You can select the start date, from which imported date the orders should be pulled for processing the tracking details. |
| **To Date** | You can select the end date, till which imported date the orders should be pulled for processing the tracking information. <ul><li>The imported FBA orders will be pulled and displayed by clicking on the process button for the period between these selected From and To Dates</li></ul> |
| **Current Day** | If you select this check box, then From and To dates will be disabled and only today date records will be displayed for processing the tracking information. |
| **Integration ID** | This field will show the list of all active FBA Marketplace Configurations. With the help of this field, you can select from which seller account orders required to be processed. |
| **Process All Integrations** | The above Integration ID field is used to retrieve the orders from one marketplace integration at a time, whereas this “Process All Integrations” option is used to retrieve the orders from all active FBA marketplace configurations at once. |
| **Process** | This button is used to import the Tracking number. Only the selected orders will be processed, once an order is processed, then the same order will no longer be available for processing again. |
| **Process All** | Process button is used to process only the selected Amazon orders, whereas this Process All button is used to process all the available orders at once. You do not require of selecting any specific order for processing all the records, by clicking on the Process all button the system will automatically selects all the available orders from the results grid and process for adding the tracking number to the imported order's note. |
| **Records Grid** | After providing the required input parameters at header sections such as giving the required period and selecting the required integration ID, you can view the pulled records by using this grid. |

#### Amazon Integration Logs

Below screens can be used to view logs created during import order or submit feed processes of this integration.

* Amazon SO Process Log (AZ000008)
* Amazon SO Import Log (AZ000016)
* Amazon SO Detailed Log (AZ000015)
* Amazon Shipment Process Log (AZ000012)
* Amazon Shipment Submit Log (AZ000014)
* Amazon Shipment Detailed Log (AZ000013)

Below screens can be used to clear/remove logs created during import order or submit feed processes of this integration.

* Amazon SO Clear Log (SO401001)
* Amazon Shipment Clear Log (SO401000)

Known Issues
------------
None at the moment

## Copyright and License

Copyright © `2019` `Acumatica`

This component is licensed under the MIT License, a copy of which is available online [here](LICENSE.md)
