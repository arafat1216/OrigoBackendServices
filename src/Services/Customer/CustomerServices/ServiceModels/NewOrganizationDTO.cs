using System;

#nullable enable

namespace CustomerServices.ServiceModels
{
    public record NewOrganizationDTO
    {
        public NewOrganizationDTO() { }

        public NewOrganizationDTO(string name, string organizationNumber, AddressDTO address, ContactPersonDTO contactPerson, LocationDTO location,
                                  Guid? parentId, string? internalNotes, Guid? partnerId, bool isCustomer,
                                  NewOrganizationPreferencesDTO preferences, Guid callerId)
        {
            Name = name;
            OrganizationNumber = organizationNumber;
            Address = address;
            ContactPerson = contactPerson;
            Location = location;
            PrimaryLocation = null;
            ParentId = parentId;
            InternalNotes = internalNotes;
            PartnerId = partnerId;
            IsCustomer = isCustomer;
            Preferences = preferences;
            CallerId = callerId;
        }

        public NewOrganizationDTO(string name, string organizationNumber, AddressDTO address, ContactPersonDTO contactPerson, Guid primaryLocation,
                                  Guid? parentId, string? internalNotes, Guid? partnerId, bool isCustomer,
                                  NewOrganizationPreferencesDTO preferences, Guid callerId)
        {
            Name = name;
            OrganizationNumber = organizationNumber;
            Address = address;
            ContactPerson = contactPerson;
            Location = null;
            PrimaryLocation = primaryLocation;
            ParentId = parentId;
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

        public string? InternalNotes { get; set; }

        /// <summary>
        ///     The ID for the partner that "owns" and handles the customer-relations with this organization. <para>   
        /// 
        ///     This value is required whenever <c><see cref="IsCustomer"/></c> is <c><see langword="true"/></c>. <br/>
        ///     This value will be <c><see langword="null"/></c> for special/custom organization entries (e.g. service-providers) that don't have any active
        ///     customer-relationship, and therefore should not be managed by a partner. </para>
        /// </summary>
        public Guid? PartnerId { get; set; }

        /// <summary>
        ///     If the value is <c><see langword="false"/></c>, then the organization is not currently considered a customer. This is typically for special 
        ///     organizations such as service-providers, partners or other legal entities where we need to store the corresponding organization and
        ///     contact details. Please note that these organizations may potentially also become a customer at a later date. <para>
        ///     
        ///     If the value is <c><see langword="true"/></c>, the organization is considered an active customer. </para><para>
        ///     
        ///     Note that whenever <c><see cref="IsCustomer"/></c> is set to <c><see langword="true"/></c>, the organization typically have stricter
        ///     checks and validations on the provided data. In additional there are typically additional requirements for customers that may need to
        ///     be provided before the value is updated. </para>
        /// </summary>
        public bool IsCustomer { get; set; }

        /// <summary>
        ///     The organization's preferences/settings. <para>
        ///     
        ///     If the preferences is <c><see langword="null"/></c> or omitted, it will be
        ///     created using the system's global default-values. The preferences should therefore
        ///     always be provided if possible! </para>
        /// </summary>
        public NewOrganizationPreferencesDTO? Preferences { get; set; }

        /// <summary>
        ///     The ID of the user that is creating the entity.
        /// </summary>
        public Guid CallerId { get; set; }

    }
}