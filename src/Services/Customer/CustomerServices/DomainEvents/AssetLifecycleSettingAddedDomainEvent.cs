using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class AssetLifecycleSettingAddedDomainEvent : BaseEvent
    {
        public AssetLifecycleSettingAddedDomainEvent(AssetCategoryLifecycleType newLifecycle) : base(newLifecycle.CustomerId)
        {
            LifecycleType = newLifecycle;
        }

        public AssetCategoryLifecycleType LifecycleType { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset category lifecyle type {Id} added.";
        }
    }
}
