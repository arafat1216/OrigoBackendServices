using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CustomerServices.Models
{
    public class AssetCategoryType : Entity
    {
        protected AssetCategoryType() { }

        public AssetCategoryType(Guid assetCategoryId, Guid customerId, IList<AssetCategoryLifecycleType> lifecycleTypes)
        {
            AssetCategoryId = assetCategoryId;
            ExternalCustomerId = customerId;
            LifecycleTypes = lifecycleTypes;
        }

        public Guid AssetCategoryId { get; protected set; }

        public Guid ExternalCustomerId { get; protected set; }

        public ICollection<AssetCategoryLifecycleType> LifecycleTypes { get; protected set; }

        public void UpdateCustomerId(Guid customerId)
        {
            ExternalCustomerId = customerId;
        }

        public void SetAssetCategoryId(Guid assetCategoryId)
        {
            AssetCategoryId = assetCategoryId;
        }

        public void SetLifecycleTypes(IList<AssetCategoryLifecycleType> lifecycleTypes)
        {
            LifecycleTypes = lifecycleTypes;
        }
    }
}
