using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models
{
    public class OfficeLocation
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }

        /// <summary>
        ///     The country, using the uppercase <c>ISO 3166-1 (alpha-2)</c> standard.
        /// </summary>
        /// <example> US </example>
        [RegularExpression("^[a-zA-Z]{2}")] // Exactly 2 characters
        [Required]
        public string Country { get; set; }
        public bool IsPrimary { get; set; } = false;
    }
}
