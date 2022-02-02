using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.Asset
{
    /// <summary>
    /// Request object
    /// </summary>
    public class CustomerAssetCount
    {
        [Required]
        public Guid OrganizationId { get; set; }
        [Required]
        public int Count { get; set; }
    }
}
