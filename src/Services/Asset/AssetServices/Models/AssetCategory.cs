using System;
using System.Collections.Generic;
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
        }

        /// <summary>
        /// External id of the AssetCategory
        /// </summary>
        public Guid AssetCategoryId { get; protected set; }

        public AssetCategory ParentAssetCategory { get; protected set; }

        public IList<AssetCategoryTranslation> Translations { get; set; }

        /// <summary>
        /// Returns a list of all subdepartments of this department
        /// </summary>
        /// <returns></returns>
        public IList<AssetCategory> SubCategory(IList<AssetCategory> categories)
        {
            List<AssetCategory> subCategories = new();
            foreach (var category in categories)
            {
                if (category?.ParentAssetCategory?.AssetCategoryId == AssetCategoryId)
                    subCategories.Add(category);
            }
            return subCategories;
        }
    }
}