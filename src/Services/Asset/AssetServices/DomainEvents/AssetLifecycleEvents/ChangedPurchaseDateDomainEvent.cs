using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class ChangedPurchaseDateDomainEvent : BaseEvent
    {
        public ChangedPurchaseDateDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, DateTime previousPurchaseDate) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousPurchaseDate = previousPurchaseDate;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public DateTime PreviousPurchaseDate { get; protected set; }

        public override string EventMessage()
        {
            return $"Purchase date for Asset Life Cycle has changed from {PreviousPurchaseDate} to {AssetLifecycle.PurchaseDate}.";
        }
    }
}
