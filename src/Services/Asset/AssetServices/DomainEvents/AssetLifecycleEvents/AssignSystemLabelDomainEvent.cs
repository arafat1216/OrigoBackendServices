using AssetServices.Models;
using Common.Logging;
using System;
using System.Text.Json.Serialization;

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class AssignSystemLabelDomainEvent : BaseEvent
    {
        public AssignSystemLabelDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, CustomerLabel customerLabel) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            CustomerLabel = customerLabel;
        }
        

        public AssetLifecycle AssetLifecycle { get; set; }
        public Guid CallerId { get; set; }
        public CustomerLabel CustomerLabel { get; set; }

        public override string EventMessage()
        {
            return $"System label {CustomerLabel?.Label.Text} added for asset life cycle.";
        }
    }
}
