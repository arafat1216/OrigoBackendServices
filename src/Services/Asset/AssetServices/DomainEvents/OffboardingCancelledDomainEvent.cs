using System;
using AssetServices.Models;
using Common.Logging;

namespace AssetServices.DomainEvents
{
    public class OffboardingCancelledDomainEvent : BaseEvent
    {
        public OffboardingCancelledDomainEvent(AssetLifecycle assetLifecycle, Guid callerId) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Employee Offboarding Cancelled for User Id: {AssetLifecycle.ContractHolderUser!.Id} owner of Asset Id: {AssetLifecycle.ExternalId}";
        }
    }
}
