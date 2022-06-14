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
    }
}
