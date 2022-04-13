using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.Asset
{
    /// <summary>
    /// Request object
    /// </summary>
    public class CustomerAssetValue
    {
        [Required]
        public Guid OrganizationId { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
