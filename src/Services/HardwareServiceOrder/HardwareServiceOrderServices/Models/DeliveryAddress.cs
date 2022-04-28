using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents a single shipping address.
    /// </summary>
    public class DeliveryAddress : Entity
    {
        /// <summary>
        ///     The name of the recipient. Typically this will be the name of a person or company.
        /// </summary>
        public string Recipient { get; set; }

        public string Address1 { get; set; }

        public string? Address2 { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{2}")]
        [MinLength(2)]
        [MaxLength(2)]
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
