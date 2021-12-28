using Common.Seedwork;
using System;
using System.Collections.Generic;

namespace CustomerServices.Models
{
    public class AssetCategoryType : Entity
    {
        protected AssetCategoryType() { }

        public AssetCategoryType(Guid assetCategoryId, Guid customerId, IList<AssetCategoryLifecycleType> lifecycleTypes, Guid callerId)
        {
            AssetCategoryId = assetCategoryId;
            ExternalCustomerId = customerId;
            LifecycleTypes = lifecycleTypes;
            CreatedBy = callerId;
        }

        public Guid AssetCategoryId { get; protected set; }

        public Guid ExternalCustomerId { get; protected set; }

        public ICollection<AssetCategoryLifecycleType> LifecycleTypes { get; protected set; }

        public void UpdateCustomerId(Guid customerId)
        {
            ExternalCustomerId = customerId;
        }

        public void SetAssetCategoryId(Guid assetCategoryId, Guid callerId)
        {
            UpdatedBy = callerId;
            AssetCategoryId = assetCategoryId;
        }

        public void SetLifecycleTypes(IList<AssetCategoryLifecycleType> lifecycleTypes)
        {
            LifecycleTypes = lifecycleTypes;
        }
        public void SetDeletedBy(Guid callerId)
        {
            DeletedBy  = callerId;
        }
    }
}
