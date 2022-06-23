using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asset.API.ViewModels
{
    public class ReportDevice
    {
        [Required]
        public Guid AssetLifeCycleId { get; set; }
        public EmailPersonAttribute? ContractHolderUser { get; set; }
        public IList<EmailPersonAttribute>? Managers { get; set; }
        public IList<EmailPersonAttribute>? CustomerAdmins { get; set; }
        public string ReportedBy { get; set; } = string.Empty;
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
        public Guid CallerId { get; set; }
    }
}
