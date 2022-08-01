using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.Asset
{
    /// <summary>
    /// A Assets ID (Guid), that will be bought.
    /// </summary>
    public class BuyoutDevice
    {
        [Required]
        public Guid AssetId { get; set; }
        [RegularExpression("^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}$", ErrorMessage = "Invalid Email address!!")]
        public string PayrollContactEmail { get; set; } = string.Empty;
    }
}
