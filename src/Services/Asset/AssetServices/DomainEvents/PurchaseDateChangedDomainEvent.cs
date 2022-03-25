using System;
using AssetServices.Models;
using Common.Logging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class PurchaseDateChangedDomainEvent<T> : BaseEvent where T:AssetLifecycle
    {
        public T AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public DateTime PreviousPurchaseDate { get; protected set; }

        public PurchaseDateChangedDomainEvent(T assetLifecycle, Guid callerId, DateTime previousPurchaseDate) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousPurchaseDate = previousPurchaseDate;
        }

        public override string EventMessage()
        {
            return $"Asset changed purchase date from {PreviousPurchaseDate:yyyy-MM-dd} to {AssetLifecycle.PurchaseDate:yyyy-MM-dd}.";
        }
    }
}