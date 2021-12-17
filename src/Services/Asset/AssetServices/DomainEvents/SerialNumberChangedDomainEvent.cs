using AssetServices.Models;
using Common.Logging;
using System;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class SerialNumberChangedDomainEvent<T> : BaseEvent where T: HardwareAsset
    {
        public T Asset { get; protected set; }
        public Guid CallerId { get; protected set; }
        public string PreviousSerialNumber { get; protected set; }

        public SerialNumberChangedDomainEvent(T asset, Guid callerId, string previousSerialNumber) : base(asset.ExternalId)
        {
            Asset = asset;
            CallerId = callerId;
            PreviousSerialNumber = previousSerialNumber;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset changed serial number from {PreviousSerialNumber} to {Asset.SerialNumber}.";
        }
    }
}