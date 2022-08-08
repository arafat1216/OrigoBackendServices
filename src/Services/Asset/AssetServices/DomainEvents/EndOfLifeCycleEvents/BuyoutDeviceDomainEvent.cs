using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

namespace AssetServices.DomainEvents.EndOfLifeCycleEvents
{
    public class BuyoutDeviceDomainEvent : BaseEvent
    {
        public BuyoutDeviceDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, decimal buyoutPrice, AssetLifecycleStatus previousAssetLifecycleStatus) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousAssetLifecycleStatus = previousAssetLifecycleStatus;
            BuyoutPrice = buyoutPrice;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public AssetLifecycleStatus PreviousAssetLifecycleStatus { get; protected set; }
        public decimal BuyoutPrice { get; protected set; }

        public override string EventMessage()
        {
            return $"User Id: {AssetLifecycle.ContractHolderUser!.ExternalId} has Bought out the asset Id:{AssetLifecycle.ExternalId} and Status has been changed to 'BoughtByUser' from {PreviousAssetLifecycleStatus} with Buyout Price: {BuyoutPrice};";
        }
    }
}
