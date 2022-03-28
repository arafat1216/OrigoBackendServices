using CustomerServices.ServiceModels;
using System;

namespace Customer.API.ViewModels
{
    public record NewOrganization
    {
        public string Name { get; set; }

        public string OrganizationNumber { get; set; }

        public AddressDTO Address { get; set; }

        public ContactPerson ContactPerson { get; set; }

        /// <summary>
        ///     The new location object that should be created and attached to the organization. <para>
        ///     
        ///     This is mutually exclusive with <see cref="PrimaryLocation"/>, meaning only one of them should be provided. </para>
        /// </summary>
        public NewLocation Location { get; set; }

        /// <summary>
        ///     The ID of an existing location object that should be attached to the organization. <para>
        ///     
        ///     This is mutually exclusive with <see cref="Location"/>, meaning only one of them should be provided. </para>
        /// </summary>
        public Guid PrimaryLocation { get; set; }

        /// <summary>
        ///     If this is a child-organization, this is the <see cref="Organization.OrganizationId"/> of the parent. 
        ///     For root-organization this value can be <see langword="null"/> or omitted.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        ///     The ID of the user that is creating the entity.
        /// </summary>
        public Guid CallerId { get; set; }

        public string ContactEmail { get; set; }

        public string InternalNotes { get; set; }

        public Guid? PartnerId { get; set; }

        /// <summary>
        ///     Is organization also a customer?
        /// </summary>
        public bool IsCustomer { get; set; }

        public NewOrganizationPreferences Preferences { get; set; }
    }
}