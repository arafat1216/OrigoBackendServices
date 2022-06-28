using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents
{
    public class NoteChangedDomainEvent<T> : BaseEvent where T:Models.Asset
    {
        public T Asset { get; protected set; }
        public Guid CallerId { get; protected set; }
        public string PreviousNote { get; protected set; }

        public NoteChangedDomainEvent(T asset, Guid callerId, string previousNote) : base(asset.ExternalId)
        {
            Asset = asset;
            CallerId = callerId;
            PreviousNote = previousNote;
        }

        public override string EventMessage()
        {
            return $"Asset note changed from '{PreviousNote}' to 'Asset.Note'.";
        }
    }
}
