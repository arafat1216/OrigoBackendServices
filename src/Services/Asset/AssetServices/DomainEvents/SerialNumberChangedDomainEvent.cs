using AssetServices.Models;
using Common.Logging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class SerialNumberChangedDomainEvent : BaseEvent
    {
        public Asset Asset { get; protected set; }
        public string PreviousSerialNumber { get; protected set; }

        public SerialNumberChangedDomainEvent(Asset asset, string previousSerialNumber) : base(asset.AssetId)
        {
            Asset = asset;
            PreviousSerialNumber = previousSerialNumber;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset changed serial number from {PreviousSerialNumber} to {Asset.SerialNumber}.";
        }
    }
}