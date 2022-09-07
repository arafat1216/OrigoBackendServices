namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Contains the information required for registering a new repair-order with an external service-provider.
    /// </summary>
    public abstract class NewExternalServiceOrderDTO
    {
        /// <summary>
        ///     Reserved JSON (de-)serializer constructor.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected NewExternalServiceOrderDTO()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            IncludedExternalAddonIds ??= new HashSet<string>();
        }


        protected NewExternalServiceOrderDTO(Guid userId, string firstName, string lastName, string? phoneNumber, string email, Guid organizationId, string organizationName, string? organizationNumber, Guid partnerId, string partnerName, string partnerOrganizationNumber, AssetInfoDTO assetInfo, ISet<string>? includedExternalAddonIds)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
            OrganizationId = organizationId;
            OrganizationName = organizationName;
            OrganizationNumber = organizationNumber;
            PartnerId = partnerId;
            PartnerName = partnerName;
            PartnerOrganizationNumber = partnerOrganizationNumber;
            AssetInfo = assetInfo;
            IncludedExternalAddonIds = includedExternalAddonIds ?? new HashSet<string>();
        }


        /// <summary>
        ///     The ID of the user that handles the service-order.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        ///     The first name of the person that handles the service-order.
        /// </summary>
        /// <example> John </example>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        ///     The last name of the person that handles the service-order.
        /// </summary>
        /// <example> Doe </example>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        ///     An phone-number in <c>E.164</c> format that the service-provider can use to get in touch with the person that handles the service-order.
        /// </summary>
        [Phone]
        [RegularExpression("^\\+[1-9]\\d{1,14}$")]
        [StringLength(maximumLength: 15)] // The longest possible length for a valid E.164 phone-number
        public string? PhoneNumber { get; set; }

        /// <summary>
        ///     An email address the service-provider can use to get in touch with the person that handles the service-order.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(maximumLength: 320)] // The RFC's max-length for email addresses
        public string Email { get; set; }

        /// <summary>
        ///     The ID for the organization that the service-request belongs to.
        /// </summary>
        [Required]
        public Guid OrganizationId { get; set; }

        /// <summary>
        ///     The name of the organization the service-request belongs to.
        /// </summary>
        /// <example> Demo Organization ASA </example>
        [Required]
        public string OrganizationName { get; set; }

        /// <summary>
        ///     The organization-number for the organization the service-request belongs to.
        /// </summary>
        /// <example> 999990000 </example>
        public string? OrganizationNumber { get; set; }

        /// <summary>
        ///     The partner ID.
        /// </summary>
        [Required]
        public Guid PartnerId { get; set; }

        /// <summary>
        ///     The juridical name for the partner.
        /// </summary>
        /// <example> MyPartner A/S </example>
        [Required]
        public string PartnerName { get; set; }

        /// <summary>
        ///     The partner's organization number.
        /// </summary>
        /// <example> 888880000 </example>
        [Required]
        public string PartnerOrganizationNumber { get; set; }

        /// <summary>
        ///     Information about the product.
        /// </summary>
        [Required]
        public AssetInfoDTO AssetInfo { get; set; }

        /// <summary>
        ///     A list containing a list of the service-providers IDs (<see cref="ServiceOrderAddonDTO.ServiceProviderId"/>), detailing
        ///     what <see cref="ServiceOrderAddonDTO"/> items to be included when the service-order is created.
        /// </summary>
        public ISet<string> IncludedExternalAddonIds { get; set; }
    }
}
