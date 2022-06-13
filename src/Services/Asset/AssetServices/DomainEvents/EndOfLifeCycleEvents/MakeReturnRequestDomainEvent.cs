using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

namespace AssetServices.DomainEvents.EndOfLifeCycleEvents
{
    public class MakeReturnRequestDomainEvent : BaseEvent
    {
        public MakeReturnRequestDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, AssetLifecycleStatus previousAssetLifecycleStatus) : base(assetLifecycle.ExternalId)
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
            return $"User Id: {AssetLifecycle.ContractHolderUser!.ExternalId} has requested to Return the asset,Status has been changed to 'Pending Return' from {PreviousAssetLifecycleStatus};";
        }
    }
}
