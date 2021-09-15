﻿using OrigoApiGateway.Models.BackendDTO;
using System;

namespace OrigoApiGateway.Models
{
    public class OrigoAssetCategory
    {
        public OrigoAssetCategory(AssetCategoryDTO assetCategory)
        {
            AssetCategoryId = assetCategory.AssetCategoryId;
            Name = assetCategory.Name;
            UsesImei = assetCategory.UsesImei;
        }
        /// <summary>
        /// External id of the AssetCategory
        /// </summary>
        public Guid AssetCategoryId { get; set; }

        public string Name { get; set; }

        public bool UsesImei { get; set; }
    }
}
