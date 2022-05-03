using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents.AssetLifecycleEvents
{
    public class UnAssignDepartmentAssetLifecycleDomainEvent : BaseEvent
    {
       public UnAssignDepartmentAssetLifecycleDomainEvent(AssetLifecycle assetLifecycle, Guid callerId) : base(
       assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public Guid? PreviousDepartmentId { get; protected set; }

        public override string EventMessage()
        {
            return AssetLifecycle.ContractHolderUser == null
                ? $"Asset lifecycle unassigned from department {AssetLifecycle.ManagedByDepartmentId}."
                : $"Asset lifecycle assigned to department {AssetLifecycle.ContractHolderUser} from department {AssetLifecycle.ManagedByDepartmentId}.";
        }
    }
}
