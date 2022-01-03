using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    public class OrganizationPreferencesUpdatedDomainEvent : BaseEvent
    {
        public OrganizationPreferencesUpdatedDomainEvent(Organization organization):base(organization.OrganizationId)
        {
            Organization = organization;
        }
        public Organization Organization { get; protected set; }
        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"";
        }
    }
}
