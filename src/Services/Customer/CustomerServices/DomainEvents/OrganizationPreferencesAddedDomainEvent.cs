using Common.Logging;
using CustomerServices.Models;


namespace CustomerServices.DomainEvents
{
    class OrganizationPreferencesAddedDomainEvent : BaseEvent
    {
        public OrganizationPreferencesAddedDomainEvent(OrganizationPreferences newOrganizationPreferences) : base(newOrganizationPreferences.OrganizationId)
        {
            NewOrganizationPreferences = newOrganizationPreferences;
        }
        public OrganizationPreferences NewOrganizationPreferences { get; protected set; }
        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Organization preferences added for {NewOrganizationPreferences.OrganizationId}.";
        }
    }
}
