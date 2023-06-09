﻿using AssetServices.Models;
using Common.Logging;
using System;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AssetServices.DomainEvents
{
    public class AssetCreatedDomainEvent<T> : BaseEvent where T:Models.Asset
    {
        public AssetCreatedDomainEvent()
        { }
        public AssetCreatedDomainEvent(T asset, Guid callerId, string text) : base(asset.ExternalId)
        {
            Asset = asset;
            CallerId = callerId;
            Text = text;
        }

        public T Asset { get; set; }
        public Guid CallerId { get; set; }
        public string Text { get; set; }

        public override string EventMessage()
        {
            return $"{Asset.ProductName} created with {Text}";
        }
    }
}