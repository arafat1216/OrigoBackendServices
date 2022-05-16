using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HardwareServiceOrderServices.Models
{
    [Owned]
    /// <summary>
    ///     Represents a single shipping address.
    /// </summary>
    public class DeliveryAddress
    {
        /// <summary>
        ///     The name of the recipient. Typically this will be the name of a person or company.
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        ///     Mandatory 1st address line.
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        ///     Optional 2nd address line.
        /// </summary>
        public string? Address2 { get; set; }

        /// <summary>
        ///     The addresses zip/postal code.
        /// </summary>
        [Required]
        [StringLength(maximumLength: 12)] // The longest worldwide postal-code.
        public string PostalCode { get; set; }

        /// <summary>
        ///     The name of the city/town/village.
        /// </summary>
        [StringLength(maximumLength: 85)] // The longest worldwide place/city name
        public string City { get; set; }

        /// <summary>
        ///     The 2-character country-code using the uppercase <c>ISO 3166 alpha-2</c> standard.
        /// </summary>
        [Required]
        [RegularExpression("^[A-Z]{2}")]
        [StringLength(maximumLength: 2, MinimumLength = 2)]
        public string Country { get; set; }

        /// <summary>
        ///     Added to prevent Entity framework No suitable constructor found exception.
        /// </summary>
        private DeliveryAddress()
        {
        }

        public DeliveryAddress(string recipient, string description, string address1, string address2, string postalCode, string city, string country)
        {
            Recipient = recipient;
            Address1 = address1;
            Address2 = address2;
            PostalCode = postalCode;
            City = city;
            Country = country;
        }

    }
}
