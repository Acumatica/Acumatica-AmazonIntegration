using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;

namespace AmazonIntegration
{
    public class SOSubmitClearLog : PXGraph<SOSubmitClearLog>
    {
        #region Select

        public PXCancel<INFilter> Cancel;
        public PXFilter<INFilter> Filter;
        [PXFilterable]
        public PXFilteredProcessing<SOSubmitProcessLog, INFilter, Where<SOSubmitProcessLog.integrationID, Equal<Current<INFilter.integrationID>>,
                                                                  And<SOSubmitProcessLog.processDate, Between<Current<INFilter.startDate>, Current<INFilter.endDate>>>>> Log;
        // This DummyDetailedLog view, we are using for cascading deletion using PXParent.
        public PXSelect<SOSubmitDetailedProcessLog> DummyDetailedLog;

        #endregion
        public SOSubmitClearLog()
        {
            Log.SetProcessCaption(SOConstants.btnClearLog);
            Log.SetProcessAllCaption(SOConstants.btnClearAllLog);
            Log.SetProcessDelegate(
                delegate (List<SOSubmitProcessLog> list)
                {
                    SubmitClearLogRecords(list);
                });
        }
        protected virtual IEnumerable log()
        {
            if (Filter.Current != null)
            {
                PXSelectBase<SOSubmitProcessLog> logItem = new PXSelect<SOSubmitProcessLog, Where<SOSubmitProcessLog.integrationID, Equal<Required<SOFilter.integrationID>>,
                                                           And<SOSubmitProcessLog.processDate, Between<Required<SOFilter.startDate>, Required<SOFilter.endDate>>>>>(this);
                return logItem.Select(Filter.Current.IntegrationID, Filter.Current.StartDate, Convert.ToDateTime(Filter.Current.EndDate).AddDays(1));
            }
            return null;
        }
        public static void SubmitClearLogRecords(List<SOSubmitProcessLog> listOfSubmitLog)
        {
            SOSubmitClearLog graph = PXGraph.CreateInstance<SOSubmitClearLog>();
            graph.SubmitClearLogAmazonRecords(graph, listOfSubmitLog);
        }
        public void SubmitClearLogAmazonRecords(SOSubmitClearLog graph, List<SOSubmitProcessLog> listOfSubmitLog)
        {
            foreach (SOSubmitProcessLog lg in listOfSubmitLog)
            {
                try
                {
                    if (lg.Selected == true)
                    {
                        graph.Log.Cache.Delete(lg);
                        PXProcessing<SOSubmitProcessLog>.SetInfo(listOfSubmitLog.IndexOf(lg), SOMessages.logRecordDeleted);
                    }
                }
                catch (Exception ex)
                {
                    PXProcessing<SOSubmitProcessLog>.SetError(listOfSubmitLog.IndexOf(lg), ex.Message);
                }
            }
            graph.Actions.PressSave();
        }
    }

    [Serializable()]
    public partial class INFilter : IBqlTable
    {
        #region StartDate
        public abstract class startDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _StartDate;
        [PXDBDate()]
        [PXUIField(DisplayName = "Start Date", Visibility = PXUIVisibility.SelectorVisible, Required = false)]
        [PXDefault()]
        public virtual DateTime? StartDate
        {
            get
            {
                return this._StartDate;
            }
            set
            {
                this._StartDate = value;
            }
        }
        #endregion

        #region EndDate
        public abstract class endDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _EndDate;
        [PXDBDate()]
        [PXUIField(DisplayName = "End Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? EndDate
        {
            get
            {
                return this._EndDate;
            }
            set
            {
                this._EndDate = value;
            }
        }
        #endregion

        #region IntegrationID

        [PXString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Integration ID")]
        [PXSelector(typeof(Search<SOAmazonSetup.integrationID, Where<SOAmazonSetup.status, Equal<True>,
                                   And<SOAmazonSetup.integrationType, Equal<SOConstants.IntegrationType>>>>), typeof(SOAmazonSetup.integrationID), typeof(SOAmazonSetup.description), typeof(SOAmazonSetup.integrationType))]
        public string IntegrationID { get; set; }
        public class integrationID : IBqlField { }

        #endregion
    }
}