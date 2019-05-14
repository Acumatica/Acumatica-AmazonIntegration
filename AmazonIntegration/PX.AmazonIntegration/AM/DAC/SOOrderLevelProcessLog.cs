using System;
using PX.Data;

namespace AmazonIntegration
{
    [Serializable]
    [PXCacheName(SOMessages.importlog)]
    public class SOOrderLevelProcessLog : IBqlTable
    {
        #region LineNumber

        [PXDBIdentity(IsKey =true)]
        [PXUIField(DisplayName = "Line Nbr.")]       
        public virtual int? LineNumber { get; set; }
        public abstract class lineNumber : IBqlField { }

        #endregion

        #region IntegrationID

        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Integration ID")]
        [PXSelector(typeof(Search<SOAmazonSetup.integrationID>), typeof(SOAmazonSetup.integrationID), typeof(SOAmazonSetup.description), typeof(SOAmazonSetup.integrationType))]
        public virtual string IntegrationID { get; set; }
        public abstract class integrationID : IBqlField { }

        #endregion

        #region ProcessID       
        
        [PXDBInt()]
        [PXUIField(DisplayName = "Process ID")]
        [PXDBDefault(typeof(SOOrderProcessLog.processID))]
        [PXParent(typeof(Select<SOOrderProcessLog, Where<SOOrderProcessLog.parentProcessID, Equal<Current<SOOrderLevelProcessLog.processID>>>>))]
        public virtual int? ProcessID { get; set; }
        public abstract class processID : IBqlField { }

        #endregion

        #region AmazonOrderID

        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Amazon Order ID")]
        public virtual string AmazonOrderID { get; set; }
        public abstract class amazonOrderID : IBqlField { }

        #endregion

        #region AcumaticaOrderType

        [PXDBString(2, IsUnicode = false, IsFixed = true, InputMask = ">aa")]
        [PXUIField(DisplayName = "Order Type")]
        public virtual string AcumaticaOrderType { get; set; }
        public abstract class acumaticaOrderType : IBqlField { }

        #endregion

        #region AcumaticaOrderID

        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acumatica Order ID")]
        public virtual string AcumaticaOrderID { get; set; }
        public abstract class acumaticaOrderID : IBqlField { }

        #endregion

        #region ImportStatus

        [PXDBBool()]
        [PXUIField(DisplayName = "Import Status")]
        public virtual bool? ImportStatus { get; set; }
        public abstract class importStatus : IBqlField { }

        #endregion

        #region ErrorDesc

        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Error Desc")]
        public virtual string ErrorDesc { get; set; }
        public abstract class errorDesc : IBqlField { }

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


        [PXDBCreatedDateTime(InputMask = "g", PreserveTime = true, UseTimeZone = false)]        
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