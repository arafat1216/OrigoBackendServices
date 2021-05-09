using System;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Seedwork;

namespace AssetServices.Models
{
    public class Asset : Entity, IAggregateRoot
    {

        // TODO: Set to protected as DDD best practice
        public Asset()
        {
        }


        /// <summary>
        /// External Id of the Asset
        /// </summary>
        public Guid AssetId { get; set; }

        [Required]
        [StringLength(100)]
        public string AssetName { get; set; }

        /// <summary>
        /// Id of the person owning or controlling the asset
        /// </summary>
        public Guid AssetHolderId { get; set; }

        /// <summary>
        /// The current owner of the asset
        /// </summary>
        /// <value></value>
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

        public void BuyoutDevice() { }
    }
}
