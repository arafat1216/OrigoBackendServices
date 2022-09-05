using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    public class OrganizationUpdateTechstepAccountOwner : BaseEvent
    {
        public OrganizationUpdateTechstepAccountOwner(Organization organization, string oldOrganizationOwner)
        {
            this.organization = organization;
            OldOrganizationOwner = oldOrganizationOwner;
        }

        public Organization organization { get; protected set; }
        public string OldOrganizationOwner { get; protected set; }

        public override string EventMessage()
        {
            return $"Customer techstep account owner changed from {OldOrganizationOwner} to {organization.TechstepAccountOwner}.";
        }
    }
}
