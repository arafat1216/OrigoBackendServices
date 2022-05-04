using System;
using AssetServices.Models;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents;

public class AssignAssetToAssetLifeCycleDomainEvent : BaseEvent
{
    public AssignAssetToAssetLifeCycleDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, Guid? previousAssetId) : base(
        assetLifecycle.ExternalId)
    {
        AssetLifecycle = assetLifecycle;
        CallerId = callerId;
        PreviousAssetId = previousAssetId;
    }

    public AssetLifecycle AssetLifecycle { get; protected set; }

    public Guid CallerId { get; protected set; }
    public Guid? PreviousAssetId { get; protected set; }

    public override string EventMessage()
    {

        return PreviousAssetId == null
            ? $"Asset lifecycle with id assigned to asset."
            : $"Asset lifecycle with id {AssetLifecycle?.ExternalId} assigned to asset from asset {PreviousAssetId}.";
    }
}