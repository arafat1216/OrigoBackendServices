using System;
using AssetServices.Models;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents;

public class AssignDepartmentAssetLifecycleDomainEvent : BaseEvent
{
    public AssignDepartmentAssetLifecycleDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, Guid? previousDepartmentId) : base(
        assetLifecycle.ExternalId)
    {
        AssetLifecycle = assetLifecycle;
        CallerId = callerId;
        PreviousDepartmentId = previousDepartmentId;
    }

    public AssetLifecycle AssetLifecycle { get; protected set; }
    public Guid CallerId { get; protected set; }
    public Guid? PreviousDepartmentId { get; protected set; }

    public override string EventMessage()
    {
        return PreviousDepartmentId == null
            ? $"Asset lifecycle assigned to department {AssetLifecycle.ManagedByDepartmentId}."
            : $"Asset lifecycle assigned to asset {AssetLifecycle.ManagedByDepartmentId} from department {PreviousDepartmentId}.";
    }
}