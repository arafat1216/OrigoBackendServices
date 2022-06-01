using HardwareServiceOrderServices.Models;
using System.ComponentModel.DataAnnotations;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <inheritdoc cref="Models.DeliveryAddress"/>
    public class DeliveryAddressDTO : IValidatableObject
    {
        [Required]
        [EnumDataType(typeof(RecipientTypeEnum))]
        public RecipientTypeEnum RecipientType { get; set; }

        /// <example> MyPartner A/S </example>
        /// <inheritdoc cref="DeliveryAddress.Recipient"/>
        [Required]
        public string Recipient { get; set; }

        /// <example> CoolStreet 89A </example>
        /// <inheritdoc cref="DeliveryAddress.Address1"/>
        [Required]
        public string Address1 { get; set; }

        /// <example> C/O: John Doe </example>
        /// <inheritdoc cref="DeliveryAddress.Address2"/>
        public string? Address2 { get; set; }

        /// <example> 0279 </example>
        /// <inheritdoc cref="DeliveryAddress.PostalCode"/>
        [Required]
        [StringLength(maximumLength: 12)]
        public string PostalCode { get; set; }

        /// <example> Oslo </example>
        /// <inheritdoc cref="DeliveryAddress.City"/>
        [Required]
        [StringLength(maximumLength: 85)]
        public string City { get; set; }

        /// <summary>
        ///     Backing field for <see cref="Country"/>.
        /// </summary>
        private string _country = null!;

        /// <example> NO </example>
        /// <inheritdoc cref="DeliveryAddress.Country"/>
        [Required]
        [RegularExpression("^[A-Z]{2}$")]
        [StringLength(maximumLength: 2, MinimumLength = 2)]
        public string Country
        {
            get { return _country; }
            set { _country = value.ToUpperInvariant(); }
        }


        /// <summary>
        ///     Reserved constructor for JSON serializers
        /// </summary>
        public DeliveryAddressDTO()
        {

        }


        public DeliveryAddressDTO(RecipientTypeEnum recipientType, string recipient, string address1, string? address2, string postalCode, string city, string country)
        {
            RecipientType = recipientType;
            Recipient = recipient;
            Address1 = address1;
            Address2 = address2;
            PostalCode = postalCode;
            City = city;
            Country = country;
        }


        private static ValidationResult ValidateRecipientType(RecipientTypeEnum value, ValidationContext context)
        {
            if (value == RecipientTypeEnum.Null)
            {
                return new ValidationResult($"The value {value} is not valid.", new List<string>() { context.MemberName });
            }

            // Nothing triggered, so it's OK!
            return ValidationResult.Success;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (RecipientType == RecipientTypeEnum.Null)
            {
                yield return new ValidationResult($"The value '{(int)RecipientType}' is not valid.", new List<string>() { nameof(RecipientType) });
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
