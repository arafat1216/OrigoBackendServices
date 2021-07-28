﻿using Common.Enums;
using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class AssetDTO
    {

        /// <summary>
        /// External Id of the Asset
        /// </summary>
        public Guid AssetId { get; set; }

        /// <summary>
        /// Asset is linked to this customer 
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Id of category this asset belongs to.
        /// </summary>
        public int AssetCategoryId { get; set; }

        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// The category this asset belongs to.
        /// </summary>
        public string AssetCategoryName { get; set; }

        /// <summary>
        /// The asset brand (e.g. Samsung)
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// The type of lifecycle for this asset.
        /// </summary>
        public int LifecycleType { get; set; }

        /// <summary>
        /// The name of the lifecycle for this asset.
        /// </summary>
        public string LifecycleName { get; set; }

        /// <summary>
        /// The date the asset was purchased.
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// The date the asset was registered/created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// The department or cost center this asset is assigned to.
        /// </summary>
        public Guid? ManagedByDepartmentId { get; set; }

        /// <summary>
        /// The employee holding the asset.
        /// </summary>
        public Guid? AssetHolderId { get; set; }

        /// <summary>
        /// The imei(s) of the asset.
        /// A comma separated string, where each instance is an imei.
        /// Applicable to assets with category type Mobile Phones.
        /// </summary>
        public string Imei { get; set; }

        /// <summary>
        /// The mac address of the asset
        /// </summary>
        public string MacAddress { get; set; }

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
        public AssetStatus AssetStatus { get; set; }

        public string AssetStatusName { get; set; }
    }
}
