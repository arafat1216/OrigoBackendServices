using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    public class CustomerAddedTechstepCustomerIdDomainEvent : BaseEvent
    {
        public CustomerAddedTechstepCustomerIdDomainEvent(Organization organization)
        {
            Organization = organization;
        }

        public Organization Organization { get; protected set; }

        public override string EventMessage()
        {
            return $"Techstep id was added for customer.";

        }
    }
}
