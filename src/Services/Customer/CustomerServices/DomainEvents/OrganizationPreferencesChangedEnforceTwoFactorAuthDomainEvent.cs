using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class OrganizationPreferencesChangedEnforceTwoFactorAuthDomainEvent : BaseEvent
    {
        public OrganizationPreferencesChangedEnforceTwoFactorAuthDomainEvent(OrganizationPreferences organizationPreferences, string oldTwoFactorAuth) : base(organizationPreferences.OrganizationId)
        {
            OrganizationPreferences = organizationPreferences;
            OldTwoFactorAuth = oldTwoFactorAuth;
        }
        public OrganizationPreferences OrganizationPreferences { get; protected set; }
        public string OldTwoFactorAuth { get; protected set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer changed two factor authentication from {OldTwoFactorAuth} to {OrganizationPreferences.EnforceTwoFactorAuth}.";
        }
    }
}
