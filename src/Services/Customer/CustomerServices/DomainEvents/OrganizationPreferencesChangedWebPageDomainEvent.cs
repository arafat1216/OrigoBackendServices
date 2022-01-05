using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class OrganizationPreferencesChangedWebPageDomainEvent : BaseEvent
    {
        public OrganizationPreferencesChangedWebPageDomainEvent(OrganizationPreferences organizationPreferences, string oldWebPage) : base(organizationPreferences.OrganizationId)
        {
            OrganizationPreferences = organizationPreferences;
            OldWebPage = oldWebPage;
        }
        public OrganizationPreferences OrganizationPreferences { get; protected set; }
        public string OldWebPage { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer webpage changed from {OldWebPage} to {OrganizationPreferences.WebPage}.";
        }

        
    }
}
