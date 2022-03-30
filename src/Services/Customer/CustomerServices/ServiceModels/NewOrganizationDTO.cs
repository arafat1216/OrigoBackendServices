using System;

#nullable enable

namespace CustomerServices.ServiceModels
{
    public record NewOrganizationDTO
    {
        public NewOrganizationDTO() { }

        public NewOrganizationDTO(string name, string organizationNumber, AddressDTO address, ContactPersonDTO contactPerson, LocationDTO location,
                                  Guid? parentId, string contactEmail, string? internalNotes, Guid? partnerId, bool isCustomer,
                                  NewOrganizationPreferencesDTO preferences, Guid callerId)
        {
            Name = name;
            OrganizationNumber = organizationNumber;
            Address = address;
            ContactPerson = contactPerson;
            Location = location;
            PrimaryLocation = null;
            ParentId = parentId;
            ContactEmail = contactEmail;
            InternalNotes = internalNotes;
            PartnerId = partnerId;
            IsCustomer = isCustomer;
            Preferences = preferences;
            CallerId = callerId;
        }

        public NewOrganizationDTO(string name, string organizationNumber, AddressDTO address, ContactPersonDTO contactPerson, Guid primaryLocation,
                                  Guid? parentId, string contactEmail, string? internalNotes, Guid? partnerId, bool isCustomer,
                                  NewOrganizationPreferencesDTO preferences, Guid callerId)
        {
            Name = name;
            OrganizationNumber = organizationNumber;
            Address = address;
            ContactPerson = contactPerson;
            Location = null;
            PrimaryLocation = primaryLocation;
            ParentId = parentId;
            ContactEmail = contactEmail;
            InternalNotes = internalNotes;
            PartnerId = partnerId;
            IsCustomer = isCustomer;
            Preferences = preferences;
            CallerId = callerId;
        }

        public string Name { get; set; }

        public string OrganizationNumber { get; set; }

        public AddressDTO Address { get; set; }

        public ContactPersonDTO ContactPerson { get; set; }

        /// <summary>
        ///     The new location object that should be created and attached to the organization. <para>
        ///     
        ///     This is mutually exclusive with <see cref="PrimaryLocation"/>, meaning only one of them should be provided. </para>
        /// </summary>
        public LocationDTO? Location { get; set; }

        /// <summary>
        ///     The ID of an existing location object that should be attached to the organization. <para>
        ///     
        ///     This is mutually exclusive with <see cref="Location"/>, meaning only one of them should be provided. </para>
        /// </summary>
        public Guid? PrimaryLocation { get; set; }

        /// <summary>
        ///     If this is a child-organization, this is the <see cref="Organization.OrganizationId"/> of the parent. 
        ///     For root-organization this value can be <see langword="null"/> or omitted.
        /// </summary>
        public Guid? ParentId { get; set; }

        public string ContactEmail { get; set; }

        public string? InternalNotes { get; set; }

        public Guid? PartnerId { get; set; }

        /// <summary>
        ///     Is organization also a customer?
        /// </summary>
        public bool IsCustomer { get; set; }

        public NewOrganizationPreferencesDTO Preferences { get; set; }

        /// <summary>
        ///     The ID of the user that is creating the entity.
        /// </summary>
        public Guid CallerId { get; set; }

    }
}