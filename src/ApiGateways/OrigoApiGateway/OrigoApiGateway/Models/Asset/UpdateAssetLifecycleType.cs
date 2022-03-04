using System;

namespace OrigoApiGateway.Models.Asset
{
    /// <summary>
    /// Request object.
    /// Information required to update an asset's lifecycletype
    /// </summary>
    public class UpdateAssetLifecycleType
    {
        public Guid AssetId { get; set; }
        public Guid CallerId { get; set; }
        public int LifecycleType { get; set; }
    }
}
