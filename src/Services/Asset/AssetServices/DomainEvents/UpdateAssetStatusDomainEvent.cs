using AssetServices.Models;
using Common.Enums;
using Common.Logging;
using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace AssetServices.DomainEvents
{
    public class UpdateAssetStatusDomainEvent<T> : BaseEvent where T:Asset
    {
        public T Asset { get; protected set; }
        public Guid CallerId { get; protected set; }
        public AssetStatus PreviousStatus { get; protected set; }

        public UpdateAssetStatusDomainEvent(T asset, Guid callerId, AssetStatus previousStatus) : base(asset.ExternalId)
        {
            Asset = asset;
            CallerId = callerId;
            PreviousStatus = previousStatus;
        }

        public override string EventMessage()
        {
            return $"Status changed from {PreviousStatus} to {Asset.Status}.";
        }
    }
}