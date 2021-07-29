using MediatR;
using System;
using AssetServices.Models;
using Common.Logging;

namespace AssetServices.DomainEvents
{
    public class AssetCreatedDomainEvent : BaseEvent
    {
        public AssetCreatedDomainEvent(Asset newAsset) : base(newAsset.AssetId)
        {
            NewAsset = newAsset;
        }

        public Asset NewAsset { get; protected set; }

    }
}
