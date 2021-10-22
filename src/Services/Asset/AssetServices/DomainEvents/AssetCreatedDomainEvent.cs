using AssetServices.Models;
using Common.Logging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class AssetCreatedDomainEvent : BaseEvent
    {
        public AssetCreatedDomainEvent(Asset asset) : base(asset.AssetId)
        {
            Asset = asset;
        }

        public Asset Asset { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset {Id} created.";
        }
    }
}