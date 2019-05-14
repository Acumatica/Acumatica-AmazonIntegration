using System;
using PX.Data;

namespace AmazonIntegration
{
    [Serializable]
    [PXCacheName(SOMessages.fieldMapping)]
    public class SOFieldMapping : IBqlTable
    {
        #region MappingFieldId

        [PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(SOAmazonSetup.mappingFieldIdCntr))]
        [PXUIField(DisplayName = "Mapping Field Id", Visible = false)]
        [PXParent(typeof(Select<SOAmazonSetup, Where<SOAmazonSetup.integrationID, Equal<Current<SOFieldMapping.integrationID>>>>))]
        public virtual int? MappingFieldId { get; set; }
        public abstract class mappingFieldId : IBqlField { }

        #endregion

        #region IntegrationID

        [PXDBString(30, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXDBDefault(typeof(SOAmazonSetup.integrationID))]
        [PXUIField(DisplayName = "Integration ID")]
        public virtual string IntegrationID { get; set; }
        public abstract class integrationID : IBqlField { }

        #endregion     

        #region IsActive

        [PXDBBool()]
        [PXUIField(DisplayName = "Is Active")]
        [PXDefault(true)]
        public virtual bool? IsActive { get; set; }
        public abstract class isActive : IBqlField { }

        #endregion

        #region SourceObject

        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Source Object", Required = true)]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        [PXDefault]
        public virtual string SourceObject { get; set; }
        public abstract class sourceObject : IBqlField { }

        #endregion

        #region SourceField

        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Source Field", Required = true)]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        [PXDefault]
        public virtual string SourceField { get; set; }
        public abstract class sourceField : IBqlField { }

        #endregion

        #region TargetObject

        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Target Object", Required = true)]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        [PXDefault]
        public virtual string TargetObject { get; set; }
        public abstract class targetObject : IBqlField { }

        #endregion

        #region TargetField

        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Target Field", Required = true)]
        [PXDefault]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        [PXCheckUnique(Where = typeof(Where<SOFieldMapping.integrationID, Equal<Current<SOFieldMapping.integrationID>>,
                                             And<SOFieldMapping.isActive, Equal<Current<SOFieldMapping.isActive>>,
                                             And<Where2<Where<SOFieldMapping.targetObject, Equal<Current<SOFieldMapping.targetObject>>,
                                                            And<SOFieldMapping.targetField, Equal<Current<SOFieldMapping.targetField>>>>,
                                                            Or<Where<SOFieldMapping.sourceObject, Equal<Current<SOFieldMapping.sourceObject>>,
                                                            And<SOFieldMapping.sourceField, Equal<Current<SOFieldMapping.sourceField>>,
                                                            And<SOFieldMapping.targetObject, Equal<Current<SOFieldMapping.targetObject>>,
                                                            And<SOFieldMapping.targetField, Equal<Current<SOFieldMapping.targetField>>>>>>>>>>>))]
        public virtual string TargetField { get; set; }
        public abstract class targetField : IBqlField { }

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