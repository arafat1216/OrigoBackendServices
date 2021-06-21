using System;
using System.Collections;
using System.Collections.Generic;
using Common.Seedwork;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace CustomerServices.Models
{
    public class Customer : Entity, IAggregateRoot
    {
        public Guid CustomerId { get; protected set; }

        public string CompanyName { get; protected set; }

        public string OrgNumber { get; protected set; }

        public Address CompanyAddress { get; protected set; }

        public ContactPerson CustomerContactPerson { get; protected set; }

        public IList<User> Users { get; set; }

        public IList<ProductModuleGroup> SelectedProductModuleGroups { get; set; }

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
