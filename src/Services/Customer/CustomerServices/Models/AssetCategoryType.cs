using Common.Seedwork;
using CustomerServices.DomainEvents;
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
            AddDomainEvent(new AssetCategoryTypeCreatedDomainEvent(this));
        }

        public Guid AssetCategoryId { get; protected set; }

        public Guid ExternalCustomerId { get; protected set; }

        public ICollection<AssetCategoryLifecycleType> LifecycleTypes { get; protected set; }

        public void UpdateCustomerId(Guid customerId)
        {
            string oldCustomerId = ExternalCustomerId.ToString();
            ExternalCustomerId = customerId;
            AddDomainEvent(new AssetCategoryTypeUpdatedAssetCustomerIdDomainEvent(this, oldCustomerId));
        }

        public void SetAssetCategoryId(Guid assetCategoryId, Guid callerId)
        {
            string oldAssetCategoryId = AssetCategoryId.ToString();
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AssetCategoryId = assetCategoryId;
            AddDomainEvent(new AssetCategoryTypeUpdatedAssetCategoryIdDomainEvent(this, oldAssetCategoryId));
        }

        public void SetLifecycleTypes(IList<AssetCategoryLifecycleType> lifecycleTypes)
        {
            AddDomainEvent(new AssetCategoryTypeUpdatedLifecycleTypesDomainEvent(this));
            LifecycleTypes = lifecycleTypes;
        }
        public void SetDeletedBy(Guid callerId)
        {
            LastUpdatedDate = DateTime.UtcNow;
            DeletedBy  = callerId;
            AddDomainEvent(new AssetCategoryTypeDeletedDomainEvent(this));
        }
    }
}
