using CustomerServices.Models;
using MediatR;
using System;
using Common.Logging;

namespace CustomerServices.DomainEvents
{
    public class CustomerCreatedDomainEvent : BaseEvent
    {

        public CustomerCreatedDomainEvent(Customer newCustomer)
        {
            CustomerId = newCustomer.CustomerId;
            CompanyName = newCustomer.CompanyName;
            OrgNumber = newCustomer.CompanyName;
            CompanyAddress = (Address) newCustomer.CompanyAddress.GetCopy();
            CustomerContactPerson = (ContactPerson) newCustomer.CustomerContactPerson.GetCopy();
        }

        public Guid CustomerId { get; }

        public string CompanyName { get; }

        public string OrgNumber { get;  }

        public Address CompanyAddress { get; }

        public ContactPerson CustomerContactPerson { get;  }
    }
}
