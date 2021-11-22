using Common.Enums;
using OrigoApiGateway.Models.BackendDTO;
using System;

namespace OrigoApiGateway.Models
{
    public class OrigoAssetCategoryLifecycleType
    {
        public string Name { get; set; }

        public int AssetCategoryId { get; set; }

        public LifecycleType LifecycleType { get; set; }

        public bool IsChecked { get; set; }

        public OrigoAssetCategoryLifecycleType() { }

        public OrigoAssetCategoryLifecycleType(AssetCategoryLifecycleTypeDTO assetCategoryLifecycleType)
        {
            AssetCategoryId = assetCategoryLifecycleType.AssetCategoryId;
            Name = assetCategoryLifecycleType.Name;
            LifecycleType = assetCategoryLifecycleType.LifecycleType;
        }
    }
}
