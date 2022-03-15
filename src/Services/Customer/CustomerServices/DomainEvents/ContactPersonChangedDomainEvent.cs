using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class ContactPersonChangedDomainEvent : BaseEvent
    {
        public ContactPersonChangedDomainEvent(Organization organization, ContactPerson oldContactPerson) : base(organization.OrganizationId)
        {
            Organization = organization;
            OldContactPerson = oldContactPerson;
        }

        public override string EventMessage()
        {
            return $"Contact person changed from {OldContactPerson} to {Organization.ContactPerson}.";
        }

        public Organization Organization { get; protected set; }
        public ContactPerson OldContactPerson { get; protected set; }
    }
}
