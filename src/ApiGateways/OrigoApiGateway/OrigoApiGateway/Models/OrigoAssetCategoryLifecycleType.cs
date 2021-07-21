using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class OrigoAssetCategoryLifecycleType
    {
        public Guid AssetCategoryLifecycleId { get; set; }

        public string Name { get; set; }

        public bool IsChecked { get; set; }

        public OrigoAssetCategoryLifecycleType(AssetCategoryLifecycleTypeDTO assetCategoryLifecycleType)
        {
            AssetCategoryLifecycleId = assetCategoryLifecycleType.AssetCategoryLifecycleId;
            Name = assetCategoryLifecycleType.Name;
        }

        public OrigoAssetCategoryLifecycleType(AssetCategoryLifecycleTypeDTO assetCategoryLifecycleType, IList<OrigoAssetCategoryLifecycleType> selctedModules)
        {
            AssetCategoryLifecycleId = assetCategoryLifecycleType.AssetCategoryLifecycleId;
            Name = assetCategoryLifecycleType.Name;
            IsChecked = selctedModules?.FirstOrDefault(p => p.AssetCategoryLifecycleId == AssetCategoryLifecycleId) != null;
        }
    }
}
