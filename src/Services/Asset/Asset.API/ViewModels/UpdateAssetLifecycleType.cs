using System;

namespace Asset.API.ViewModels
{
    public class UpdateAssetLifecycleType
    {
        public Guid AssetId { get; set; }
        public Guid CallerId { get; set; }
        public int LifecycleType { get; set; }
    }
}
