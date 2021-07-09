using System;
using OrigoApiGateway.Models.BackendDTO;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace OrigoApiGateway.Models
{
    public class OrigoAsset
    {
        public OrigoAsset(AssetDTO asset)
        {
            Id = asset.AssetId;
            CustomerId = asset.CustomerId;
            AssetCategoryId = asset.AssetCategoryId;
            SerialNumber = asset.SerialNumber;
            AssetCategoryName = asset.AssetCategoryName;
            Brand = asset.Brand;
            Model = asset.Model;
            LifecycleType = asset.LifecycleType;
            LifeCycleName = asset.LifecycleName;
            PurchaseDate = asset.PurchaseDate;
            CreatedDate = asset.CreatedDate;
            ManagedByDepartmentId = asset.ManagedByDepartmentId;
            AssetHolderId = asset.AssetHolderId;
            IsActive = asset.IsActive;
        }

        /// <summary>
        /// Asset is linked to this customer 
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Asset is linked to this customer 
        /// </summary>
        public Guid CustomerId { get; protected set; }

        /// <summary>
        /// Asset is linked to this category
        /// </summary>
        public int AssetCategoryId { get; protected set; }

        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        public string SerialNumber { get; protected set; }

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
        public string Model { get; protected set; }

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
        /// The imei of the device. Applicable to devices with category Mobile device.
        /// </summary>
        public string Imei { get; protected set; }

        /// <summary>
        /// The mac address of the device.
        /// </summary>
        public string MacAddress { get; protected set; }

        public bool IsActive { get; set; }

    }
}
