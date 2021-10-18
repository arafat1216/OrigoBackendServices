using System;

namespace OrigoApiGateway.Models
{
    public record NewOrganization
    {
        public string Name { get; set; }

        public string OrganizationNumber { get; set; }

        public Address Address { get; set; }

        public ContactPerson ContactPerson { get; set; }

        public Guid ParentId { get; set; }
        public Guid PrimaryLocation { get; set; }
        public string ContactEmail { get; set; }
        public string InternalNotes { get; set; }
        public NewLocation Location { get; set; }
        public NewOrganizationPreferences Preferences { get; set; }
    }
}
