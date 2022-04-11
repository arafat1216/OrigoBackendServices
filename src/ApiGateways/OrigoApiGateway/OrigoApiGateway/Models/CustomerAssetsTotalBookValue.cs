using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public class CustomerAssetsTotalBookValue
    {
        [Required]
        public Guid OrganizationId { get; set; }
        [Required]
        public int AssetsTotalBookValue { get; set; }
    }
}
