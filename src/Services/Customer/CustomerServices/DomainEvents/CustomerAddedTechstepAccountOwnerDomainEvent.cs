using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    public class CustomerAddedTechstepAccountOwnerDomainEvent : BaseEvent
    {
        public CustomerAddedTechstepAccountOwnerDomainEvent(Organization organization)
        {
            Organization = organization;
        }

        public Organization Organization { get; protected set; }

        public override string EventMessage()
        {
            return $"{Organization.TechstepAccountOwner} was added as account owner.";

        }
    }
}
