using System;
using AssetServices.Models;
using Common.Logging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class PurchaseDateChangedDomainEvent : BaseEvent
    {
        public Asset Asset { get; protected set; }
        public DateTime PreviousPurchaseDate { get; protected set; }

        public PurchaseDateChangedDomainEvent(Asset asset, DateTime previousPurchaseDate) : base(asset.ExternalId)
        {
            Asset = asset;
            PreviousPurchaseDate = previousPurchaseDate;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset changed purchase date from {PreviousPurchaseDate:yyyy-MM-dd} to {Asset.PurchaseDate:yyyy-MM-dd}.";
        }
    }
}