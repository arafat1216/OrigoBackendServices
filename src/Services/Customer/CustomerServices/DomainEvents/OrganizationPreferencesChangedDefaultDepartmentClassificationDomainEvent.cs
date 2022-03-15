using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    public class OrganizationPreferencesChangedDefaultDepartmentClassificationDomainEvent : BaseEvent
    {
        public OrganizationPreferencesChangedDefaultDepartmentClassificationDomainEvent(OrganizationPreferences organizationPreferences, string oldDefaultDepartmentClassification) : base(organizationPreferences.OrganizationId)
        {
            OrganizationPreferences = organizationPreferences;
            OldDefaultDepartmentClassification = oldDefaultDepartmentClassification;
        }

        public OrganizationPreferences OrganizationPreferences { get; protected set; }
        public string OldDefaultDepartmentClassification { get; protected set; }

        public override string EventMessage()
        {
            return $"Customer default department classification changed from {OldDefaultDepartmentClassification} to {OrganizationPreferences.DefaultDepartmentClassification}.";
        }
    }
}
