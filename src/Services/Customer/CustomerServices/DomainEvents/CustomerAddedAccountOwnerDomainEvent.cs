using Common.Logging;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.DomainEvents
{
    public class CustomerAddedAccountOwnerDomainEvent : BaseEvent
    {
        public CustomerAddedAccountOwnerDomainEvent(Organization organization)
        {
            Organization = organization;
        }

        public Organization Organization { get; protected set; }

        public override string EventMessage()
        {
            return $"{Organization.AccountOwner} was added as account owner.";

        }
    }
}
