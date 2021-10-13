using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public record Organization
    {
        public System.Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationNumber { get; set; }

        public Address OrganizationAddress { get; set; }

        public ContactPerson OrganizationContactPerson { get; set; }

        public Location OrganizationLocation { get; set; }

        public OrganizationPreferences OrganizationPreferences { get; set; }

        public ICollection<Organization> ChildOrganizations { get; set; }
    }
}
