using AssetServices.Models;
using Common.Logging;

namespace AssetServices.DomainEvents
{
    public class TagUpdatedDomainEvent : BaseEvent
    {
        public Asset Asset { get; protected set; }
        public string PreviousTag { get; protected set; }

        public TagUpdatedDomainEvent(Asset asset, string previousTag) : base(asset.ExternalId)
        {
            Asset = asset;
            PreviousTag = previousTag;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset tag changed from {PreviousTag} to {Asset.Note}.";
        }
    }
}
