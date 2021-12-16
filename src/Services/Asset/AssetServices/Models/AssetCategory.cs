using System;
using System.Collections.Generic;
using Common.Seedwork;

namespace AssetServices.Models
{
    public class AssetCategory : Entity
    {
        // ReSharper disable once UnusedMember.Global
        public AssetCategory()
        {
        }

       /* public AssetCategory(AssetCategory parentAssetCategory, IList<AssetCategoryTranslation> translations, int id, DateTime createdDate, Guid createdBy, DateTime lastUpdatedDate, Guid updatedBy, bool isDeleted, Guid deletedBy)
        {
            ParentAssetCategory = parentAssetCategory;
            Translations = translations;
            Id = id;
            CreatedDate = createdDate;
            CreatedBy = createdBy;
            LastUpdatedDate = lastUpdatedDate;
            UpdatedBy = updatedBy;
            IsDeleted = isDeleted;
            DeletedBy = deletedBy;
        }*/

        public AssetCategory(int id, AssetCategory parent, IList<AssetCategoryTranslation> translations)
        {
            Id = id;
            ParentAssetCategory = parent;
            Translations = translations;
        }

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
                if (category?.ParentAssetCategory?.Id == Id)
                    subCategories.Add(category);
            }
            return subCategories;
        }
    }
}