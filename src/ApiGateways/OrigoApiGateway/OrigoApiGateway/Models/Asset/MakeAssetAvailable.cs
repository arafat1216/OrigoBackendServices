using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.Asset
{
    /// <summary>
    /// A Assets ID (Guid), that will be made available.
    /// </summary>
    public class MakeAssetAvailable
    {
        [Required]
        public Guid AssetLifeCycleId { get; set; }
    }
}
