using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class CustomerNameChangedDomainEvent : BaseEvent
    {
        public CustomerNameChangedDomainEvent(Organization organization, string oldName) : base(organization.OrganizationId)
        {
            Organization = organization;
            OldName = oldName;
        }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer name changed from {OldName} to {Organization.Name}.";
        }

        public Organization Organization { get; protected set; }
        public string OldName { get; protected set; }
    }
}
