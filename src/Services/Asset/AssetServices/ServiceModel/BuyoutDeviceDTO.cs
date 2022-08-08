using System;
using System.ComponentModel.DataAnnotations;

namespace AssetServices.ServiceModel
{
    public class BuyoutDeviceDTO
    {
        [Required]
        public Guid AssetLifeCycleId { get; set; }
        [EmailAddress]
        public string? PayrollContactEmail { get; set; } = null;
        public Guid CallerId { get; set; }
    }
}
