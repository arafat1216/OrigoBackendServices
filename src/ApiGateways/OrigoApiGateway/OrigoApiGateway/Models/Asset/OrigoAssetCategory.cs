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
            Description = assetCategory.Description;
            Language = assetCategory.Language;
            ParentId = assetCategory.ParentId;
            ChildAssetCategory = assetCategory.ChildAssetCategory?.Select(c => new OrigoAssetCategory(c)).ToList();
        }
        public int AssetCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int? ParentId { get; set; }
        public IList<OrigoAssetCategory> ChildAssetCategory { get; set; }
    }
}
