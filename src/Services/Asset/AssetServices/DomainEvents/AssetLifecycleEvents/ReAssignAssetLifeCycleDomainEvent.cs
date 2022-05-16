using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class ReAssignAssetLifeCycleDomainEvent : BaseEvent
    {
        public ReAssignAssetLifeCycleDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, User? previousContractHolder, Guid? previousDepartmentId) : base(
        assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            PreviousContractHolder = previousContractHolder;
            PreviousDepartmentId = previousDepartmentId;
        }
        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public Guid? PreviousDepartmentId { get; protected set; }
        public User? PreviousContractHolder { get; protected set; }
        public override string EventMessage()
        {
            return AssetLifecycle.ContractHolderUser != null ?
                $"Re-assignment of asset: {AssetLifecycle.ExternalId}; To Department: {AssetLifecycle.ManagedByDepartmentId} and User: {AssetLifecycle.ContractHolderUser?.Name} ({AssetLifecycle.ContractHolderUser?.ExternalId}); From Department: {PreviousDepartmentId} and User: {PreviousContractHolder?.Name} ({PreviousContractHolder?.ExternalId})"
                : $"Re-assignment of asset: {AssetLifecycle.ExternalId}; To Department: {AssetLifecycle.ManagedByDepartmentId}; From Department: {PreviousDepartmentId}";
        }
    }
}
