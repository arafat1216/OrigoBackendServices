using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class ChangedAliasDomainEvent : BaseEvent
    {
        public ChangedAliasDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, string previousAlias) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousAlias = previousAlias;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public string PreviousAlias { get; protected set; }
        public User? PreviousContractHolder { get; protected set; }

        public override string EventMessage()
        {
            return $"Alias for Asset Life Cycle has changed from {PreviousAlias} to {AssetLifecycle.Alias}.";
        }
    }
}
