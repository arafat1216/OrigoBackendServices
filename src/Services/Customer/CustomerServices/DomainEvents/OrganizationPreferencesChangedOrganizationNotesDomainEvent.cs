using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class OrganizationPreferencesChangedOrganizationNotesDomainEvent : BaseEvent 
    {
        public OrganizationPreferencesChangedOrganizationNotesDomainEvent(OrganizationPreferences organizationPreferences, string oldNotes) : base(organizationPreferences.OrganizationId)
        {
            OrganizationPreferences = organizationPreferences;
            OldNotes = oldNotes;
        }

        public OrganizationPreferences OrganizationPreferences { get; protected set; }
        public string OldNotes { get; protected set; }

        public override string EventMessage()
        {
            return $"Customer changed organization notes from {OldNotes} to {OrganizationPreferences.OrganizationNotes}.";
        }
    }
}
