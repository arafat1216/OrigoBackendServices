using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class OrganizationPreferencesDeletedDomainEvent : BaseEvent
    {
        public OrganizationPreferencesDeletedDomainEvent(OrganizationPreferences organizationPreferences) : base(organizationPreferences.OrganizationId)
        {
            OrganizationPreferences = organizationPreferences;
        }

        public OrganizationPreferences OrganizationPreferences { get; protected set; }

        public override string EventMessage()
        {
            return $"Organization preferences for customer {OrganizationPreferences.OrganizationId} was deleted.";
        }
    }
}
