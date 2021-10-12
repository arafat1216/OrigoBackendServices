using System;
using System.Collections.Generic;
using System.Linq;

namespace Asset.API.ViewModels
{
    public record AssetCategory
    {
        public AssetCategory(AssetServices.Models.AssetCategory assetCategory, IList<AssetServices.Models.AssetCategory> assetCategories)
        {
            if (assetCategory == null)
                return;
            AssetCategoryId = assetCategory.AssetCategoryId;
            Name = assetCategory.Name;
            UsesImei = assetCategory.UsesImei;
            ChildAssetCategory = assetCategory.SubCategory(assetCategories).Select(ac => new AssetCategory(ac, assetCategories)).ToList();
        }

        /// <summary>
        /// External id of the AssetCategory
        /// </summary>
        public Guid AssetCategoryId { get; set; }

        public string Name { get; set; }

        public bool UsesImei { get; set; }

        public IList<AssetCategory> ChildAssetCategory { get; set; }
    }
}