using System;

namespace Customer.API.ViewModels
{
    public record NewOrganization
    {
        public string OrganizationName { get; set; }

        public string OrganizationNumber { get; set; }

        public Address OrganizationAddress { get; set; }

        public ContactPerson OrganizationContactPerson { get; set; }

        public NewLocation OrganizationLocation { get; set; }

        public Guid PrimaryLocation { get; set; }
        public Guid ParentId { get; set; }

        public string ContactEmail { get; set; }

        public string InternalNotes { get; set; }

        public NewOrganizationPreferences OrganizationPreferences { get; set; }
    }
}