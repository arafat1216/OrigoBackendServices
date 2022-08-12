using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    /// <summary>
    /// Domain event when asset lifecycle changes status.
    /// </summary>
    public class UpdateAssetLifecycleStatusDomainEvent : BaseEvent
    {
        /// <summary>
        /// Initializes a new instance of the UpdateAssetLifecycleStatusDomainEvent
        /// </summary>
        /// <param name="assetLifecycle">Asset lifecycle object.</param>
        /// <param name="previousAssetLifecycleStatus">The status the asset lifecycle had before the change.</param>
        /// <param name="callerId">The identification of the user making the change.</param>
        public UpdateAssetLifecycleStatusDomainEvent(AssetLifecycle assetLifecycle, AssetLifecycleStatus previousAssetLifecycleStatus, Guid callerId):base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousAssetLifecycleStatus = previousAssetLifecycleStatus;
        }
        /// <summary>
        /// The asset lifecycle object that has been changed.
        /// </summary>
        public AssetLifecycle AssetLifecycle { get; protected set; }
        /// <summary>
        /// The previous status of the asset lifecycle.
        /// </summary>
        public AssetLifecycleStatus PreviousAssetLifecycleStatus { get; protected set; }
        /// <summary>
        /// Identification for the user making the change.
        /// </summary>
        public Guid CallerId { get; protected set; }
        /// <inheritdoc/>
        public override string EventMessage()
        {
            return $"Asset life cycle status changed from {PreviousAssetLifecycleStatus} to {AssetLifecycle.AssetLifecycleStatus}.";
        }
    }
}