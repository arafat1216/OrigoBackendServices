using HardwareServiceOrderServices.Models;
using System.ComponentModel.DataAnnotations;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <inheritdoc cref="Models.DeliveryAddress"/>
    public class DeliveryAddressDTO
    {
        [Required]
        public RecipientTypeEnum RecipientType { get; set; }

        /// <example> MyPartner A/S </example>
        /// <inheritdoc cref="Models.DeliveryAddress.Recipient"/>
        [Required]
        public string Recipient { get; set; }

        /// <example> CoolStreet 89A </example>
        /// <inheritdoc cref="Models.DeliveryAddress.Address1"/>
        [Required]
        public string Address1 { get; set; }

        /// <example> C/O: John Doe </example>
        /// <inheritdoc cref="Models.DeliveryAddress.Address2"/>
        public string? Address2 { get; set; }

        /// <example> 0279 </example>
        /// <inheritdoc cref="Models.DeliveryAddress.PostalCode"/>
        [Required]
        [StringLength(maximumLength: 12)]
        public string PostalCode { get; set; }

        /// <example> Oslo </example>
        /// <inheritdoc cref="Models.DeliveryAddress.City"/>
        [Required]
        [StringLength(maximumLength: 85)]
        public string City { get; set; }

        /// <summary>
        ///     Backing field for <see cref="Country"/>.
        /// </summary>
        private string _country = null!;

        /// <example> NO </example>
        /// <inheritdoc cref="Models.DeliveryAddress.Country"/>
        [Required]
        [RegularExpression("^[A-Z]{2}$")]
        [StringLength(maximumLength: 2, MinimumLength = 2)]
        public string Country
        {
            get { return _country; }
            set { _country = value.ToUpperInvariant(); }
        }
    }
}
