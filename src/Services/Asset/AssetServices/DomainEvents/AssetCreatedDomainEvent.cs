using MediatR;
using System;
using AssetServices.Models;

namespace AssetServices.DomainEvents
{
    public class AssetCreatedDomainEvent : INotification
    {
        public AssetCreatedDomainEvent(Asset newAsset)
        {
            NewAsset = newAsset;
        }

        public Asset NewAsset { get; set; }

    }
}
