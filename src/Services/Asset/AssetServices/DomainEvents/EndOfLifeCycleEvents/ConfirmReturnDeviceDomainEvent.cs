using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

namespace AssetServices.DomainEvents.EndOfLifeCycleEvents
{
    public class ConfirmReturnDeviceDomainEvent : BaseEvent
    {
        public ConfirmReturnDeviceDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, AssetLifecycleStatus previousAssetLifecycleStatus) : base(assetLifecycle.ExternalId)
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
            return $"Asset Id: {AssetLifecycle.ExternalId} is successfully 'Returned' from '{PreviousAssetLifecycleStatus.ToString()}' By Caller Id: {CallerId}";
        }
    }
}
