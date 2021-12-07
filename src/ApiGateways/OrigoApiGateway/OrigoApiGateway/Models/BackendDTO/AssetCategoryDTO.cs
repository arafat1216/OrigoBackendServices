using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class AssetCategoryDTO
    {
        public int AssetCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int? ParentId { get; set; }
        public IList<AssetCategoryDTO> ChildAssetCategory { get; set; }
    }
}
