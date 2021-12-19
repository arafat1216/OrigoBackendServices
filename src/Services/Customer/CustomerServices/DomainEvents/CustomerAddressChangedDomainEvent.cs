using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class CustomerAddressChangedDomainEvent : BaseEvent
    {
        public CustomerAddressChangedDomainEvent(Organization organization, Address oldAddress) : base(organization.OrganizationId)
        {
            Organization = organization;
            OldAddress = oldAddress;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer Address changed from {OldAddress} to {Organization.Address}.";
        }

        public Organization Organization { get; protected set; }
        public Address OldAddress { get; protected set; }
    }
}
