using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

namespace AssetServices.DomainEvents.EndOfLifeCycleEvents
{
    public class MakeAssetExpiredDomainEvent : BaseEvent
    {
        public MakeAssetExpiredDomainEvent(AssetLifecycle assetLifecycle, Guid callerId) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Asset Life Cycle has made 'Expired' from {AssetLifecycle.AssetLifecycleStatus} by caller id {CallerId} ";
        }
    }
}
