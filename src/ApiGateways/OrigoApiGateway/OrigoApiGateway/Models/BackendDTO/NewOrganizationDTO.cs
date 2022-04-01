using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record NewOrganizationDTO
    {
        public string Name { get; set; }

        public string OrganizationNumber { get; set; }

        public Address Address { get; set; }

        public OrigoContactPerson ContactPerson { get; set; }
        public NewLocation Location { get; set; }
        public Guid PrimaryLocation { get; set; }
        public Guid? ParentId { get; set; }
        public Guid CallerId { get; set; }

        [Obsolete("This is not in use, and will soon be removed")]
        public string? ContactEmail { get; set; }
        public string InternalNotes { get; set; }

        public Guid? PartnerId { get; set; }
        public bool IsCustomer { get; set; }

        public NewOrganizationPreferences Preferences { get; set; }
    }
}
