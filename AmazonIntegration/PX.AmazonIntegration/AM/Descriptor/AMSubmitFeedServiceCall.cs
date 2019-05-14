using System;
using System.Text;
using MarketplaceWebService.Model;
using System.IO;
using System.Xml;
using PX.Data;
using System.Threading;
using MarketplaceWebService;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AmazonIntegration
{
    public class AMSubmitFeedServiceCall
    {
        public readonly MarketplaceWebService.MarketplaceWebService clientFeed;
        public AMSubmitFeedServiceCall(MarketplaceWebService.MarketplaceWebService Feeds)
        {
            this.clientFeed = Feeds;
        }
        public AmazonEnvelope InvokeServicesCalls(SubmitFeedParamaters objParams, string xmlFeedContent)
        {
            AmazonEnvelope resultEnvelope = null;
            try
            {
                MarketplaceWebService.MarketplaceWebService serviceConfig = this.Configurations(objParams.objSOAmazonSetup.IntegrationType.Trim(), objParams.objSOAmazonSetup.AccessKey, objParams.objSOAmazonSetup.SecretKey);
                if (serviceConfig != null)
                {
                    SubmitFeedResponse feedresponse = null;
                    feedresponse = new AMSubmitFeedServiceCall(serviceConfig).InvokeSubmitFeed(objParams, xmlFeedContent);
                    if (feedresponse != null && feedresponse.SubmitFeedResult != null && feedresponse.SubmitFeedResult.FeedSubmissionInfo != null &&
                        !string.IsNullOrEmpty(feedresponse.SubmitFeedResult.FeedSubmissionInfo.FeedProcessingStatus) &&
                        feedresponse.SubmitFeedResult.FeedSubmissionInfo.FeedProcessingStatus.ToUpper().Trim('_') == SOConstants.Submitted)
                    {
                        string submissionFeedId = feedresponse.SubmitFeedResult.FeedSubmissionInfo.FeedSubmissionId;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            new AMSubmitFeedServiceCall(serviceConfig).GetSubmitFeedResult(objParams, submissionFeedId, ms);
                            XmlDocument xDocument = new XmlDocument();
                            xDocument.Load(ms);
                            string xmlResultContent = SOHelper.ObjectToXMLConversion(xDocument, string.Empty, true);
                            XmlSerializer deserializer = new XmlSerializer(typeof(AmazonEnvelope), new XmlRootAttribute(SOConstants.amazonEnvelope));
                            using (TextReader reader = new StringReader(xmlResultContent))
                            {
                                resultEnvelope = (AmazonEnvelope)deserializer.Deserialize(reader);
                            }
                            return resultEnvelope;
                        }
                    }
                }
                else
                {
                    throw new PXException(SOConstants.credentialsinvalid);
                }
            }
            catch(Exception ex)
            {               
                throw new PXException(ex.Message);
            }
            return resultEnvelope;
        }
        private MarketplaceWebService.MarketplaceWebService Configurations(string intergrationType, string accessKey, string secretKey)
        {
            MarketplaceWebServiceConfig config = new MarketplaceWebServiceConfig();
            config.ServiceURL = SOHelper.GetIntegrationType(intergrationType.Trim(), SOConstants.serviceUrl);
            MarketplaceWebService.MarketplaceWebService serviceConfig = new MarketplaceWebService.MarketplaceWebServiceClient(accessKey, secretKey, SOConstants.appName, SOConstants.version, config);
            return serviceConfig;
        }

        private SubmitFeedResponse InvokeSubmitFeed(SubmitFeedParamaters objParams, string xmlFeedContent)
        {
            SubmitFeedResponse resSubmitFeed = new SubmitFeedResponse();
            try
            {
                UTF8Encoding encoding = new UTF8Encoding();
                Stream streamData = new MemoryStream(encoding.GetBytes(xmlFeedContent));
                SubmitFeedRequest request = new SubmitFeedRequest();
                request.Merchant = objParams.objSOAmazonSetup.SellerId;
                request.FeedContent = streamData;
                request.ContentMD5 = MarketplaceWebServiceClient.CalculateContentMD5(request.FeedContent);
                request.FeedContent.Position = 0;
                request.PurgeAndReplace = false;
                request.FeedType = SOConstants.feedTypePostOrderFulfillment;
                request.MWSAuthToken = objParams.objSOAmazonSetup.AuthToken;
                resSubmitFeed = clientFeed.SubmitFeed(request);
            }
            catch (Exception ex)
            {
                if (ex is MarketplaceWebServiceException)
                {
                    MarketplaceWebServiceException exception = ex as MarketplaceWebServiceException;
                    if (!string.IsNullOrEmpty(exception.ErrorCode) && exception.ErrorCode.ToLower().Trim() == SOMessages.requestThrottled)
                    {
                        Thread.Sleep(SOHelper.DelayProcess(objParams.objPartialMaint, SOConstants.apiSubmitFeed));
                        resSubmitFeed = InvokeSubmitFeed(objParams, xmlFeedContent);
                    }
                    else
                    {
                        throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                             ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                             : SOConstants.exceptionIsEmpty);
                    }
                }
                else
                {
                    throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                             ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                             : SOConstants.exceptionIsEmpty);
                }
            }
            return resSubmitFeed;
        }

        private void GetSubmitFeedResult(SubmitFeedParamaters objParams, string submissionFeedId, MemoryStream ms)
        {
            try
            {
                GetFeedSubmissionResultRequest request = new GetFeedSubmissionResultRequest();
                request.FeedSubmissionId = submissionFeedId;
                request.Merchant = objParams.objSOAmazonSetup.SellerId;
                request.MWSAuthToken = objParams.objSOAmazonSetup.AuthToken;
                request.FeedSubmissionResult = ms;
                clientFeed.GetFeedSubmissionResult(request);
            }
            catch (Exception ex)
            {
                if (ex is MarketplaceWebServiceException)
                {
                    MarketplaceWebServiceException exception = ex as MarketplaceWebServiceException;
                    if (!string.IsNullOrEmpty(exception.ErrorCode) && exception.ErrorCode.ToLower().Trim() == SOConstants.feedProcessingResultNotReady)
                    {
                        Thread.Sleep(SOHelper.DelayProcess(objParams.objPartialMaint, SOConstants.apiFeedResultWaiting));
                        GetSubmitFeedResult(objParams, submissionFeedId, ms);
                    }
                    else
                    {
                        throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                            ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                            : SOConstants.exceptionIsEmpty);
                    }
                }
                else
                {
                    throw new PXException(!string.IsNullOrEmpty(ex.Message) ? ex.Message :
                            ex.InnerException != null && ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message
                            : SOConstants.exceptionIsEmpty);
                }
            }
        }
        public class AmazonEnvelope
        {
            public Header Header { get; set; }
            public Message Message { get; set; }
        }
        public class Message
        {
            public string MessageID { get; set; }
            public ProcessingReport ProcessingReport { get; set; }
        }
        public class ProcessingReport
        {
            public string DocumentTransactionID { get; set; }
            public string StatusCode { get; set; }
            public ProcessingSummary ProcessingSummary { get; set; }
            [XmlElement(SOConstants.Result)]
            public List<Result> Result { get; set; }
        }
        public class ProcessingSummary
        {
            public int MessagesProcessed { get; set; }
            public int MessagesSuccessful { get; set; }
            public int MessagesWithError { get; set; }
            public int MessagesWithWarning { get; set; }
        }
        public class Result
        {
            public string MessageID { get; set; }
            public string ResultCode { get; set; }
            public string ResultMessageCode { get; set; }
            public string ResultDescription { get; set; }
        }
    }
}