using CustomerServices.ServiceModels;
using System;

namespace Customer.API.ViewModels
{
    public record Customer
    {
        public Guid Id { get; set; }

        public string CompanyName { get; set; }

        public string OrgNumber { get; set; }

        public AddressDTO CompanyAddress { get; set; }

        public ContactPerson CustomerContactPerson { get; set; }
    }
}
