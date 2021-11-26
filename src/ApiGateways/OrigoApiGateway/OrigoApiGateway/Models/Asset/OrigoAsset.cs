using Common.Enums;
using OrigoApiGateway.Models.BackendDTO;
using System;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace OrigoApiGateway.Models
{
    public abstract class OrigoAsset
    {
        protected OrigoAsset() { }

        protected OrigoAsset(AssetDTO asset)
        {
            Id = asset.Id;
            OrganizationId = asset.OrganizationId;
            Alias = asset.Alias;
            Note = asset.Note;
            AssetCategoryId = asset.AssetCategoryId;
            AssetCategoryName = asset.AssetCategoryName;
            Brand = asset.Brand;
            ProductName = asset.ProductName;
            LifecycleType = asset.LifecycleType;
            LifeCycleName = asset.LifecycleName;
            PurchaseDate = asset.PurchaseDate;
            CreatedDate = asset.CreatedDate;
            ManagedByDepartmentId = asset.ManagedByDepartmentId;
            AssetHolderId = asset.AssetHolderId;
            AssetStatus = asset.AssetStatus;
            AssetStatusName = asset.AssetStatusName;
        }

        /// <summary>
        /// Asset is linked to this customer 
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
        public int LifecycleType { get; protected set; }

        /// <summary>
        /// The name of the lifecycle for this asset.
        /// </summary>
        public string LifeCycleName { get; protected set; }

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

        public string AssetStatusName { get; protected set; }
    }
}
