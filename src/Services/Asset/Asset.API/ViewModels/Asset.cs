using System;
using AssetServices.Models;
using Common.Enums;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Asset.API.ViewModels
{
    public record Asset
    {
        public Asset()
        {

        }

        public Asset(AssetServices.Models.Asset asset)
        {
            AssetId = asset.AssetId;
            CustomerId = asset.CustomerId;
            AssetCategoryId = asset.AssetCategoryId;
            SerialNumber = asset.SerialNumber;
            AssetCategoryName = asset.AssetCategory.Name;
            Brand = asset.Brand;
            Model = asset.Model;
            LifecycleType = asset.LifecycleType;
            PurchaseDate = asset.PurchaseDate;
            CreatedDate = asset.CreatedDate;
            ManagedByDepartmentId = asset.ManagedByDepartmentId;
            Imei = asset.Imei;
            MacAddress = asset.MacAddress;
            AssetHolderId = asset.AssetHolderId;
            IsActive = asset.IsActive;
            AssetStatus = asset.Status;
        }

        /// <summary>
        /// External Id of the Asset
        /// </summary>
        public Guid AssetId { get; protected set; }

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
        public string AssetCategoryName { get; protected set;  }

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
        public LifecycleType LifecycleType { get; protected set; }

        /// <summary>
        /// The name of the lifecycle for this asset.
        /// </summary>
        public string LifecycleName
        { 
            get
            {
                return LifecycleType.GetName<LifecycleType>(LifecycleType);
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
        /// The imei of the asset. Applicable to assets with category Mobile phone.
        /// </summary>
        public string Imei { get; protected set; }

        /// <summary>
        /// The mac address of the asset
        /// </summary>
        public string MacAddress { get; protected set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// The status of the asset.
        /// It can have these values:
        /// - NoStatus,
        /// - Active,
        /// - Inactive,
        /// - OnRepair,
        /// - InputRequired
        /// </summary>
        public AssetStatus AssetStatus { get; protected set; }

        public string AssetStatusName 
        { 
            get 
            {
                return AssetStatus.GetName<AssetStatus>(AssetStatus);
            } 
        }
    }
}
