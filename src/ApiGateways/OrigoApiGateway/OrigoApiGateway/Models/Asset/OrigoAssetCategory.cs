using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrigoApiGateway.Models
{
    public class OrigoAssetCategory
    {
        public int AssetCategoryId { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string Language { get; init; }
        public int? ParentId { get; init; }
        public IList<OrigoAssetCategory> ChildAssetCategory { get; init; }
        public IList<AssetCategoryAttribute> AssetCategoryAttributes { get; set; }
    }
}
