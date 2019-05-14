using System;
using PX.Data;

namespace AmazonIntegration
{
    [Serializable]
    [PXCacheName(SOMessages.processLog)]
    public class SOOrderProcessLog : IBqlTable
    {
        #region ProcessID

        [PXDBIdentity(IsKey = true)]        
        [PXUIField(DisplayName = "Process ID")]       
        public virtual int? ProcessID { get; set; }
        public abstract class processID : IBqlField { }

        #endregion 

        #region ParentProcessID

        [PXDBInt]
        [PXUIField(DisplayName = "Process ID")]
        public virtual int? ParentProcessID { get; set; }
        public abstract class parentProcessID : IBqlField { }

        #endregion  

        #region IntegrationID

        [PXDBString(30, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Integration ID")]
        [PXSelector(typeof(Search<SOAmazonSetup.integrationID>), typeof(SOAmazonSetup.integrationID), typeof(SOAmazonSetup.description), typeof(SOAmazonSetup.integrationType))]
        public virtual string IntegrationID { get; set; }
        public abstract class integrationID : IBqlField { }

        #endregion

        #region ProcessDate

        [PXDBDate(InputMask = "g", PreserveTime = true, UseTimeZone = false)]
        [PXUIField(DisplayName = "Process Date")]
        public virtual DateTime? ProcessDate { get; set; }
        public abstract class processDate : IBqlField { }

        #endregion

        #region Operation

        [PXDBString(20, IsUnicode=true)]
        [PXUIField(DisplayName = "Operation Name")]
        public virtual string Operation { get; set; }
        public abstract class operation : IBqlField { }

        #endregion

        #region TotalRecordstoImport

        [PXDBInt()]
        [PXUIField(DisplayName = "Total Records to Import")]
        public virtual int? TotalRecordstoImport { get; set; }
        public abstract class totalRecordstoImport : IBqlField { }

        #endregion

        #region ImportedRecordsCount

        [PXDBInt()]
        [PXUIField(DisplayName = "Total Imported Records")]
        public virtual int? ImportedRecordsCount { get; set; }
        public abstract class importedRecordsCount : IBqlField { }

        #endregion

        #region FailedRecordsCount

        [PXDBInt()]
        [PXUIField(DisplayName = "Total Records Failed")]
        public virtual int? FailedRecordsCount { get; set; }
        public abstract class failedRecordsCount : IBqlField { }

        #endregion

        #region Selected
        public abstract class selected : IBqlField
        {
        }
        protected bool? _Selected = false;
        [PXBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected
        {
            get
            {
                return _Selected;
            }
            set
            {
                _Selected = value;
            }
        }
        #endregion

        #region Tstamp

        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }

        #endregion

        #region CreatedByID

        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : IBqlField { }

        #endregion

        #region CreatedByScreenID

        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : IBqlField { }

        #endregion

        #region CreatedDateTime

        [PXDBCreatedDateTime()]
        [PXUIField(DisplayName = "Created Date Time")]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : IBqlField { }

        #endregion
    }
}