﻿using System;
using System.ComponentModel.DataAnnotations;
using Common.Seedwork;
using AssetServices.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using AssetServices.Infrastructure;
using System.Threading.Tasks;
using Common.Enums;
using AssetServices.Attributes;
using AssetServices.Utility;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.Models
{
    public class Asset : Entity, IAggregateRoot
    {
        // TODO: Set to protected as DDD best practice
        protected Asset()
        {
        }

        public Asset(Guid assetId, Guid customerId, string serialNumber, AssetCategory assetCategory, string brand, string model,
            LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId,
            bool isActive, string imei, string macAddress, Guid? managedByDepartmentId = null)
        {
            AssetId = assetId;
            CustomerId = customerId;
            SerialNumber = (serialNumber != null) ? serialNumber : string.Empty;
            AssetCategoryId = assetCategory.Id;
            AssetCategory = assetCategory;
            Brand = brand;
            Model = model;
            LifecycleType = lifecycleType;
            PurchaseDate = purchaseDate;
            AssetHolderId = assetHolderId;
            IsActive = isActive;
            ManagedByDepartmentId = managedByDepartmentId;
            Imei = (imei != null) ? imei : string.Empty;
            MacAddress = (macAddress != null) ? macAddress : string.Empty;
            ErrorMsgList = new List<string>();
            AssetPropertiesAreValid = ValidateAsset();
        }

        /// <summary>
        /// External Id of the Asset
        /// </summary>
        public Guid AssetId { get; protected set; }

        /// <summary>
        /// Asset is linked to this customer 
        /// </summary>
        [Required]
        public Guid CustomerId { get; protected set; }

        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        [Required]
        public string SerialNumber { get; protected set; }

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
        [StringLength(50, ErrorMessage ="Brand max length is 50")]
        public string Brand { get; protected set; }

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Model max length is 50")]
        public string Model { get; protected set; }

        /// <summary>
        /// Set the imei for the asset.
        /// Erases existing imeis on asset.
        /// 
        /// Imei is a comma separated string, and can hold multiple imei values
        /// </summary>
        /// <param name="imei"></param>
        public void SetImei(string imei)
        {
            foreach (string e in imei.Split(','))
            {
                if (!AssetValidatorUtility.ValidateImei(e))
                {
                    throw new InvalidAssetDataException(string.Format("Invalid imei: {0}", e));
                }
            }
            Imei = imei;
        }

        /// <summary>
        /// Appends an Imei for the device.
        /// Imei is a comma separated string, and can hold multiple imei values
        /// </summary>
        /// <param name="imei"></param>
        public void AddImei(string imei)
        {
            foreach(string e in imei.Split(','))
            {
                if (!AssetValidatorUtility.ValidateImei(e))
                {
                    throw new InvalidAssetDataException(string.Format("Invalid imei: {0}", e));
                }
            }

            if (Imei == "")
            {
                Imei += imei;
            }
            else
            {
                Imei += "," + imei;
            }
        }

        /// <summary>
        /// Sets the macaddress of the asset
        /// </summary>
        /// <param name="macAddress"></param>
        public void SetMacAddress(string macAddress)
        {
            MacAddress = macAddress;
        }

        /// <summary>
        /// The type of lifecycle for this asset.
        /// </summary>
        public LifecycleType LifecycleType { get; protected set; }

        public void SetLifeCycleType(LifecycleType newLifecycleType)
        {
            LifecycleType = newLifecycleType;
        }

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
        /// Is this asset activated
        /// </summary>
        public bool IsActive { get; protected set; }

        /// <summary>
        /// A comma seperated string holding 0->n imei numbers for this entity.
        /// 
        /// A mobile phone must have atleast 1 imei.
        /// </summary>
        [ImeiValidation(ErrorMessage = "Invalid imei value.")]
        public string Imei { get; protected set; }

        /// <summary>
        /// The mac-address of the asset
        /// </summary>
        public string MacAddress { get; protected set; }

        /// <summary>
        /// Defines wether the asset made has the necessary properties set, as defined by ValidateAsset.
        /// </summary>
        [NotMapped]
        public bool AssetPropertiesAreValid { get; protected set; }

        /// <summary>
        /// List of error messages set when ValidateAsset runs
        /// </summary>
        [NotMapped]
        public List<string> ErrorMsgList { get; protected set; }

        public void SetActiveStatus(bool isActive)
        {
            IsActive = isActive;
        }
        public void UpdateBrand(string brand)
        {
            Brand = brand;
        }

        public void UpdateModel(string model)
        {
            Model = model;
        }

        public void ChangeSerialNumber(string serialNumber)
        {
            SerialNumber = serialNumber;
        }

        public void ChangePurchaseDate(DateTime purchaseDate)
        {
            PurchaseDate = purchaseDate;
        }

        public void AssignAssetToUser(Guid? userId)
        {
            AssetHolderId = userId;
        }

        /// <summary>
        /// Validate the properties of the asset.
        //  All assets need the following properties set (default values count as null):
        //   - customerId
        //   - brand
        //   - model
        //   - purchaseDate
        //
        //  Additional restrictions based on asset category:
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
                validAsset =  false;
            }

            if (string.IsNullOrEmpty(Brand))
            {
                ErrorMsgList.Add("Brand - Cannot be null or empty");
                validAsset = false;
            }

            if (string.IsNullOrEmpty(Model))
            {
                ErrorMsgList.Add("Model - Cannot be null or empty");
                validAsset = false;
            }

            if (PurchaseDate == DateTime.MinValue)
            {
                ErrorMsgList.Add("PurchaseDate - Cannot be DateTime.MinValue");
                validAsset = false;
            }

            // Mobile Phones
            if (AssetCategory.Name== "Mobile phones")
            {
                foreach (string e in Imei.Split(","))
                {
                    if (!AssetValidatorUtility.ValidateImei(e))
                    {
                        ErrorMsgList.Add("Imei : " + e);
                        validAsset = false;
                    }
                }
            }
            return validAsset;
        }
    }
}