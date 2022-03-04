using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Asset.API.ViewModels
{
    public record Asset
    {
        public Asset()
        {
            // So much death.. What can men do agains such reckless hate. - King Théoden
        }

        public Asset(AssetServices.Models.Asset asset)
        {
            AssetTag = asset.AssetTag;
            Description = asset.Description;
            Id = asset.ExternalId;
            OrganizationId = asset.CustomerId;
            Alias = asset.Alias;
            Brand = asset.Brand;
            ProductName = asset.ProductName;
            LifecycleType = asset.LifecycleType;
            PurchaseDate = asset.PurchaseDate;
            CreatedDate = asset.CreatedDate;
            AssetHolderId = asset.AssetHolderId;
            ManagedByDepartmentId = asset.ManagedByDepartmentId;
            AssetStatus = asset.Status;
            Note = asset.Note;
            AssetStatus = asset.Status;
            CreatedDate = asset.CreatedDate;
            AssetCategoryId = asset.AssetCategory.Id;
            AssetCategoryName = asset.AssetCategory?.Translations?.FirstOrDefault(a => a.Language == "EN")?.Name;
            SetLabels(asset.AssetLabels);
        }

        /// <summary>
        /// External Id of the Asset
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Asset is linked to this customer 
        /// </summary>
        public Guid OrganizationId { get; protected set; }

        /// <summary>
        /// Alias for the asset.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// A note containing additional information or comments for the asset.
        /// </summary>
        public string Note { get; protected set; }

        /// <summary>
        /// A description of the asset.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Tags associated with this asset.
        /// </summary>
        public string AssetTag { get; protected set; }

        /// <summary>
        /// Asset is linked to this category
        /// </summary>
        public int AssetCategoryId { get; protected set; }

        /// <summary>
        /// The category this asset belongs to.
        /// </summary>
        public string AssetCategoryName { get; protected set; }

        /// <summary>
        /// The asset brand (e.g. Samsung)
        /// </summary>
        public string Brand { get; protected set; }

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        public string ProductName { get; protected set; }

        /// <summary>
        /// The type of lifecycle for this asset.
        /// </summary>
        public LifecycleType LifecycleType { get; protected set; }

        /// <summary>
        /// The name of the lifecycle for this asset.
        /// </summary>
        public string LifecycleName
        {
            get
            {
                return Enum.GetName(LifecycleType);
            }
        }

        /// <summary>
        /// The date the asset was purchased.
        /// </summary>
        public DateTime PurchaseDate { get; protected set; }

        /// <summary>
        /// The date the asset was registered/created.
        /// </summary>
        public DateTime CreatedDate { get; protected set; }

        /// <summary>
        /// The department or cost center this asset is assigned to.
        /// </summary>
        public Guid? ManagedByDepartmentId { get; protected set; }

        /// <summary>
        /// The employee holding the asset.
        /// </summary>
        public Guid? AssetHolderId { get; protected set; }

        /// <summary>
        /// The status of the asset.
        /// <see cref="Common.Enums.AssetStatus">AssetStatus</see>
        /// </summary>
        public AssetStatus AssetStatus { get; protected set; }

        public string AssetStatusName
        {
            get
            {
                return Enum.GetName(AssetStatus);
            }
        }

        public IList<Label> Labels { get; protected set; }

        public void SetLabels(ICollection<AssetServices.Models.AssetLabel> assetLabels)
        {
            IList<Label> labelList = new List<Label>();
            foreach (AssetServices.Models.AssetLabel al in assetLabels)
            {
                labelList.Add(new Label(al.Label));
            }
            Labels = labelList;
        }
    }
}
