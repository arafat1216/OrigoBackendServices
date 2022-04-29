using System.ComponentModel.DataAnnotations;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <inheritdoc cref="Models.DeliveryAddress"/>
    public class DeliveryAddressDTO
    {
        /// <inheritdoc cref="Models.DeliveryAddress.Recipient"/>
        [Required]
        public string Recipient { get; set; }

        /// <inheritdoc cref="Models.DeliveryAddress.Address1"/>
        [Required]
        public string Address1 { get; set; }

        /// <inheritdoc cref="Models.DeliveryAddress.Address2"/>
        public string? Address2 { get; set; }

        /// <inheritdoc cref="Models.DeliveryAddress.PostalCode"/>
        [Required]
        [StringLength(maximumLength: 12)]
        public string PostalCode { get; set; }

        /// <inheritdoc cref="Models.DeliveryAddress.City"/>
        [Required]
        [StringLength(maximumLength: 85)]
        public string City { get; set; }

        /// <summary>
        ///     Backing field for <see cref="Country"/>.
        /// </summary>
        private string _country = null!;

        /// <inheritdoc cref="Models.DeliveryAddress.Country"/>
        [Required]
        [RegularExpression("^[A-Z]{2}")]
        [StringLength(maximumLength: 2, MinimumLength = 2)]
        public string Country
        {
            get { return _country; }
            set { _country = value.ToUpperInvariant(); }
        }
    }
}
