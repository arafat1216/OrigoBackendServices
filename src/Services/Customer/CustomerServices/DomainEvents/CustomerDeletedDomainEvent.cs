using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class CustomerDeletedDomainEvent : BaseEvent
    {
        public CustomerDeletedDomainEvent(Organization organization) : base(organization.OrganizationId)
        {
            Organization = organization;
        }

        public override string EventMessage()
        {
            return $"Customer was deleted {Organization.OrganizationId}.";
        }

        public Organization Organization { get; protected set; }
    }
}
