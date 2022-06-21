using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class AddUsersToOktaChangedDomainEvent : BaseEvent
    {
        public AddUsersToOktaChangedDomainEvent(Organization organization) : base(organization.OrganizationId)
        {
            Organization = organization;
        }

        public override string EventMessage()
        {
            return $"Add users to Okta changed from {!Organization.AddUsersToOkta} to {Organization.AddUsersToOkta}.";
        }

        public Organization Organization { get; protected set; }
    }
}
