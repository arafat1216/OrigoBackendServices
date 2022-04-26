using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class AssignSystemLabelDomainEvent : BaseEvent
    {
        public AssignSystemLabelDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, CustomerLabel? customerLabel) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            CustomerLabel = customerLabel;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public CustomerLabel? CustomerLabel { get; protected set; }

        public override string EventMessage()
        {
            return $"System label {CustomerLabel?.Label.Text} added for asset life cycle.";
        }
    }
}
