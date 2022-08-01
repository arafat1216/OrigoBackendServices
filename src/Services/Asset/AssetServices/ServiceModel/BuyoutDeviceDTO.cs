using System;
using System.ComponentModel.DataAnnotations;

namespace AssetServices.ServiceModel
{
    public class BuyoutDeviceDTO
    {
        [Required]
        public Guid AssetLifeCycleId { get; set; }
        [RegularExpression("^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}$", ErrorMessage = "Invalid Email address!!")]
        public string PayrollContactEmail { get; set; } = string.Empty;
        public Guid CallerId { get; set; }
    }
}
