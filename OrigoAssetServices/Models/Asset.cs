using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoAssetServices.Models
{
    public class Asset
    {
        /// <summary>
        /// Id of the Asset
        /// </summary>
        [Key]
        public int Id { get; set; }
        [Required]
        public Guid AssetId { get; set; }

        [Required]
        [StringLength(100)]
        public string AssetName {get; set;}

        /// <summary>
        /// Id of the person owning or controlling the asset
        /// </summary>
        public Guid AssetHolderId { get; set; }
        public AssetHolder AssetHolder { get; set; }
        /// <summary>
        /// Id of the company owning or controlling the asset.
        /// </summary>
        public Guid CompanyId { get; set; }
        /// <summary>
        /// The id of department for the company which owns or controls the asset.
        /// </summary>
        public Guid DepartmentId { get; set; }
        public string Imei { get; set; }
        public string Vendor { get; set; }
        public string PhoneNumber { get; set; }
    }
}
