using System;

namespace OrigoApiGateway.Models.Asset
{
    /// <summary>
    /// Request object
    /// </summary>
    public class AssignAssetToUser
    {
        public Guid AssetId { get; set; }
        public Guid? UserId { get; set; }
        public Guid CallerId { get; set; }
    }
}
