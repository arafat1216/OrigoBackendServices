using AssetServices.Models;
using Common.Enums;
using Common.Logging;
using System;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class SetAssetLifeCycleTypeDomainEvent<T> : BaseEvent where T:AssetLifecycle
    {
        public T AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public LifecycleType PreviousLifecycleType { get; protected set; }

        public SetAssetLifeCycleTypeDomainEvent(T assetLifecycle, Guid callerId, LifecycleType previousLifecycleType) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousLifecycleType = previousLifecycleType;
        }

        public override string EventMessage()
        {
            return $"Lifecycle changed from {PreviousLifecycleType} to {AssetLifecycle.AssetLifecycleType}.";
        }
    }
}