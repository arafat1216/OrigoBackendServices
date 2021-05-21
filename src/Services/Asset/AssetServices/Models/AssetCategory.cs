using System;
using System.Collections.Generic;
using Common.Seedwork;

namespace AssetServices.Models
{
    public class AssetCategory : Entity
    {
        protected AssetCategory()
        {
        }

        public AssetCategory(Guid assetCategoryId, string name, bool usesImei)
        {
            AssetCategoryId = assetCategoryId;
            Name = name;
            UsesImei = usesImei;
        }

        /// <summary>
        /// External id of the AssetCategory
        /// </summary>
        public Guid AssetCategoryId { get; set; }

        public string Name { get; set; }

        public bool UsesImei { get; set; }

        public List<Asset> Assets { get; set; }
    }
}