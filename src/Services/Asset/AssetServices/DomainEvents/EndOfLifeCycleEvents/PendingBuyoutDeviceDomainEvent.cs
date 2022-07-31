using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;


namespace AssetServices.DomainEvents.EndOfLifeCycleEvents
{
    public class PendingBuyoutDeviceDomainEvent : BaseEvent
    {
        public PendingBuyoutDeviceDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, AssetLifecycleStatus previousAssetLifecycleStatus) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousAssetLifecycleStatus = previousAssetLifecycleStatus;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public AssetLifecycleStatus PreviousAssetLifecycleStatus { get; protected set; }

        public override string EventMessage()
        {
            return $"User Id: {AssetLifecycle.ContractHolderUser!.ExternalId} has been requested to Bought out on Last Working Day the asset Id:{AssetLifecycle.ExternalId} and Status has been changed to 'PendingBuyout' from {PreviousAssetLifecycleStatus} with Buyout Price {AssetLifecycle.OffboardBuyoutPrice};";
        }
    }
}
