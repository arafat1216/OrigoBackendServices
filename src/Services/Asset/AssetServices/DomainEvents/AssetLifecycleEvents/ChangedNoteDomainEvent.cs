using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class ChangedNoteDomainEvent : BaseEvent
    {
        public ChangedNoteDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, string previousNote) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousNote = previousNote;
        }
        
        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public string PreviousNote { get; protected set; }

        public override string EventMessage()
        {
            return $"Note for Asset Life Cycle has changed from {PreviousNote} to {AssetLifecycle.Note}.";
        }
    }
}
