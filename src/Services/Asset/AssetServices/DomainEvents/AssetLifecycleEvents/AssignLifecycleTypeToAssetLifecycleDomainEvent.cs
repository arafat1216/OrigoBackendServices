using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class AssignLifecycleTypeToAssetLifecycleDomainEvent : BaseEvent
    {
        public AssignLifecycleTypeToAssetLifecycleDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, LifecycleType lifecycleType):base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            LifecycleType = lifecycleType;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public LifecycleType LifecycleType { get; protected set; }

        public override string EventMessage()
        {
            return $"Asset life cycle type {LifecycleType} assigned to asset life cycle.";
        }
    }
}