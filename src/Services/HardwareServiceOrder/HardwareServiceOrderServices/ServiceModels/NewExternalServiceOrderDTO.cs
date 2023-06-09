﻿namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Contains the information required for registering a new repair-order with an external service-provider.
    /// </summary>
    public class NewExternalServiceOrderDTO
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="NewExternalServiceOrderDTO"/> class.
        ///     
        ///     <para>
        ///     This is a reserved constructor intended for JSON serializers, AutoMapper, unit-testing and other automated processes. This constructor
        ///     should never be called directly in any production code. </para>
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public NewExternalServiceOrderDTO()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            IncludedExternalAddonIds ??= new HashSet<string>();
        }


        /// <summary>
        ///     Creates a new instance of the <see cref="NewExternalServiceOrderDTO"/> class.
        /// </summary>
        /// <param name="userId"> The ID of the user that handles the service-order. </param>
        /// <param name="firstName"> The first name of the person that handles the service-order. </param>
        /// <param name="lastName"> The last name of the person that handles the service-order. </param>
        /// <param name="phoneNumber"> An phone-number in <c>E.164</c> format that the service-provider can use to get in touch with the person that handles the service-order. </param>
        /// <param name="email"> An email address the service-provider can use to get in touch with the person that handles the service-order. </param>
        /// <param name="organizationId"> The ID for the organization that the service-request belongs to. </param>
        /// <param name="organizationName"> The name of the organization the service-request belongs to. </param>
        /// <param name="organizationNumber"> The national/legal entity's organization-number (business registration number), that identifies the organization the service-request belongs to. </param>
        /// <param name="partnerId"> The partner's ID. </param>
        /// <param name="partnerName"> The juridical name for the partner. </param>
        /// <param name="partnerOrganizationNumber"> The partner's national/legal entity's organization-number (business registration number). </param>
        /// <param name="deliveryAddress"> 
        ///     If applicable, the address that the asset should be delivery to once the service is complete. 
        ///     This is also used as the "return to sender" address in shipping labels.
        ///     
        ///     <para>
        ///     This is also used as the "return to sender" address in shipping-labels. </para>
        /// </param>
        /// <param name="assetInfo"> Information about the asset that is sent to the service-provider. </param>
        /// <param name="userDescription"> A user provided description explaining the problem or reason for the service order. </param>
        /// <param name="includedExternalAddonIds"> 
        ///     A list containing a list of the service-providers IDs (<see cref="ServiceOrderAddonDTO.ServiceProviderId"/>), detailing
        ///     what <see cref="ServiceOrderAddonDTO"/> items to be included when the service-order is created.
        /// </param>
        public NewExternalServiceOrderDTO(Guid userId, string firstName, string lastName, string? phoneNumber, string email, Guid organizationId, string organizationName, string? organizationNumber, Guid partnerId, string partnerName, string partnerOrganizationNumber, DeliveryAddressDTO deliveryAddress, AssetInfoDTO assetInfo, string userDescription, ISet<string>? includedExternalAddonIds)
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
            DeliveryAddress = deliveryAddress;
            AssetInfo = assetInfo;
            UserDescription = userDescription;
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
        ///     The national/legal entity's organization-number (business registration number), that identifies the organization the service-request belongs to.
        /// </summary>
        /// <example> 999990000 </example>
        public string? OrganizationNumber { get; set; }

        /// <summary>
        ///     The partner's ID.
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
        ///     The partner's national/legal entity's organization-number (business registration number).
        /// </summary>
        /// <example> 888880000 </example>
        [Required]
        public string PartnerOrganizationNumber { get; set; }

        /// <summary>
        ///     If applicable, the address that the asset should be delivery to once the service is complete. 
        ///     This is also used as the "return to sender" address in shipping labels.
        ///     
        ///     <para>
        ///     This is also used as the "return to sender" address in shipping-labels. </para>
        /// </summary>
        [Required]
        public DeliveryAddressDTO DeliveryAddress { get; set; }

        /// <summary>
        ///     Information about the asset that is sent to the service-provider.
        /// </summary>
        [Required]
        public AssetInfoDTO AssetInfo { get; set; }

        /// <summary>
        ///     A user provided description explaining the problem or reason for the service order.
        /// </summary>
        /// <example> I dropped the device, and now it has a broken screen and has problems charging. </example>
        [Required]
        public string UserDescription { get; set; }

        /// <summary>
        ///     A list containing a list of the service-providers IDs (<see cref="ServiceOrderAddonDTO.ServiceProviderId"/>), detailing
        ///     what <see cref="ServiceOrderAddonDTO"/> items to be included when the service-order is created.
        /// </summary>
        public ISet<string> IncludedExternalAddonIds { get; set; }
    }
}
