using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class OrganizationPreferencesChangedLogoUrlDomainEvent : BaseEvent
    {
        public OrganizationPreferencesChangedLogoUrlDomainEvent(OrganizationPreferences organizationPreferences, string oldLogoURL) : base(organizationPreferences.OrganizationId)
        {
            OrganizationPreferences = organizationPreferences;
            OldLogoURL = oldLogoURL;
        }

        public OrganizationPreferences OrganizationPreferences { get; protected set; }
        public string OldLogoURL { get; protected set; }

        public override string EventMessage()
        {
            return $"Customer changed logo URL from {OldLogoURL} to {OrganizationPreferences.LogoUrl}.";
        }
    }
}
