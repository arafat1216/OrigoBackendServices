using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    public class OrganizationUpdateAccountOwner : BaseEvent
    {
        public OrganizationUpdateAccountOwner(Organization organization, string oldOrganizationOwner)
        {
            this.organization = organization;
            OldOrganizationOwner = oldOrganizationOwner;
        }

        public Organization organization { get; protected set; }
        public string OldOrganizationOwner { get; protected set; }

        public override string EventMessage()
        {
            return $"Customers account owner changed from {OldOrganizationOwner} to {organization.AccountOwner}.";
        }
    }
}
