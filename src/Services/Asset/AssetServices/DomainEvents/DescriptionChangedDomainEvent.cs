using AssetServices.Models;
using Common.Logging;

namespace AssetServices.DomainEvents
{
    public class DescriptionChangedDomainEvent : BaseEvent
    {
        public Asset Asset { get; protected set; }
        public string PreviousTag { get; protected set; }

        public DescriptionChangedDomainEvent(Asset asset, string previousTag) : base(asset.ExternalId)
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
