using System;
using Common.Seedwork;

namespace AssetServices.Models
{
    public class AssetCategory : Entity
    {
        // ReSharper disable once UnusedMember.Global
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
        public Guid AssetCategoryId { get; protected set; }

        public string Name { get; protected set; }

        public bool UsesImei { get; protected set; }
    }
}