using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.Asset
{
    /// <summary>
    /// A Assets ID (Guid), that will be returned.
    /// </summary>
    public class ReturnAsset
    {
        [Required]
        public Guid AssetId { get; set; }
        public Guid ReturnLocationId { get; set; } = Guid.Empty;
    }
}
