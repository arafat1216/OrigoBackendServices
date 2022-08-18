namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public record NewOrganization
    {
        /// <summary>
        ///     The name of the organization.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        ///     The organization-number as used in the national registry.
        /// </summary>
        public string OrganizationNumber { get; init; }

        public Address Address { get; init; }

        public OrigoContactPerson ContactPerson { get; init; }

        /// <summary>
        ///     If this is a child/sub-organization, then this is the ID of it's parent. If this is a top-level organization, then the value 
        ///     should be <c><see langword="null"/></c>, or omitted from the request.
        /// </summary>
        public Guid? ParentId { get; init; }

        /// <summary>
        ///     <c>Note:</c> This is mutually exclusive with <c><see cref="Location"/></c>. As such, only one of them should be provided. <para>
        /// 
        ///     The ID of an existing location that should be used. </para>
        /// </summary>
        public Guid? PrimaryLocation { get; init; }

        /// <summary>
        ///     <c>Note:</c> This is mutually exclusive with <c><see cref="PrimaryLocation"/></c>. As such, only one of them should be provided. <para>
        /// 
        ///     The new location object that is created and used with the organization. </para>
        /// </summary>
        public NewLocation? Location { get; set; }

        [EmailAddress]
        [Obsolete("This is not in use, and will soon be removed.")]
        public string? ContactEmail { get; set; }

        public string? InternalNotes { get; init; }

        public NewOrganizationPreferences Preferences { get; init; }

        /// <summary>
        ///     The ID for the partner that "owns" and handles the customer-relations with this organization. <para>   
        /// 
        ///     This value is required for all customers. This should be <c><see langword="null"/></c> or omitted for other organizations, such as 
        ///     specialized/custom organizations (e.g. partners and service-providers) that don't have a normal customer relationship under a partner. </para>
        /// </summary>
        public Guid? PartnerId { get; set; }

        /// <summary>
        /// Should new users be added to Okta for this organization.
        /// </summary>
        public bool? AddUsersToOkta { get; set; }
    }
}
