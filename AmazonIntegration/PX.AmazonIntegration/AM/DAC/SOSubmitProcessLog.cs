using System;
using PX.Data;

namespace AmazonIntegration
{
    [Serializable]
    [PXCacheName(SOMessages.feedProcessLog)]
    public class SOSubmitProcessLog : IBqlTable
    {
        #region ProcessID

        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Process ID")]
        public virtual int? ProcessID { get; set; }
        public abstract class processID : IBqlField { }

        #endregion

        #region ProcessDate

        [PXDBDate(InputMask ="g", PreserveTime = true,UseTimeZone = false)]
        [PXUIField(DisplayName = "Process Date")]
        public virtual DateTime? ProcessDate { get; set; }
        public abstract class processDate : IBqlField { }

        #endregion

        #region TotalRecordstoProcess

        [PXDBInt()]
        [PXUIField(DisplayName = "Total Records to Process")]
        public virtual int? TotalRecordstoProcess { get; set; }
        public abstract class totalRecordstoProcess : IBqlField { }

        #endregion

        #region SubmitRecordsCount

        [PXDBInt()]
        [PXUIField(DisplayName = "Submit Records Count")]
        public virtual int? SubmitRecordsCount { get; set; }
        public abstract class submitRecordsCount : IBqlField { }

        #endregion

        #region SubmitFailedRecordsCount

        [PXDBInt()]
        [PXUIField(DisplayName = "Submit Failed Records Count")]
        public virtual int? SubmitFailedRecordsCount { get; set; }
        public abstract class submitFailedRecordsCount : IBqlField { }

        #endregion

        #region IntegrationID

        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Integration ID")]
        [PXSelector(typeof(Search<SOAmazonSetup.integrationID, Where<SOAmazonSetup.status, Equal<True>, And<SOAmazonSetup.integrationType, Equal<SOConstants.IntegrationType>>>>),
                    typeof(SOAmazonSetup.integrationID), typeof(SOAmazonSetup.description), typeof(SOAmazonSetup.integrationType))]
        public virtual string IntegrationID { get; set; }
        public abstract class integrationID : IBqlField { }

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

        #region LastModifiedByID

        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : IBqlField { }

        #endregion

        #region LastModifiedByScreenID

        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : IBqlField { }

        #endregion

        #region LastModifiedDateTime

        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : IBqlField { }

        #endregion

        #region Tstamp

        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }

        #endregion
    }
}
