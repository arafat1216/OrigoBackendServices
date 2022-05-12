using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class AssetHasBeenStolenDomainEvent : BaseEvent
    {
        public AssetHasBeenStolenDomainEvent(AssetLifecycle assetLifecycle, AssetLifecycleStatus previousAssetLifecycleStatus, Guid callerId):base(assetLifecycle.ExternalId)
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
            return $"Asset life cycle reported as stolen. Status changed from {PreviousAssetLifecycleStatus} to {AssetLifecycle.AssetLifecycleStatus}.";
        }
    }
}