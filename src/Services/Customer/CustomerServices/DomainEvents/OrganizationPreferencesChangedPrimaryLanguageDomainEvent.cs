using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class OrganizationPreferencesChangedPrimaryLanguageDomainEvent : BaseEvent
    {
        public OrganizationPreferencesChangedPrimaryLanguageDomainEvent(OrganizationPreferences organizationPreferences, string oldPrimaryLanguage) : base(organizationPreferences.OrganizationId)
        {
            OrganizationPreferences = organizationPreferences;
            OldPrimaryLanguage = oldPrimaryLanguage;
        }

        public OrganizationPreferences OrganizationPreferences { get; protected set; }
        public string OldPrimaryLanguage { get; protected set; }

        public override string EventMessage()
        {
            return $"Customer changed primary language from {OldPrimaryLanguage} to {OrganizationPreferences.PrimaryLanguage}.";
        }
    }
}
