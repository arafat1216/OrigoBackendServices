using System;
using AssetServices.Models;
using Common.Logging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class PurchaseDateChangedDomainEvent<T> : BaseEvent where T:Asset
    {
        public T Asset { get; protected set; }
        public Guid CallerId { get; protected set; }
        public DateTime PreviousPurchaseDate { get; protected set; }

        public PurchaseDateChangedDomainEvent(T asset, Guid callerId, DateTime previousPurchaseDate) : base(asset.ExternalId)
        {
            Asset = asset;
            CallerId = callerId;
            PreviousPurchaseDate = previousPurchaseDate;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset changed purchase date from {PreviousPurchaseDate:yyyy-MM-dd} to {Asset.PurchaseDate:yyyy-MM-dd}.";
        }
    }
}