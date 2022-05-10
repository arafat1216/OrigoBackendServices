using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Contains the information required for registering a new repair-order with a service-provider.
    /// </summary>
    public class NewRepairOrderDTO
    {
        /// <summary>
        ///     The ID of the user that handles the service-order.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        ///     The first name of the person that handles the service-order.
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        ///     The last name of the person that handles the service-order.
        /// </summary>
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
        ///     The ID of the organization the service-request belongs to.
        /// </summary>
        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public string OrganizationName { get; set; }

        /// <summary>
        ///     The organization-number for the organization the service-request belongs to.
        /// </summary>
        public string? OrganizationNumber { get; set; }

        [Required]
        public Guid PartnerId { get; set; }

        [Required]
        public string PartnerName { get; set; }

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
        [Required]
        public string ErrorDescription { get; set; }


        [JsonConstructor]
        public NewRepairOrderDTO(Guid userId, string fistName, string lastName, string? phoneNumber, string email, Guid organizationId, string? organizationNumber, DeliveryAddressDTO deliveryAddress, AssetInfoDTO assetInfo, string errorDescription)
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
    }
}
