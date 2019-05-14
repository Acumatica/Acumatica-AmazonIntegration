using System;
using PX.Data;

namespace AmazonIntegration
{
  [Serializable]
  [PXCacheName(SOMessages.throttleConstants)]
  public class SOThrottleConstants: IBqlTable
  {
		#region Apiname

		[PXDBString(50, IsKey =true, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "API Name")]
		public virtual string Apiname { get; set; }
		public abstract class apiname : IBqlField{}

		#endregion

		#region DelayTime

		[PXDBInt()]
		[PXUIField(DisplayName = "Delay Time")]
        public virtual int DelayTime { get; set; }
        public abstract class delayTime : IBqlField { }

        #endregion

        #region CreatedByID

        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : IBqlField { }

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