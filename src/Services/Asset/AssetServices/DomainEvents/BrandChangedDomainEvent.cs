﻿using AssetServices.Models;
using Common.Logging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class BrandChangedDomainEvent : BaseEvent
    {
        public Asset Asset { get; protected set; }
        public string PreviousBrand { get; protected set; }

        public BrandChangedDomainEvent(Asset asset, string previousBrand) : base(asset.AssetId)
        {
            Asset = asset;
            PreviousBrand = previousBrand;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Brand changed from {PreviousBrand} to {Asset.Brand}.";
        }
    }
}