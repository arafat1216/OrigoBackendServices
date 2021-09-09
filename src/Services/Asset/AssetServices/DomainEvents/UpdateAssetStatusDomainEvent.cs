﻿using AssetServices.Models;
using Common.Enums;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace AssetServices.DomainEvents
{
    public class UpdateAssetStatusDomainEvent : BaseEvent
    {
        public Asset Asset { get; protected set; }
        public AssetStatus PreviousStatus { get; protected set; }

        public UpdateAssetStatusDomainEvent(Asset asset, AssetStatus previousStatus) : base(asset.AssetId)
        {
            Asset = asset;
            PreviousStatus = previousStatus;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Status changed from {PreviousStatus} to {Asset.Status}.";
        }
    }
}