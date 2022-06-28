using AssetServices.Models;
using Common.Logging;
using System;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class BrandChangedDomainEvent<T> : BaseEvent where T:Models.Asset
    {
        public T Asset { get; protected set; }
        public Guid CallerId { get; protected set; }
        public string PreviousBrand { get; protected set; }

        public BrandChangedDomainEvent(T asset, Guid callerId, string previousBrand) : base(asset.ExternalId)
        {
            Asset = asset;
            CallerId = callerId;
            PreviousBrand = previousBrand;
        }

        public override string EventMessage()
        {
            return $"Brand changed from {PreviousBrand} to {Asset.Brand}.";
        }
    }
}