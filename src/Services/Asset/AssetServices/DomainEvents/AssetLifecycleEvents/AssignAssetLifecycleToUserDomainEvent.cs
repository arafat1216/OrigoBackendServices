using System;
using AssetServices.Models;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class AssignAssetLifecycleToUserDomainEvent : BaseEvent
    {
        public AssignAssetLifecycleToUserDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, Guid? previousUserId):base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousUserId = previousUserId;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public Guid? PreviousUserId { get; protected set; }

        public override string EventMessage()
        {
            return $"Asset life cycle assigned to user {AssetLifecycle.ContractHolderUser?.Name} from user {PreviousUserId}.";
        }
    }
}