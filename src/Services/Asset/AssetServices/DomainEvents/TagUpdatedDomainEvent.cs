using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents
{
    public class TagUpdatedDomainEvent<T> : BaseEvent where T:Asset
    {
        public T Asset { get; protected set; }
        public Guid CallerId { get; protected set; }
        public string PreviousTag { get; protected set; }

        public TagUpdatedDomainEvent(T asset, Guid callerId, string previousTag) : base(asset.ExternalId)
        {
            Asset = asset;
            CallerId = callerId;
            PreviousTag = previousTag;
        }

        public override string EventMessage()
        {
            return $"Asset tag changed from {PreviousTag} to {Asset.Note}.";
        }
    }
}
