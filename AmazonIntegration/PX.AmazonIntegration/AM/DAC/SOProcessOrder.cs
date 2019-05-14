using System;
using PX.Data;

namespace AmazonIntegration
{
    [Serializable]
    [PXCacheName(SOMessages.processOrder)]
    public class SOProcessOrder : IBqlTable
    {
        #region LineNumber

        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Mapping Field Id")]
        public virtual int? LineNumber { get; set; }
        public abstract class lineNumber : IBqlField { }

        #endregion

        #region ProcessID

        [PXDBInt()]
        [PXUIField(DisplayName = "Process ID")]
        public virtual int? ProcessID { get; set; }
        public abstract class processID : IBqlField { }

        #endregion

        #region Selected

        public abstract class selected : IBqlField
        {
        }
        protected bool? _Selected = false;
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
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

        #region IntegrationID

        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Integration ID")]
        [PXSelector(typeof(Search<SOAmazonSetup.integrationID>), typeof(SOAmazonSetup.integrationID), typeof(SOAmazonSetup.description))]
        public virtual string IntegrationID { get; set; }
        public abstract class integrationID : IBqlField { }

        #endregion

        #region AmazonOrderID

        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Amazon Order ID")]
        public virtual string AmazonOrderID { get; set; }
        public abstract class amazonOrderID : IBqlField { }

        #endregion

        #region AmazonStatus

        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Amazon Order Status")]
        public virtual string AmazonStatus { get; set; }
        public abstract class amazonStatus : IBqlField { }

        #endregion

        #region AmazonOrderDate

        [PXDBDateAndTime(UseTimeZone = false)]
        [PXUIField(DisplayName = "Amazon Order Date")]
        public virtual DateTime? AmazonOrderDate { get; set; }
        public abstract class amazonOrderDate : IBqlField { }

        #endregion

        #region AmazonOrderLastUpdated

        [PXDBDateAndTime(UseTimeZone = false)]
        [PXUIField(DisplayName = "Order Updated Date")]
        public virtual DateTime? AmazonOrderLastUpdated { get; set; }
        public abstract class amazonOrderLastUpdated : IBqlField { }

        #endregion

        #region SynDatetime

        [PXDBDateAndTime(UseTimeZone = false)]
        [PXUIField(DisplayName = "Prepare Sync Date")]
        public virtual DateTime? SynDatetime { get; set; }
        public abstract class synDatetime : IBqlField { }

        #endregion

        #region Buyer Email ID

        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Buyer Email ID")]
        public virtual string BuyerEmailID { get; set; }
        public abstract class buyerEmailID : IBqlField { }

        #endregion

        #region PrepareStatus

        [PXUIField(DisplayName = "PrepareStatus")]
        [PXDBBool]
        [PXDefault(false)]
        public virtual bool? PrepareStatus { get; set; }
        public abstract class prepareStatus : IBqlField { }

        #endregion

        #region ImportStatus

        [PXUIField(DisplayName = "ImportStatus")]
        [PXDBBool]
        public virtual bool? ImportStatus { get; set; }
        public abstract class importStatus : IBqlField { }

        #endregion       

        #region OrderAmount

        [PXUIField(DisplayName = "Order Amount")]
        [PXDBDecimal]
        public virtual decimal? OrderAmount { get; set; }
        public abstract class orderAmount : IBqlField { }

        #endregion

        #region AmazonTrackingNumber

        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tracking Number")]
        public virtual string AmazonTrackingNumber { get; set; }
        public abstract class amazonTrackingNumber : IBqlField { }

        #endregion

        #region AmazonTrackingNumber

        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Ship Via")]
        public virtual string Carrier { get; set; }
        public abstract class carrier : IBqlField { }

        #endregion

        #region OrderSchema

        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Order Schema")]
        public virtual string OrderSchema { get; set; }
        public abstract class orderSchema : IBqlField { }

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