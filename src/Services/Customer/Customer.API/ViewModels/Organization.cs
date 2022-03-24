using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    /// <inheritdoc cref="CustomerServices.Models.Organization"/>
    public record Organization
    {
        /// <inheritdoc cref="CustomerServices.Models.Organization.OrganizationId"/>
        public Guid OrganizationId { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.Name"/>
        public string Name { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.OrganizationNumber"/>
        public string OrganizationNumber { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.Address"/>
        public Address Address { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.ContactPerson"/>
        public ContactPerson ContactPerson { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.Location"/>
        public Location Location { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.Preferences"/>
        public OrganizationPreferences Preferences { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.ChildOrganizations"/>
        public ICollection<Organization> ChildOrganizations { get; set; }
    }
}
