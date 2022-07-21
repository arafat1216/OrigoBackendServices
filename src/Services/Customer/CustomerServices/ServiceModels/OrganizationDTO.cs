using Common.Enums;
using CustomerServices.Models;
using System;
using System.Collections.Generic;

#nullable enable

namespace CustomerServices.ServiceModels
{
    /// <inheritdoc cref="Organization"/>
    public record OrganizationDTO
    {
        public OrganizationDTO() { }

        public OrganizationDTO(Organization organization)
        {
            OrganizationId = organization.OrganizationId;
            Name = organization.Name;
            OrganizationNumber = organization.OrganizationNumber;
            Address = new AddressDTO(organization.Address);
            ContactPerson = new ContactPersonDTO(organization.ContactPerson);
            Location = new LocationDTO(organization.PrimaryLocation!);
            Preferences = new OrganizationPreferencesDTO(organization.Preferences);
            PartnerId = organization.Partner?.ExternalId;
            ParentId = organization.ParentId;
            AddUsersToOkta = organization.AddUsersToOkta;

            if (organization.ChildOrganizations is not null && organization.ChildOrganizations.Count != 0)
            {
                foreach (var child in organization.ChildOrganizations)
                {
                    ChildOrganizations.Add(new OrganizationDTO(child));
                }
            }
        }


        /// <inheritdoc cref="Organization.OrganizationId"/>
        public Guid OrganizationId { get; set; }

        /// <inheritdoc cref="Organization.ParentId"/>
        public Guid? ParentId { get; set; }

        /// <inheritdoc cref="Organization.Name"/>
        public string Name { get; set; }

        /// <inheritdoc cref="Organization.OrganizationNumber"/>
        public string OrganizationNumber { get; set; }

        /// <inheritdoc cref="Organization.Address"/>
        public AddressDTO Address { get; set; }

        /// <inheritdoc cref="Organization.ContactPerson"/>
        public ContactPersonDTO ContactPerson { get; set; }

        /// <inheritdoc cref="Organization.Location"/>
        public LocationDTO Location { get; set; }

        /// <inheritdoc cref="Organization.Preferences"/>
        public OrganizationPreferencesDTO Preferences { get; set; }

        /// <inheritdoc cref="Organization.ChildOrganizations"/>
        public ICollection<OrganizationDTO> ChildOrganizations { get; set; } = new List<OrganizationDTO>();

        /// <summary>
        ///     The partner that "owns" and handles the customer-relations with this organization.
        /// </summary>
        public Guid? PartnerId { get; set; }

        /// <summary>
        /// Should new users be added to Okta when created.
        /// </summary>
        public bool? AddUsersToOkta { get; set; }
        /// <summary>
        /// The enum value of the customer status 
        /// </summary>
        public CustomerStatus Status { get; set; }
        /// <summary>
        /// The string value of the customer status
        /// </summary>
        public string StatusName { get; set; }
    }
}
