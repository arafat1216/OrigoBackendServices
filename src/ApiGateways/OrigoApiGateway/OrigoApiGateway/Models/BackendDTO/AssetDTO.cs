﻿using Common.Enums;
using Common.Model;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class AssetDTO
    {

        /// <summary>
        /// External Id of the Asset
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Asset is linked to this customer 
        /// </summary>
        public Guid OrganizationId { get; init; }

        /// <summary>
        /// Alias for the asset.
        /// </summary>
        public string Alias { get; init; }

        /// <summary>
        /// A note containing additional information or comments for the asset.
        /// </summary>
        public string Note { get; init; }

        /// <summary>
        /// Id of category this asset belongs to.
        /// </summary>
        public int AssetCategoryId { get; init; }

        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        public string SerialNumber { get; init; }

        /// <summary>
        /// The category this asset belongs to.
        /// </summary>
        public string AssetCategoryName { get; init; }

        /// <summary>
        /// The asset brand (e.g. Samsung)
        /// </summary>
        public string Brand { get; init; }

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        public string ProductName { get; init; }

        /// <summary>
        /// The type of lifecycle for this asset.
        /// </summary>
        public int LifecycleType { get; init; }

        /// <summary>
        /// The name of the lifecycle for this asset.
        /// </summary>
        public string LifecycleName { get; init; }

        /// <summary>
        /// The date the asset was purchased.
        /// </summary>
        public DateTime PurchaseDate { get; init; }

        /// <summary>
        /// The date the asset was registered/created.
        /// </summary>
        public DateTime CreatedDate { get; init; }

        /// <summary>
        /// The start period for this asset lifecycle.
        /// </summary>
        public DateTime? StartPeriod { get; init; }
        /// <summary>
        /// The end period for this asset lifecycle.
        /// </summary>
        public DateTime? EndPeriod { get; init; }

        /// <summary>
        /// The department or cost center this asset is assigned to.
        /// </summary>
        public Guid? ManagedByDepartmentId { get; init; }

        /// <summary>
        /// The employee holding the asset.
        /// </summary>
        public Guid? AssetHolderId { get; init; }

        /// <summary>
        /// The imei(s) of the asset.
        /// A comma separated string, where each instance is an imei.
        /// Applicable to assets with category type Mobile Phones.
        /// </summary>
        public IList<long> Imei { get; init; }

        /// <summary>
        /// The mac address of the asset
        /// </summary>
        public string MacAddress { get; init; }

        /// <summary>
        /// A description of the asset.
        /// </summary>
        public string Description { get; init; }
        /// <summary>
        /// Person who purhcased the Asset
        /// </summary>
        public string PurchasedBy { get; init; }

        /// <summary>
        /// The payment amount by company for the asset. For mobile phones and other devices
        /// This is the amount that company covered/paid for the asset's overall cost
        /// </summary>
        public Money PaidByCompany { get; init; }

        /// <summary>
        /// Calculated Book Value for the asset's overall cost.
        /// </summary>
        public decimal BookValue { get; init; }

        /// <summary>
        /// Calculated BuyOut price for the asset.
        /// </summary>
        public decimal BuyoutPrice { get; init; }

        /// <summary>
        /// Tags associated with this asset.
        /// </summary>
        public string AssetTag { get; init; }

        /// <summary>
        /// The status of the asset.
        /// <see cref="AssetStatus">AssetStatus</see>
        /// </summary>
        public AssetLifecycleStatus AssetStatus { get; init; }

        public string AssetStatusName { get; init; }
        public Money OffboardBuyoutPrice { get; init; }

        public IList<Label> Labels { get; init; }

        /// <summary>
        /// Is a personal or non-personal asset.
        /// </summary>
        public bool IsPersonal { get; set; }
        /// <summary>
        /// Is an asset Active state or Inactive state.
        /// </summary>
        public bool IsActiveState { get; set; }

        /// <summary>
        /// Which source was this fetched from.
        /// </summary>
        public string? Source { get; init; }
    }
}
