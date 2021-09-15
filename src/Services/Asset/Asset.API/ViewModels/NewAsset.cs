using Common.Enums;
using System;

namespace Asset.API.ViewModels
{
    public class NewAsset
    {
        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// The category this asset belongs to.
        /// </summary>
        public Guid AssetCategoryId { get; set; }

        /// <summary>
        /// A note containing additional information or comments for the asset.
        /// </summary>
        public string Note { get; set; }

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
        public LifecycleType LifecycleType { get; set; }


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
        /// The imei of the asset. Applicable to assets with category Mobile Phone
        /// </summary>
        public string Imei { get; set; }

        /// <summary>
        /// The mac address of the asset.
        /// </summary>
        public string MacAddress { get; set; }

        public bool IsActive { get; set; }

        public int AssetStatus { get; set; }
    }
}