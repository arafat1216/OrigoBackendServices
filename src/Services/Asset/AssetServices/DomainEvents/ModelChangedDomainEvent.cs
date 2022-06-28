using AssetServices.Models;
using Common.Logging;
using System;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class ModelChangedDomainEvent<T> : BaseEvent where T:Models.Asset
    {
        public T Asset { get; protected set; }
        public Guid CallerId { get; protected set; }
        public string PreviousModel { get; protected set; }

        public ModelChangedDomainEvent(T asset, Guid callerId, string previousModel) : base(asset.ExternalId)
        {
            Asset = asset;
            CallerId = callerId;
            PreviousModel = previousModel;
        }

        public override string EventMessage()
        {
            return $"Asset changed from {PreviousModel} to {Asset.ProductName}.";
        }
    }
}