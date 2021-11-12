using AssetServices.Models;
using Common.Logging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class SerialNumberChangedDomainEvent : BaseEvent
    {
        public HardwareSuperType Asset { get; protected set; }
        public string PreviousSerialNumber { get; protected set; }

        public SerialNumberChangedDomainEvent(HardwareSuperType asset, string previousSerialNumber) : base(asset.ExternalId)
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