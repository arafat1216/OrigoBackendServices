using System;

namespace Customer.API.ViewModels
{
    public record Customer
    {
        public Guid Id { get; set; }

        public string CompanyName { get; set; }

        public string OrgNumber { get; set; }

        public Address CompanyAddress { get; set; }

        public ContactPerson CustomerContactPerson { get; set; }
    }
}
