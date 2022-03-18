using System;

namespace Customer.API.ApiModels
{
    public record NewOrganization
    {
        public string Name { get; set; }

        public string OrganizationNumber { get; set; }

        public Address Address { get; set; }

        public ContactPerson ContactPerson { get; set; }

        public NewLocation Location { get; set; }

        public Guid PrimaryLocation { get; set; }
        public Guid ParentId { get; set; }
        public Guid CallerId { get; set; }

        public string ContactEmail { get; set; }

        public string InternalNotes { get; set; }

        public bool IsCustomer { get; set; }

        public NewOrganizationPreferences Preferences { get; set; }
    }
}