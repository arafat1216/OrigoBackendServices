using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

namespace AssetServices.DomainEvents.EndOfLifeCycleEvents
{
    public class ConfirmReturnDeviceDomainEvent : BaseEvent
    {
        public ConfirmReturnDeviceDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, AssetLifecycleStatus previousAssetLifecycleStatus, string locationName, string description) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousAssetLifecycleStatus = previousAssetLifecycleStatus;
            LocationName = locationName;
            Description = description;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public AssetLifecycleStatus PreviousAssetLifecycleStatus { get; protected set; }
        public string LocationName { get; protected set; }
        public string Description { get; protected set; }

        public override string EventMessage()
        {
            return $"Asset Id: {AssetLifecycle.ExternalId} is successfully 'Returned' from '{PreviousAssetLifecycleStatus.ToString()}' By Caller Id: {CallerId}." +
                $"Return Location: {LocationName} - {Description}";
        }
    }
}
