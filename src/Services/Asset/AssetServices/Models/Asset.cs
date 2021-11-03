﻿using System;
using System.ComponentModel.DataAnnotations;
using Common.Seedwork;
using AssetServices.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using AssetServices.DomainEvents;
using Common.Enums;
using AssetServices.Utility;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.Models
{
    public class Asset : Entity, IAggregateRoot
    {
        // Set to protected as DDD best practice
        // ReSharper disable once UnusedMember.Global
        protected Asset()
        {
        }

        public Asset(Guid assetId, Guid customerId, string serialNumber, AssetCategory assetCategory, string brand, string model,
            LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId,
            string imei, string macAddress, AssetStatus status, string note, Guid? managedByDepartmentId = null)
        {
            ExternalId = assetId;
            CustomerId = customerId;
            AssetCategoryId = assetCategory.Id;
            AssetCategory = assetCategory;
            Brand = brand;
            ProductName = model;
            LifecycleType = lifecycleType;
            PurchaseDate = purchaseDate;
            AssetHolderId = assetHolderId;
            ManagedByDepartmentId = managedByDepartmentId;
            Imei = imei ?? string.Empty;
            MacAddress = macAddress ?? string.Empty;
            ErrorMsgList = new List<string>();
            AssetPropertiesAreValid = ValidateAsset();
            Status = status;
            Note = note;
            AddDomainEvent(new AssetCreatedDomainEvent(this));
        }

        /// <summary>
        /// External Id of the Asset
        /// </summary>
        public Guid ExternalId { get; protected set; }

        /// <summary>
        /// Asset is linked to this customer 
        /// </summary>
        [Required]
        public Guid CustomerId { get; protected set; }

        /// <summary>
        /// A note containing additional information or comments for the asset.
        /// </summary>
        public string Note { get; protected set; }

        /// <summary>
        /// The category this asset belongs to.
        /// </summary>
        [Required]
        public int AssetCategoryId { get; protected set; }

        /// <summary>
        /// The category this asset belongs to.
        /// </summary>
        public AssetCategory AssetCategory { get; protected set; }

        /// <summary>
        /// The asset brand (e.g. Samsung)
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Brand max length is 50")]
        public string Brand { get; protected set; }

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Model max length is 50")]
        public string ProductName { get; protected set; }

        /// <summary>
        /// The type of lifecycle for this asset.
        /// </summary>
        public LifecycleType LifecycleType { get; protected set; }

        [Required]
        public DateTime PurchaseDate { get; protected set; }

        /// <summary>
        /// The department or cost center this asset is assigned to.
        /// </summary>
        public Guid? ManagedByDepartmentId { get; protected set; }

        /// <summary>
        /// The employee holding the asset.
        /// </summary>
        public Guid? AssetHolderId { get; protected set; }

        /// <summary>
        /// The status of the asset.
        /// <see cref="AssetStatus">AssetStatus</see>
        /// </summary>
        public AssetStatus Status { get; protected set; }

        /// <summary>
        /// A comma separated string holding 0->n imei numbers for this entity.
        /// 
        /// A mobile phone must have at least 1 imei.
        /// </summary>
        public string Imei { get; protected set; }

        /// <summary>
        /// The mac-address of the asset
        /// </summary>
        public string MacAddress { get; protected set; }

        /// <summary>
        /// Defines whether the asset made has the necessary properties set, as defined by ValidateAsset.
        /// </summary>
        [NotMapped]
        public bool AssetPropertiesAreValid { get { return ValidateAsset(); } }

        /// <summary>
        /// List of error messages set when ValidateAsset runs
        /// </summary>
        [NotMapped]
        public List<string> ErrorMsgList { get; protected set; }

        public void SetLifeCycleType(LifecycleType newLifecycleType)
        {
            var previousLifecycleType = LifecycleType;
            AddDomainEvent(new SetLifeCycleTypeDomainEvent(this, previousLifecycleType));
            LifecycleType = newLifecycleType;
        }

        /// <summary>
        /// Sets the macaddress of the asset
        /// </summary>
        /// <param name="macAddress"></param>
        public void SetMacAddress(string macAddress)
        {
            MacAddress = macAddress;
        }

        public void UpdateAssetStatus(AssetStatus status)
        {
            var previousStatus = Status;
            Status = status;
            AddDomainEvent(new UpdateAssetStatusDomainEvent(this, previousStatus));
        }

        public void UpdateBrand(string brand)
        {
            var previousBrand = Brand;
            Brand = brand;
            AddDomainEvent(new BrandChangedDomainEvent(this, previousBrand));
        }

        public void UpdateModel(string model)
        {
            var previousModel = ProductName;
            ProductName = model;
            AddDomainEvent(new ModelChangedDomainEvent(this, previousModel));
        }

        public void ChangePurchaseDate(DateTime purchaseDate)
        {
            var previousPurchaseDate = PurchaseDate;
            PurchaseDate = purchaseDate;
            AddDomainEvent(new PurchaseDateChangedDomainEvent(this, previousPurchaseDate));
        }

        public void AssignAssetToUser(Guid? userId)
        {
            var oldUserId = AssetHolderId;
            AssetHolderId = userId;
            AddDomainEvent(new AssignAssetToUserDomainEvent(this, oldUserId));
        }

        public void UpdateNote(string note)
        {
            var previousNote = Note;
            Note = note;
            AddDomainEvent(new NoteChangedDomainEvent(this, previousNote));
        }

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
        protected bool ValidateAsset()
        {
            bool validAsset = true;
            // General (all types)
            if (CustomerId == Guid.Empty)
            {
                ErrorMsgList.Add("CustomerId - Cannot be Guid.Empty");
                validAsset = false;
            }

            if (string.IsNullOrEmpty(Brand))
            {
                ErrorMsgList.Add("Brand - Cannot be null or empty");
                validAsset = false;
            }

            if (string.IsNullOrEmpty(ProductName))
            {
                ErrorMsgList.Add("Model - Cannot be null or empty");
                validAsset = false;
            }

            if (PurchaseDate == DateTime.MinValue)
            {
                ErrorMsgList.Add("PurchaseDate - Cannot be DateTime.MinValue");
                validAsset = false;
            }

            return validAsset;
        }
    }
}