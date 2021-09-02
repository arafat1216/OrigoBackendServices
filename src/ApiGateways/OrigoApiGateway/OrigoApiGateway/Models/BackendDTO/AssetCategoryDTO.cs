using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class AssetCategoryDTO
    {
        /// <summary>
        /// External id of the AssetCategory
        /// </summary>
        public Guid AssetCategoryId { get; set; }

        public string Name { get; set; }

        public bool UsesImei { get; set; }
    }
}
