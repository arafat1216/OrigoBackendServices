using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.Models
{
    public abstract class Asset : Entity, IAggregateRoot
    {
        /// <summary>
        /// External Id of the Asset
        /// </summary>
        [JsonInclude]
        public Guid ExternalId { get;  set; }

        /// <summary>
        /// The asset brand (e.g. Samsung)
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Brand max length is 50")]
        [JsonInclude]
        public string Brand { get; protected set; }

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Model max length is 50")]
        [JsonInclude]
        public string ProductName { get; protected set; }

        public virtual void UpdateBrand(string brand, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
        }

        public virtual void UpdateProductName(string model, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// List of error messages set when ValidateAsset runs
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<string> ErrorMsgList { get; protected set; } = new();

        /// <summary>
        /// Validate the properties of the asset.
        ///  All assets need the following properties set (default values count as null):
        ///   - customerId
        ///   - brand
        ///   - model
        ///   - purchaseDate
        ///
        ///  Additional restrictions based on asset category:
        /// Mobile phones:
        ///  - Imei must be valid
        /// </summary>
        /// <returns>Boolean value, true if asset has valid properties, false if not</returns>
        public abstract bool ValidateAsset();
    }
}