using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class MakeAssetAvailableDomainEvent : BaseEvent
    {
        public MakeAssetAvailableDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, AssetLifecycleStatus previousAssetLifecycleStatus, User? previousContractHolderUser) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousAssetLifecycleStatus = previousAssetLifecycleStatus;
            PreviousContractHolder = previousContractHolderUser;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public AssetLifecycleStatus PreviousAssetLifecycleStatus { get; protected set; }
        public User? PreviousContractHolder { get; protected set; }

        public override string EventMessage()
        {
            return $"Asset Life Cycle has made 'Available' from {PreviousAssetLifecycleStatus}; Previous possible User Id: {PreviousContractHolder?.ExternalId} ";
        }
    }
}
