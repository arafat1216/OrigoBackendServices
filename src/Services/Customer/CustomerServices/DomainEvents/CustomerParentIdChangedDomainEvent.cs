using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public  class CustomerParentIdChangedDomainEvent : BaseEvent
    {
        public CustomerParentIdChangedDomainEvent(Organization organization, string oldParentId) : base(organization.OrganizationId)
        {
            Organization = organization;
            OldParentId = oldParentId;
        }

        public override string EventMessage()
        {
            return $"Customer parent id changed from {OldParentId} to {Organization.ParentId}.";
        }

        public Organization Organization { get; protected set; }
        public string OldParentId { get; protected set; }
    }
}
