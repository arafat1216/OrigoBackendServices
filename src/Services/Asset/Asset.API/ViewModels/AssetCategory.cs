using System;
using System.Collections.Generic;
using System.Linq;

namespace Asset.API.ViewModels
{
    public record AssetCategory
    {
        public AssetCategory(AssetServices.Models.AssetCategory assetCategory)
        {
            if (assetCategory == null)
                return;
            var translation = assetCategory.Translations.First();
            AssetCategoryId = assetCategory.Id;
            Name = translation.Name;
            Description = translation.Description;
            Language = translation.Language;
            ParentId = assetCategory.ParentAssetCategory?.Id;
            ChildAssetCategory = null;
        }

        public AssetCategory(AssetServices.Models.AssetCategory assetCategory, IList<AssetServices.Models.AssetCategory> assetCategories)
        {
            if (assetCategory == null)
                return;
            var translation = assetCategory.Translations.First();
            AssetCategoryId = assetCategory.Id;
            Name = translation.Name;
            Description = translation.Description;
            Language = translation.Language;
            ParentId = assetCategory.ParentAssetCategory?.Id;
            ChildAssetCategory = assetCategory.SubCategory(assetCategories).Select(ac => new AssetCategory(ac, assetCategories)).ToList();
        }

        public int AssetCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int? ParentId { get; set; }
        public IList<AssetCategory> ChildAssetCategory { get; set; }
    }
}