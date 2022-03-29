using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class UnAssignContractHolderToAssetLifeCycleDomainEvent : BaseEvent
    {
        public UnAssignContractHolderToAssetLifeCycleDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, User? previousContractHolderUser) : base(
        assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousContractHolder = previousContractHolderUser;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public User? PreviousContractHolder { get; protected set; }

        public override string EventMessage()
        {
            return $"Asset lifecycle un-assigned from user with ID {PreviousContractHolder?.Id}.";
        }
    }
}
