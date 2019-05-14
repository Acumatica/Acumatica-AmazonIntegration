using System;
using PX.Data;
using MarketplaceWebServiceSellers;
using MarketplaceWebServiceSellers.Model;

namespace AmazonIntegration
{
    # region Amazon Configurations Service Methods
    public class SOServiceCalls
    {
        #region Variables
        private readonly MarketplaceWebServiceSellers.MarketplaceWebServiceSellers client;
        #endregion

        #region Constructors

        public SOServiceCalls(MarketplaceWebServiceSellers.MarketplaceWebServiceSellers seller)
        {
            this.client = seller;
        }
        #endregion

        internal static void TestSellerAccount(SOAmazonSetup setupview)
        {
            bool marketplaceIdExist = false;
            try
            {
                if (setupview != null)
                {
                    MarketplaceWebServiceSellersConfig msconfig = new MarketplaceWebServiceSellersConfig();
                    msconfig.ServiceURL = SOHelper.GetIntegrationType(setupview.IntegrationType.Trim(), SOConstants.serviceUrl);
                    MarketplaceWebServiceSellers.MarketplaceWebServiceSellers client = new MarketplaceWebServiceSellersClient(SOConstants.appName, SOConstants.version, setupview.AccessKey, setupview.SecretKey, msconfig);
                    SOServiceCalls sample = new SOServiceCalls(client);
                    ListMarketplaceParticipationsResponse response = null;
                    response = sample.InvokeListMarketplaceParticipations(setupview.SellerId, setupview.AuthToken);
                    if (response != null)
                    {
                        string integrationType = SOHelper.GetIntegrationType(setupview.IntegrationType.Trim(), SOConstants.domainName);
                        foreach (Marketplace marketplace in response.ListMarketplaceParticipationsResult.ListMarketplaces.Marketplace)
                        {
                            if (!string.IsNullOrEmpty(marketplace.Name) && !string.IsNullOrEmpty(integrationType) && marketplace.Name.ToLower() == integrationType.ToLower()
                                && !string.IsNullOrEmpty(setupview.MarketplaceId) && !string.IsNullOrEmpty(marketplace.MarketplaceId) && setupview.MarketplaceId.ToLower() == marketplace.MarketplaceId.ToLower())
                            {
                                marketplaceIdExist = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(SOMessages.connectionFailed + Convert.ToString(ex.Message));
            }
            if (marketplaceIdExist)  // we need this condition check because  if we  try to raise PXOperationCompletedException inside try catch block , 
                                     //the catch block  gets executed and displays a success message with error icon. 

                throw new PXOperationCompletedException(SOMessages.connectionSuccess);
            else
                throw new PXOperationCompletedWithErrorException(SOMessages.connectionFailed + SOMessages.errorMsg);

        }
        public ListMarketplaceParticipationsResponse InvokeListMarketplaceParticipations(string sellerId, string authToken)
        {
            ListMarketplaceParticipationsRequest request = new ListMarketplaceParticipationsRequest();
            request.SellerId = sellerId;
            request.MWSAuthToken = authToken;
            return this.client.ListMarketplaceParticipations(request);
        }
    }
    # endregion
}