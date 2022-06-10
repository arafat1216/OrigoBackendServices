using CustomerServices.ServiceModels;
using System;

#nullable enable

namespace Customer.API.ViewModels
{
    /// <summary>
    ///     A simplified and shortened version of the <see cref="Organization"/> model.
    /// </summary>
    public record PartnerOrganization
    {
        /// <inheritdoc cref="Organization.OrganizationId"/>
        public Guid OrganizationId { get; init; }

        /// <inheritdoc cref="Organization.Name"/>
        public string Name { get; init; }

        /// <inheritdoc cref="Organization.OrganizationNumber"/>
        public string OrganizationNumber { get; init; }

        /// <inheritdoc cref="Organization.Address"/>
        public AddressDTO? Address { get; init; }

        /// <inheritdoc cref="Organization.ContactPerson"/>
        public ContactPersonDTO? ContactPerson { get; init; }

        /// <inheritdoc cref="Organization.Location"/>
        public LocationDTO? Location { get; init; }


        public PartnerOrganization(CustomerServices.Models.Organization organization)
        {
            OrganizationId = organization.OrganizationId;
            Name = organization.Name;
            OrganizationNumber = organization.OrganizationNumber;

            if (organization.Address is not null)
                Address = new AddressDTO(organization.Address);

            if (organization.ContactPerson is not null)
                ContactPerson = new ContactPersonDTO(organization.ContactPerson);

            if (organization.PrimaryLocation is not null)
                Location = new LocationDTO(organization.PrimaryLocation);
        }
    }
}
