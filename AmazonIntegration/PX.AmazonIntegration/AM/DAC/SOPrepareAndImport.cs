using System;
using PX.Data;

namespace AmazonIntegration
{
    [Serializable]
    [PXCacheName(SOMessages.prepareAndImport)]

    public class SOPrepareAndImport : IBqlTable
    {
        #region Selected
        public abstract class selected : IBqlField
        {
        }
        protected bool? _Selected = false;
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected")]
        public bool? Selected
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

        #region LineNbr

        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr")]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : IBqlField { }

        #endregion

        #region LastSyncDate

        [PXDBDate()]
        [PXUIField(DisplayName = "Last Sync Date")]
        public virtual DateTime? LastSyncDate { get; set; }
        public abstract class lastSyncDate : IBqlField { }

        #endregion

        #region ToDate

        [PXDBDate()]
        [PXUIField(DisplayName = "To Date")]
        public virtual DateTime? ToDate { get; set; }
        public abstract class toDate : IBqlField { }

        #endregion

        #region ProcessID

        [PXDBInt()]
        [PXUIField(DisplayName = "Process ID")]
        public virtual int? ProcessID { get; set; }
        public abstract class processID : IBqlField { }

        #endregion

        #region IntegrationID

        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Integration ID")]
        public virtual string IntegrationID { get; set; }
        public abstract class integrationID : IBqlField { }

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

        [PXDBDate()]
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

        [PXDBDate()]
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