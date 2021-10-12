using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrigoApiGateway.Models
{
    public class OrigoAssetCategory
    {
        public OrigoAssetCategory(AssetCategoryDTO assetCategory)
        {
            AssetCategoryId = assetCategory.AssetCategoryId;
            Name = assetCategory.Name;
            UsesImei = assetCategory.UsesImei;
            ChildAssetCategory = assetCategory.ChildAssetCategory?.Select(c => new OrigoAssetCategory(c)).ToList();
        }

        /// <summary>
        /// External id of the AssetCategory
        /// </summary>
        public Guid AssetCategoryId { get; set; }

        public string Name { get; set; }

        public bool UsesImei { get; set; }

        public IList<OrigoAssetCategory> ChildAssetCategory { get; set; }
    }
}
