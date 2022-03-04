using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class AssetCategoryDTO
    {
        public int AssetCategoryId { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string Language { get; init; }
        public int? ParentId { get; init; }
        public IList<AssetCategoryDTO> ChildAssetCategory { get; init; }
    }
}
