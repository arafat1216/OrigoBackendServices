using System;

namespace Asset.API.ViewModels
{
    public record AssetCategory
    {
        /// <summary>
        /// External id of the AssetCategory
        /// </summary>
        public Guid AssetCategoryId { get; set; }

        public string Name { get; set; }

        public bool UsesImei { get; set; }
    }
}