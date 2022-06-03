using AssetServices.Models;
using Common.Enums;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class AssetRepairCompletedDomainEvent : BaseEvent
    {
        public AssetRepairCompletedDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, AssetLifecycleStatus previousAssetLifecycleStatus)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousAssetLifecycleStatus = previousAssetLifecycleStatus;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public AssetLifecycleStatus PreviousAssetLifecycleStatus { get; protected set; }
        public override string EventMessage()
        {
            return $"Repair is completed for asset life cycle {AssetLifecycle.Id}. Status changed from {PreviousAssetLifecycleStatus} to {AssetLifecycle.AssetLifecycleStatus}.";
        }
    }
}
