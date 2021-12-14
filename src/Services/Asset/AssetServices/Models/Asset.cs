﻿using AssetServices.DomainEvents;
using Common.Enums;
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
        // Set to protected as DDD best practice
        // ReSharper disable once UnusedMember.Global
        public Asset()
        {
        }

        protected Asset(Guid assetId, Guid customerId, Guid callerId, string alias, AssetCategory assetCategory, string brand, string productName,
            LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId,
            AssetStatus status, string note, string assetTag, string description, Guid? managedByDepartmentId = null)
        {
            ExternalId = assetId;
            CustomerId = customerId;
            CreatedBy = callerId;
            Alias = alias;
            Brand = brand;
            ProductName = productName;
            LifecycleType = lifecycleType;
            PurchaseDate = purchaseDate;
            AssetHolderId = assetHolderId;
            ManagedByDepartmentId = managedByDepartmentId;
            ErrorMsgList = new List<string>();
            Status = status;
            Note = note;
            AssetTag = assetTag;
            Description = description;
            AddDomainEvent(new AssetCreatedDomainEvent<Asset>(this, callerId));
        }

        /// <summary>
        /// External Id of the Asset
        /// </summary>
        [JsonInclude]
        public Guid ExternalId { get;  set; }

        /// <summary>
        /// Asset is linked to this customer 
        /// </summary>
        [Required]
        [JsonInclude]
        public Guid CustomerId { get; protected set; }

        [JsonInclude]
        public AssetCategory AssetCategory { get; protected set; }

        /// <summary>
        /// Alias for the asset.
        /// </summary>
        [JsonInclude]
        public string Alias { get; protected set; }

        /// <summary>
        /// A note containing additional information or comments for the asset.
        /// </summary>
        [JsonInclude]
        public string Note { get; protected set; }

        [JsonInclude]
        public string Description { get; protected set; }

        [JsonInclude]
        public string AssetTag { get; protected set; }

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



        /// <summary>
        /// The type of lifecycle for this asset.
        /// </summary>
        [JsonInclude]
        public LifecycleType LifecycleType { get; protected set; }

        [Required]
        [JsonInclude]
        public DateTime PurchaseDate { get; protected set; }

        /// <summary>
        /// The department or cost center this asset is assigned to.
        /// </summary>
        [JsonInclude]
        public Guid? ManagedByDepartmentId { get; protected set; }

        /// <summary>
        /// The employee holding the asset.
        /// </summary>
        [JsonInclude]
        public Guid? AssetHolderId { get; protected set; }

        /// <summary>
        /// The status of the asset.
        /// <see cref="AssetStatus">AssetStatus</see>
        /// </summary>
        [JsonInclude]
        public AssetStatus Status { get; protected set; }

        // The mapping of labels assigned to this asset
        [JsonIgnore]
        public virtual ICollection<AssetLabel> AssetLabels { get; set; }

        /// <summary>
        /// Sets the alias of the asset
        /// </summary>
        /// <param name="alias"></param>
        public void SetAlias(string alias)
        {
            Alias = alias;
        }

        public virtual void SetLifeCycleType(LifecycleType newLifecycleType)
        {
            var previousLifecycleType = LifecycleType;
            LifecycleType = newLifecycleType;
            AddDomainEvent(new SetLifeCycleTypeDomainEvent<Asset>(this, previousLifecycleType));
        }

        public virtual void UpdateAssetStatus(AssetStatus status)
        {
            var previousStatus = Status;
            Status = status;
            AddDomainEvent(new UpdateAssetStatusDomainEvent<Asset>(this, previousStatus));
        }

        public virtual void UpdateBrand(string brand)
        {
            var previousBrand = Brand;
            Brand = brand;
            AddDomainEvent(new BrandChangedDomainEvent<Asset>(this, previousBrand));
        }

        public virtual void UpdateProductName(string model)
        {
            var previousModel = ProductName;
            ProductName = model;
            AddDomainEvent(new ModelChangedDomainEvent<Asset>(this, previousModel));
        }

        public virtual void ChangePurchaseDate(DateTime purchaseDate)
        {
            var previousPurchaseDate = PurchaseDate;
            PurchaseDate = purchaseDate;
            AddDomainEvent(new PurchaseDateChangedDomainEvent<Asset>(this, previousPurchaseDate));
        }

        public virtual void AssignAssetToUser(Guid? userId)
        {
            var oldUserId = AssetHolderId;
            AssetHolderId = userId;
            AddDomainEvent(new AssignAssetToUserDomainEvent<Asset>(this, oldUserId));
        }

        public virtual void UpdateNote(string note)
        {
            var previousNote = Note;
            Note = note;
            AddDomainEvent(new NoteChangedDomainEvent<Asset>(this, previousNote));
        }

        public virtual void UpdateDescription(string description)
        {
            var previousDescription = Description;
            Description = description;
            AddDomainEvent(new DescriptionChangedDomainEvent<Asset>(this, previousDescription));
        }

        public virtual void UpdateTag(string tag)
        {
            var previousTag = AssetTag;
            AssetTag = tag;
            AddDomainEvent(new TagUpdatedDomainEvent<Asset>(this, previousTag));
        }

        /// <summary>
        /// Defines whether the asset made has the necessary properties set, as defined by ValidateAsset.
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool AssetPropertiesAreValid { get { return ValidateAsset(); } }

        /// <summary>
        /// List of error messages set when ValidateAsset runs
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<string> ErrorMsgList { get; protected set; }

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
        protected abstract bool ValidateAsset();
    }
}