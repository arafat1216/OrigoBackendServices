﻿using System;
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
            ? $"Asset lifecycle assigned to asset {AssetLifecycle?.Asset?.ExternalId} - {AssetLifecycle?.Asset?.ProductName}."
            : $"Asset lifecycle assigned to asset {AssetLifecycle?.Asset?.ExternalId} - {AssetLifecycle?.Asset?.ProductName} from asset {PreviousAssetId}.";
    }
}