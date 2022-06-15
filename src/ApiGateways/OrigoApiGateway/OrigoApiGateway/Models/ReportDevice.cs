using Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models
{
    public class ReportDevice
    {
        [Required]
        public Guid AssetId { get; set; }
        [Required]
        public ReportCategory ReportCategory { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime TimePeriodFrom { get; set; }
        [Required]
        public DateTime TimePeriodTo { get; set; }
        /// <summary>
        ///     The country, using the uppercase <c>ISO 3166-1 (alpha-2)</c> standard.
        /// </summary>
        /// <example> US </example>
        [RegularExpression("^[a-zA-Z]{2}")] // Exactly 2 characters
        [Required]
        public string Country { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string City { get; set; }
    }
}
