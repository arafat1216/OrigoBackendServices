using AssetServices.Models;
using Common.Logging;
using System;
using System.Text.Json.Serialization;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class AssetCreatedDomainEvent<T> : BaseEvent where T:Asset
    {
        public AssetCreatedDomainEvent()
        { }
        public AssetCreatedDomainEvent(T asset, string assetIdentifier, Guid callerId) : base(asset.ExternalId)
        {
            Asset = asset;
            AssetIdentifier = assetIdentifier;
            CallerId = callerId;
        }

        public T Asset { get; set; }
        // MobilePhone: Imeis[0] (if not empty), Tablet: SerialNumber (if not empty), Other: Asset.ExternalId
        public string AssetIdentifier { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset created with {AssetIdentifier}.";
        }
    }
}