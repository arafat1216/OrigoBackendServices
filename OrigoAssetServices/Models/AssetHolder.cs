using OrigoAssetServices.Interfaces;
using System;

namespace OrigoAssetServices.Models
{
    public class AssetHolder : IAggregateRoot
    {
        public int Id { get; set; }
        /// <summary>
        /// Id of the person owning or controlling the asset
        /// </summary>
        public Guid AssetHolderId { get; set; }
        /// <summary>
        /// Id of the company owning or controlling the asset.
        /// </summary>
        public Guid CompanyId { get; set; }
        /// <summary>
        /// The id of department for the company which owns or controls the asset.
        /// </summary>
        public Guid DepartmentId { get; set; }
    }
}
