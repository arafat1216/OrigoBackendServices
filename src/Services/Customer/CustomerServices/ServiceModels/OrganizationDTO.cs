using CustomerServices.ServiceModels;
using System;
using System.Collections.Generic;

namespace CustomerServices.ServiceModels
{
    /// <inheritdoc cref="CustomerServices.Models.Organization"/>
    public record OrganizationDTO
    {
        /// <inheritdoc cref="CustomerServices.Models.Organization.OrganizationId"/>
        public Guid OrganizationId { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.Name"/>
        public string Name { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.OrganizationNumber"/>
        public string OrganizationNumber { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.Address"/>
        public AddressDTO Address { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.ContactPerson"/>
        public ContactPersonDTO ContactPerson { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.Location"/>
        public LocationDTO Location { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.Preferences"/>
        public OrganizationPreferencesDTO Preferences { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Organization.ChildOrganizations"/>
        public ICollection<OrganizationDTO> ChildOrganizations { get; set; }
    }
}
