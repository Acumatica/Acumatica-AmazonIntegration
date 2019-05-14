using Customization;
using PX.Data;
using PX.SM;
using PX.Common;
using System;

namespace AmazonIntegration
{
    public class ThrottleConstants : CustomizationPlugin
    {
        public override void UpdateDatabase()
        {
            ProjectList graph = PXGraph.CreateInstance<ProjectList>();
            try
            {
                SOThrottleConstants records = PXSelectReadonly<SOThrottleConstants>.Select(graph);
                if (records != null)
                    PXDatabase.Delete<SOThrottleConstants>();
                PXDatabase.Insert<SOThrottleConstants>(
                    new PXDataFieldAssign<SOThrottleConstants.apiname>(SOConstants.apiListOrders),
                    new PXDataFieldAssign<SOThrottleConstants.delayTime>("30000"),
                    new PXDataFieldAssign<SOThrottleConstants.createdDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.createdByID>(graph.Accessinfo.UserID),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedByID>(graph.Accessinfo.UserID)
                    );

                PXDatabase.Insert<SOThrottleConstants>(
                    new PXDataFieldAssign<SOThrottleConstants.apiname>(SOConstants.apiListOrdersByNextToken),
                    new PXDataFieldAssign<SOThrottleConstants.delayTime>("30000"),
                    new PXDataFieldAssign<SOThrottleConstants.createdDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.createdByID>(graph.Accessinfo.UserID),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedByID>(graph.Accessinfo.UserID));

                PXDatabase.Insert<SOThrottleConstants>(
                    new PXDataFieldAssign<SOThrottleConstants.apiname>(SOConstants.apiGetOrder),
                    new PXDataFieldAssign<SOThrottleConstants.delayTime>("30000"),
                    new PXDataFieldAssign<SOThrottleConstants.createdDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.createdByID>(graph.Accessinfo.UserID),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedByID>(graph.Accessinfo.UserID));

                PXDatabase.Insert<SOThrottleConstants>(
                    new PXDataFieldAssign<SOThrottleConstants.apiname>(SOConstants.apiGetListOrderItems),
                    new PXDataFieldAssign<SOThrottleConstants.delayTime>("10000"),
                    new PXDataFieldAssign<SOThrottleConstants.createdDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.createdByID>(graph.Accessinfo.UserID),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedByID>(graph.Accessinfo.UserID));

                PXDatabase.Insert<SOThrottleConstants>(
                    new PXDataFieldAssign<SOThrottleConstants.apiname>(SOConstants.apiSubmitFeed),
                    new PXDataFieldAssign<SOThrottleConstants.delayTime>("20000"),
                    new PXDataFieldAssign<SOThrottleConstants.createdDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.createdByID>(graph.Accessinfo.UserID),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedByID>(graph.Accessinfo.UserID));

                PXDatabase.Insert<SOThrottleConstants>(
                    new PXDataFieldAssign<SOThrottleConstants.apiname>(SOConstants.apiGetFeedSubmissionResult),
                    new PXDataFieldAssign<SOThrottleConstants.delayTime>("20000"),
                    new PXDataFieldAssign<SOThrottleConstants.createdDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.createdByID>(graph.Accessinfo.UserID),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedByID>(graph.Accessinfo.UserID));

                PXDatabase.Insert<SOThrottleConstants>(
                    new PXDataFieldAssign<SOThrottleConstants.apiname>(SOConstants.apiRequestReport),
                    new PXDataFieldAssign<SOThrottleConstants.delayTime>("10000"),
                    new PXDataFieldAssign<SOThrottleConstants.createdDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.createdByID>(graph.Accessinfo.UserID),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedByID>(graph.Accessinfo.UserID));

                PXDatabase.Insert<SOThrottleConstants>(
                    new PXDataFieldAssign<SOThrottleConstants.apiname>(SOConstants.apiGetRequestReportList),
                    new PXDataFieldAssign<SOThrottleConstants.delayTime>("30000"),
                    new PXDataFieldAssign<SOThrottleConstants.createdDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.createdByID>(graph.Accessinfo.UserID),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedByID>(graph.Accessinfo.UserID));

                PXDatabase.Insert<SOThrottleConstants>(
                    new PXDataFieldAssign<SOThrottleConstants.apiname>(SOConstants.getGeneratedReportID),
                    new PXDataFieldAssign<SOThrottleConstants.delayTime>("30000"),
                    new PXDataFieldAssign<SOThrottleConstants.createdDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.createdByID>(graph.Accessinfo.UserID),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedByID>(graph.Accessinfo.UserID));

                PXDatabase.Insert<SOThrottleConstants>(
                    new PXDataFieldAssign<SOThrottleConstants.apiname>(SOConstants.apiGetReport),
                    new PXDataFieldAssign<SOThrottleConstants.delayTime>("20000"),
                    new PXDataFieldAssign<SOThrottleConstants.createdDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.createdByID>(graph.Accessinfo.UserID),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedByID>(graph.Accessinfo.UserID));

                PXDatabase.Insert<SOThrottleConstants>(
                    new PXDataFieldAssign<SOThrottleConstants.apiname>(SOConstants.apiFeedResultWaiting),
                    new PXDataFieldAssign<SOThrottleConstants.delayTime>("20000"),
                    new PXDataFieldAssign<SOThrottleConstants.createdDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedDateTime>(PXTimeZoneInfo.Now),
                    new PXDataFieldAssign<SOThrottleConstants.createdByID>(graph.Accessinfo.UserID),
                    new PXDataFieldAssign<SOThrottleConstants.lastModifiedByID>(graph.Accessinfo.UserID));
            }
            catch (Exception ex)
            {
                PXTrace.WriteInformation(ex.Message);
            }
        }
    }
}