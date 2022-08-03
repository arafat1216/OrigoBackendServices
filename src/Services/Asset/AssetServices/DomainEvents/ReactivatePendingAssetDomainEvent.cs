using System;
using AssetServices.Models;
using Common.Logging;

namespace AssetServices.DomainEvents
{
    public class ReactivatePendingAssetDomainEvent : BaseEvent
    {
        public ReactivatePendingAssetDomainEvent(AssetLifecycle assetLifecycle, Guid callerId) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Reactivating Asset from '{AssetLifecycle.AssetLifecycleStatus.ToString()}' by {CallerId}.";
        }
    }
}
