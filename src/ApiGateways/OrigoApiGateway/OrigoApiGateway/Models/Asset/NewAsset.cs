﻿#nullable enable
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.Asset
{
    /// <summary>
    /// Request object
    /// </summary>
    public class NewAsset
    {
        /// <summary>
        /// The category this asset belongs to.
        /// </summary>
        public int AssetCategoryId { get; set; }

        /// <summary>
        /// Alias for the asset.
        /// </summary>
        public string? Alias { get; set; }

        /// <summary>
        /// A note containing additional information or comments for the asset.
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// The asset brand (e.g. Samsung)
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        public string? ProductName { get; set; }

        /// <summary>
        /// The type of lifecycle for this asset.
        /// <see cref="Common.Enums.LifecycleType">AssetStatus</see>
        /// </summary>
        public Common.Enums.LifecycleType LifecycleType { get; set; }

        /// <summary>
        /// The date the asset was purchased.
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// The department or cost center this asset is assigned to.
        /// </summary>
        public Guid? ManagedByDepartmentId { get; set; }

        /// <summary>
        /// The employee holding the asset.
        /// </summary>
        public Guid? AssetHolderId { get; set; }

        /// <summary>
        /// A description of the asset.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Person who purhcased the Asset
        /// </summary>
        public string? PurchasedBy { get; set; }

        /// <summary>
        /// Tags associated with this asset.
        /// </summary>
        public string? AssetTag { get; set; }

        /// <summary>
        /// The imei of the asset. Applicable to assets with category Mobile Phone and Tablet.
        /// </summary>
        public IList<long>? Imei { get; set; }

        /// <summary>
        /// The mac address of the asset. Applicable to assets with category Mobile Phone and Tablet.
        /// </summary>
        public string? MacAddress { get; set; }

        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        public string? SerialNumber { get; set; }
        /// <summary>
        /// Is a personal or non-personal asset.
        /// </summary>
        public bool? IsPersonal { get; set; } = true;

        public string? Source { get; set; } = string.Empty;
    }
}