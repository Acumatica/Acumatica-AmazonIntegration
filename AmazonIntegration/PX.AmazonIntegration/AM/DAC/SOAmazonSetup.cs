using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.SO;

namespace AmazonIntegration
{
    [Serializable]
    [PXCacheName(SOMessages.marketplaceConfiguration)]
    public class SOAmazonSetup : IBqlTable
    {
        #region IntegrationID

        [PXDBString(30, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXDefault()]
        [PXUIField(DisplayName = "Integration ID", Required = true)]
        [PXSelector(typeof(Search<SOAmazonSetup.integrationID>), typeof(SOAmazonSetup.integrationID), typeof(SOAmazonSetup.description), typeof(SOAmazonSetup.integrationType))]
        public virtual string IntegrationID { get; set; }
        public abstract class integrationID : IBqlField { }

        #endregion

        #region Status

        [PXDBBool()]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Status { get; set; }
        public abstract class status : IBqlField { }

        #endregion

        #region IntegrationType

        [PXDBString(5, IsFixed = true, IsUnicode = false, InputMask = "")]
        [PXUIField(DisplayName = "Integration Type", Required = true)]
        [SOConstants.AMIntegrationType.List()]
        [PXDefault(SOConstants.AMIntegrationType.FBA)]
        [PXCheckUnique(Where = typeof(Where<SOAmazonSetup.integrationType, Equal<Current<SOAmazonSetup.integrationType>>,
                                     And<SOAmazonSetup.status, Equal<True>,
                                     And<SOAmazonSetup.sellerId, Equal<Current<SOAmazonSetup.sellerId>>>>>))]
        public virtual string IntegrationType { get; set; }
        public abstract class integrationType : IBqlField { }

        #endregion

        #region Description

        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : IBqlField { }

        #endregion

        #region OrderType

        [PXDBString(2, IsFixed = true, IsUnicode = false, InputMask = ">aa")]
        [PXDefault()]
        [PXUIField(DisplayName = "Order Type")]
        [OrderTypeSelector(typeof(SOOrderType.orderType), IsDirty = false)]
        public virtual string OrderType { get; set; }
        public abstract class orderType : IBqlField { }

        #endregion

        #region CustID

        [CustomerActive(DescriptionField = typeof(Customer.acctName), DisplayName = "Customer ID")]
        [PXDefault()]
        public virtual int? GuestCustID { get; set; }
        public abstract class guestCustID : IBqlField { }

        #endregion

        #region Default Currency

        [PXString]
        [PXUIField(DisplayName = "Default Currency")]
        [PXDBScalar(typeof(Search<
            Customer.curyID,
            Where<Customer.bAccountID, Equal<SOAmazonSetup.guestCustID>>>))]
        public string DefaultCurrency { get; set; }
        public abstract class defaultCurrency : PX.Data.BQL.BqlString.Field<defaultCurrency> { }

        #endregion

        #region DfltWarehouseID

        public abstract class dfltWarehouseID : IBqlField { }
        [PXUIField(DisplayName = "Default Warehouse")]
        [Site]
        [PXDefault()]
        public virtual int? DfltWarehouseID { get; set; }

        #endregion

        #region SellerId

        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXDefault()]
        [PXUIField(DisplayName = "Seller ID")]
        public virtual string SellerId { get; set; }
        public abstract class sellerId : IBqlField { }

        #endregion

        #region AuthToken

        [PXRSACryptString(IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Auth Token")]
        public virtual string AuthToken { get; set; }
        public abstract class authToken : IBqlField { }

        #endregion

        #region AccessKey

        [PXRSACryptString(IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Access Key")]
        public virtual string AccessKey { get; set; }
        public abstract class accessKey : IBqlField { }

        #endregion

        #region SecretKey

        [PXRSACryptString(IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Secret Key")]
        public virtual string SecretKey { get; set; }
        public abstract class secretKey : IBqlField { }

        #endregion

        #region MarketplaceId

        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXDefault()]
        [PXUIField(DisplayName = "Marketplace ID")]
        public virtual string MarketplaceId { get; set; }
        public abstract class marketplaceId : IBqlField { }

        #endregion

        #region MappingFieldIdCntr

        public abstract class mappingFieldIdCntr : IBqlField { }
        [PXDBInt()]
        [PXDefault(0)]
        public virtual int? MappingFieldIdCntr { get; set; }

        #endregion

        #region UsrSyncNote

        [PXString(200, IsUnicode = true)]
        [PXUIField(DisplayName = "Note", IsReadOnly = true, Visible = false)]
        [PXFormula(typeof(Switch<Case<Where<SOAmazonSetup.integrationID, IsNotNull>, SOConstants.FBMMessage>, StringEmpty>))]
        public virtual string FBMNote { get; set; }
        public abstract class fBMNote : IBqlField { }

        #endregion

        #region NoteID

        [PXNote(ShowInReferenceSelector = true)]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }

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

