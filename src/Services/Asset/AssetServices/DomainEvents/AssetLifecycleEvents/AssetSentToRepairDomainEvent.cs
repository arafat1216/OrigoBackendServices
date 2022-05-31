using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class AssetSentToRepairDomainEvent : BaseEvent
    {
        public AssetSentToRepairDomainEvent(AssetLifecycle assetLifecycle, AssetLifecycleStatus previousAssetLifecycleStatus, Guid callerId) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            PreviousAssetLifecycleStatus = previousAssetLifecycleStatus;
            CallerId = callerId;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public AssetLifecycleStatus PreviousAssetLifecycleStatus { get; protected set; }

        public override string EventMessage()
        {
            return $"Asset life cycle registered as sent to repair. Status changed from {PreviousAssetLifecycleStatus} to {AssetLifecycle.AssetLifecycleStatus}.";
        }
    }
}
