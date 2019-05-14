using System;
using PX.Data;

namespace AmazonIntegration
{
    [Serializable]
    [PXCacheName(SOMessages.feedProcessLog)]
    public class SOSubmitDetailedProcessLog : IBqlTable
    {
        #region LineNumber

        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Line Number")]
        public virtual int? LineNumber { get; set; }
        public abstract class lineNumber : IBqlField { }

        #endregion

        #region ProcessID

        [PXDBInt()]
        [PXUIField(DisplayName = "Process ID")]
        [PXDBDefault(typeof(SOSubmitProcessLog.processID))]
        [PXParent(typeof(Select<SOSubmitProcessLog, Where<SOSubmitProcessLog.processID, Equal<Current<SOSubmitDetailedProcessLog.processID>>>>))]
        public virtual int? ProcessID { get; set; }
        public abstract class processID : IBqlField { }

        #endregion

        #region IntegrationID

        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Integration ID")]
        [PXSelector(typeof(Search<SOAmazonSetup.integrationID, Where<SOAmazonSetup.status, Equal<True>, And<SOAmazonSetup.integrationType, Equal<SOConstants.IntegrationType>>>>),
                    typeof(SOAmazonSetup.integrationID), typeof(SOAmazonSetup.description), typeof(SOAmazonSetup.integrationType))]
        public virtual string IntegrationID { get; set; }
        public abstract class integrationID : IBqlField { }

        #endregion

        #region AmazonOrderID

        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Amazon Order ID")]
        public virtual string AmazonOrderID { get; set; }
        public abstract class amazonOrderID : IBqlField { }

        #endregion

        #region AcumaticaOrderType

        [PXDBString(2, IsUnicode = false, IsFixed =true, InputMask = ">aa")]
        [PXUIField(DisplayName = "Order Type")]
        public virtual string AcumaticaOrderType { get; set; }
        public abstract class acumaticaOrderType : IBqlField { }

        #endregion

        #region AcumaticaOrderNbr

        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acumatica Order Nbr")]
        public virtual string AcumaticaOrderNbr { get; set; }
        public abstract class acumaticaOrderNbr : IBqlField { }

        #endregion

        #region AcumaticaShipmentType

        [PXDBString(2, IsUnicode = false, IsFixed =true, InputMask = ">aa")]
        [PXUIField(DisplayName = "Shipment Type")]
        public virtual string AcumaticaShipmentType { get; set; }
        public abstract class acumaticaShipmentType : IBqlField { }

        #endregion

        #region AcumaticaShipmentNbr

        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acumatica Shipment Nbr.")]
        public virtual string AcumaticaShipmentNbr { get; set; }
        public abstract class acumaticaShipmentNbr : IBqlField { }

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

        #region XMLResponse

        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "XMLResponse")]
        public virtual string XMLResponse { get; set; }
        public abstract class xMLResponse : IBqlField { }

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