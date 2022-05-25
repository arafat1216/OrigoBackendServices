using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Contains the information required for registering a new repair-order with an external service-provider.
    /// </summary>
    public class NewExternalRepairOrderDTO
    {
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
        ///     The address that the asset should be shipped to once the service is complete.
        /// </summary>
        [Required]
        public DeliveryAddressDTO DeliveryAddress { get; set; }

        /// <summary>
        ///     Information about the product.
        /// </summary>
        [Required]
        public AssetInfoDTO AssetInfo { get; set; }

        /// <summary>
        ///     The user's description of what's wrong with the device.
        /// </summary>
        /// <example> I dropped the device, and now it has a broken screen and has problems charging. </example>
        [Required]
        public string ErrorDescription { get; set; }


        /// <summary>
        ///     Reserved JSON (de-)serializer constructor.
        /// </summary>
        public NewExternalRepairOrderDTO()
        {

        }


        public NewExternalRepairOrderDTO(Guid userId, string fistName, string lastName, string? phoneNumber, string email, Guid organizationId, string? organizationNumber, DeliveryAddressDTO deliveryAddress, AssetInfoDTO assetInfo, string errorDescription)
        {
            UserId = userId;
            FirstName = fistName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
            DeliveryAddress = deliveryAddress;
            AssetInfo = assetInfo;
            ErrorDescription = errorDescription;
        }

        // TODO: Should we introduce Levenshtein distance or Hamming distance to allow for typos? Or should we change it so this is provided by the caller?
        /// <summary>
        ///     Attempts to determine if the <see cref="DeliveryAddress"/> is for the company or the user.
        /// </summary>
        /// <returns> Returns <see langword="true"/> if the delivery address contains the <see cref="OrganizationName"/> or <see langword="false"/> if
        ///     it contains either the <see cref="FirstName"/> or <see cref="LastName"/>. If we can't find either, meaning it's likely a custom shipping address,
        ///     then <see langword="null"/> is returned. </returns>
        public bool? HasCompanyDeliveryAddress()
        {
            // If it contains the company name, it's likely the company that's the receiver.
            if (DeliveryAddress.Recipient.Contains(OrganizationName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            // If not, it's likely the user if parts of their name exist as a part of the receiver's name.
            else if (DeliveryAddress.Recipient.Contains(FirstName, StringComparison.OrdinalIgnoreCase) || DeliveryAddress.Recipient.Contains(LastName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            // If none of the above, it's likely a custom delivery address, meaning it's not going to the attached organization
            else
            {
                return null;
            }
        }
    }
}
