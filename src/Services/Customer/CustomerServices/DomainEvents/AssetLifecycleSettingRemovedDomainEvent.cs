using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{

    class AssetLifecycleSettingRemovedDomainEvent : BaseEvent
    {
        public AssetLifecycleSettingRemovedDomainEvent(AssetCategoryLifecycleType removedLifecycle) : base(removedLifecycle.CustomerId)
        {
            LifecycleType = removedLifecycle;
        }

        public AssetCategoryLifecycleType LifecycleType { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset category lifecyle type {Id} removed.";
        }
    }
}
