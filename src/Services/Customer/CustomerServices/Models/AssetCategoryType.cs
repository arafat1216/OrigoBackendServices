using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            AddDomainEvent(new AssetCategoryAddedDomainEvent(this));
        }

        public Guid AssetCategoryId { get; protected set; }

        public Guid ExternalCustomerId { get; protected set; }

        public IList<AssetCategoryLifecycleType> LifecycleTypes { get; protected set; }

        public void LogDelete()
        {
            AddDomainEvent(new AssetCategoryRemovedDomainEvent(this));
        }

        public void SetLifecycleTypes(IList<AssetCategoryLifecycleType> lifecycleTypes)
        {
            LifecycleTypes = lifecycleTypes;
        }
    }
}
