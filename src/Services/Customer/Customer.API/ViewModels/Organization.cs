using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public record Organization
    {
        public System.Guid OrganizationId { get; set; }

        public string Name { get; set; }

        public string OrganizationNumber { get; set; }

        public Address Address { get; set; }

        public ContactPerson ContactPerson { get; set; }

        public Location Location { get; set; }

        public OrganizationPreferences Preferences { get; set; }

        public ICollection<Organization> ChildOrganizations { get; set; }
    }
}
