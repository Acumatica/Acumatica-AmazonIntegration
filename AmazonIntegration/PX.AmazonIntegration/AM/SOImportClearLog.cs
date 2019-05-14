using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;

namespace AmazonIntegration
{
    public class SOImportClearLog : PXGraph<SOImportClearLog>
    { 
        #region Select
        public PXCancel<SOFilter> Cancel;
        public PXFilter<SOFilter> Filter;
        [PXFilterable]
        public PXFilteredProcessing<SOOrderProcessLog, SOFilter, Where<SOOrderProcessLog.integrationID, Equal<Current<SOFilter.integrationID>>,
                                                  And<SOOrderProcessLog.processDate, Between<Current<SOFilter.startDate>, Current<SOFilter.endDate>>>>> Log;
        //This view we are using for cascading deletion using PXParent Attribute.
        public PXSelect<SOOrderLevelProcessLog> DummyDetailedLog;

        #endregion

        protected virtual IEnumerable log()
        {
            if (Filter.Current != null)
            {
                PXSelectBase<SOOrderProcessLog> soLog = new PXSelect<SOOrderProcessLog, Where<SOOrderProcessLog.integrationID, Equal<Required<SOFilter.integrationID>>,
                                                             And<SOOrderProcessLog.processDate, Between<Required<SOFilter.startDate>, Required<SOFilter.endDate>>>>>
                                                            (this);
                return soLog.Select(Filter.Current.IntegrationID, Filter.Current.StartDate, Convert.ToDateTime(Filter.Current.EndDate).AddDays(1));
            }
            return null;
        }

        public SOImportClearLog()
        {
            Log.SetProcessCaption(SOConstants.btnClearLog);
            Log.SetProcessAllCaption(SOConstants.btnClearAllLog);
            Log.SetProcessDelegate(
                delegate (List<SOOrderProcessLog> list)
                {
                    ImportClearLogRecords(list);
                });
        }

        public static void ImportClearLogRecords(List<SOOrderProcessLog> listOfImportLog)
        {
            SOImportClearLog graph = PXGraph.CreateInstance<SOImportClearLog>();
            graph.ImportClearLogAmazonRecords(graph, listOfImportLog);
        }

        public void ImportClearLogAmazonRecords(SOImportClearLog graph, List<SOOrderProcessLog> listOfImportLog)
        {
            foreach (SOOrderProcessLog lg in listOfImportLog)
            {
                try
                {
                    if (lg.Selected == true)
                    {                      
                        graph.Log.Cache.Delete(lg);
                        PXProcessing<SOOrderProcessLog>.SetInfo(listOfImportLog.IndexOf(lg), SOMessages.logRecordDeleted);
                    }
                }
                catch (Exception ex)
                {
                    PXProcessing<SOOrderProcessLog>.SetError(listOfImportLog.IndexOf(lg), ex.Message);
                }
            }
            graph.Actions.PressSave();
        }      
    }

    [Serializable()]
    public partial class SOFilter : IBqlTable
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
        [PXSelector(typeof(Search<SOAmazonSetup.integrationID, Where<SOAmazonSetup.status, Equal<True>>>),
                    typeof(SOAmazonSetup.integrationID), typeof(SOAmazonSetup.description), typeof(SOAmazonSetup.integrationType))]
        public string IntegrationID { get; set; }
        public class integrationID : IBqlField { }

        #endregion
    }
}