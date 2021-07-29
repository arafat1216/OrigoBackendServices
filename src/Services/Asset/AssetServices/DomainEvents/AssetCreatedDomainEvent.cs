using AssetServices.Models;
using Common.Logging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class AssetCreatedDomainEvent : BaseEvent
    {
        public AssetCreatedDomainEvent(Asset newAsset) : base(newAsset.AssetId)
        {
            NewAsset = newAsset;
        }

        public Asset NewAsset { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset {Id} created.";
        }
    }
}