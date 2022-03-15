using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class OrganizationNumberChangedDomainEvent : BaseEvent
    {
        public OrganizationNumberChangedDomainEvent(Organization organization, string oldOrganizationNumber) : base(organization.OrganizationId)
        {
            Organization = organization;
            OldOrganizationNumber = oldOrganizationNumber;
        }

        public override string EventMessage()
        {
            return $"Customer organization number changed from {OldOrganizationNumber} to {Organization.OrganizationNumber}.";
        }

        public Organization Organization { get; protected set; }
        public string OldOrganizationNumber { get; protected set; }
    }
}
