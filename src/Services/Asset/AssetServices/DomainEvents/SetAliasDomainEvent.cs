using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents
{
    public class SetAliasDomainEvent<T> : BaseEvent where T: Asset
    {
        public T Asset { get; protected set; }
        public Guid CallerId { get; protected set; }
        public string PreviousAlias { get; protected set; }

        public SetAliasDomainEvent(T asset, Guid callerId, string previousAlias) : base(asset.ExternalId)
        {
            Asset = asset;
            CallerId = callerId;
            PreviousAlias = previousAlias;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset alias changed from {PreviousAlias} to {Asset.Alias}.";
        }
    }
}
