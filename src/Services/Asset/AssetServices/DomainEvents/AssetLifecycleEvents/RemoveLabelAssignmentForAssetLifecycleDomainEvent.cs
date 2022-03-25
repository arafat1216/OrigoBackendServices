using System;
using AssetServices.Models;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class RemoveLabelAssignmentForAssetLifecycleDomainEvent : BaseEvent
    {
        public RemoveLabelAssignmentForAssetLifecycleDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, CustomerLabel? customerLabel):base(assetLifecycle.ExternalId)
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
            return $"Label {CustomerLabel?.Label.Text} removed from asset life cycle.";
        }
    }
}