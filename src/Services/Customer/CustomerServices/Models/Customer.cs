using System;
using Infrastructure.Seedwork;

namespace CustomerServices.Models
{
    public class Customer : Entity, IAggregateRoot
    {
        public Guid CustomerId { get; }

        public string CompanyName { get; }

        public string OrgNumber { get; }

        public Address CompanyAddress { get; }

        public ContactPerson CustomerContactPerson { get; }

        protected Customer()
        {

        }

        public Customer(Guid customerId, string companyName, string orgNumber, Address companyAddress,
            ContactPerson customerContactPerson)
        {
            CompanyName = companyName;
            OrgNumber = orgNumber;
            CompanyAddress = companyAddress;
            CustomerContactPerson = customerContactPerson;
            CustomerId = customerId;
        }
    }
}
