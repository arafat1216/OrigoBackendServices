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
        private IList<AssetCategoryLifecycleType> lifecycleTypes;

        protected AssetCategoryType() { }

        public AssetCategoryType(Guid assetCategoryId, Guid customerId, IList<AssetCategoryLifecycleType> lifecycleTypes)
        {
            AssetCategoryId = assetCategoryId;
            ExternalCustomerId = customerId;
            LifecycleTypes = lifecycleTypes;
            AddDomainEvent(new AssetCategoryAddedDomainEvent(this));
        }

        public Guid AssetCategoryId { get; protected set; }

        public Guid ExternalCustomerId { get; protected set; }

        public ICollection<AssetCategoryLifecycleType> LifecycleTypes
        {
            get { return lifecycleTypes; }
            protected set { lifecycleTypes = value.ToList(); }
        }

        public void AddLifecyle(AssetCategoryLifecycleType lifecycleType)
        {
            lifecycleTypes.Add(lifecycleType);
        }

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
