using Common.Enums;
using Common.Seedwork;
using System;

namespace CustomerServices.Models
{
    public class AssetCategoryLifecycleType : Entity
    {
        /// <summary>
        /// Create a connection between a customer and the lifecycle types enabled for their asset categories.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="assetCategoryId"></param>
        /// <param name="lifeCycleTypeId"></param>
        public AssetCategoryLifecycleType(Guid customerId, Guid assetCategoryId, int lifecycle, Guid callerId)
        {
            AssetCategoryId = assetCategoryId;
            CustomerId = customerId;
            bool checkEnumValue = Enum.TryParse(lifecycle.ToString(), out LifecycleType lifecycleType);
            LifecycleType = checkEnumValue ? lifecycleType : LifecycleType.NoLifecycle;
            CreatedBy = callerId;
        }

        protected AssetCategoryLifecycleType() { }

        public Guid CustomerId { get; protected set; }

        public Guid AssetCategoryId { get; protected set; }

        public LifecycleType LifecycleType { get; protected set; }

        public void SetDeletedBy(Guid callerId) 
        {
            DeletedBy = callerId;
        }
        
    }
}
