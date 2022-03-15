using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class CustomerPrimaryLocationChangedDomainEvent : BaseEvent
    {
        public CustomerPrimaryLocationChangedDomainEvent(Organization organization, string oldPrimaryLocation) : base(organization.OrganizationId)
        {
            Organization = organization;
            OldPrimaryLocation = oldPrimaryLocation;
        }

        public override string EventMessage()
        {
            return $"Customer primary location changed from {OldPrimaryLocation} to {Organization.PrimaryLocation}.";
        }

        public Organization Organization { get; protected set; }
        public string OldPrimaryLocation { get; protected set; }
    }
}
