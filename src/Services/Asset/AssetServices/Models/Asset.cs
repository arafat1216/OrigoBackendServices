using System;
using System.ComponentModel.DataAnnotations;
using Common.Seedwork;
using AssetServices.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using AssetServices.DomainEvents;
using Common.Enums;
using MediatR;

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
            bool isActive, string imei, string macAddress, AssetStatus status, Guid? managedByDepartmentId = null)
        {
            AssetId = assetId;
            CustomerId = customerId;
            SerialNumber = serialNumber ?? string.Empty;
            AssetCategoryId = assetCategory.Id;
            AssetCategory = assetCategory;
            Brand = brand;
            Model = model;
            LifecycleType = lifecycleType;
            PurchaseDate = purchaseDate;
            AssetHolderId = assetHolderId;
            IsActive = isActive;
            ManagedByDepartmentId = managedByDepartmentId;
            Imei = imei ?? string.Empty;
            MacAddress = macAddress ?? string.Empty;
            ErrorMsgList = new List<string>();
            AssetPropertiesAreValid = ValidateAsset();
            Status = status;
            AddDomainEvent(new AssetCreatedDomainEvent(this));
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
        [StringLength(50, ErrorMessage = "Brand max length is 50")]
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
                if (!ValidateImei(e))
                {
                    throw new InvalidAssetDataException($"Invalid imei: {e}");
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
            foreach (string e in imei.Split(','))
            {
                if (!ValidateImei(e))
                {
                    throw new InvalidAssetDataException($"Invalid imei: {e}");
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
            var previousLifecycleType = LifecycleType;
            AddDomainEvent(new SetLifeCycleTypeDomainEvent(this, previousLifecycleType));
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
        public bool AssetPropertiesAreValid { get; protected set; }

        /// <summary>
        /// List of error messages set when ValidateAsset runs
        /// </summary>
        [NotMapped]
        public List<string> ErrorMsgList { get; protected set; }

        /// <summary>
        /// Validate an imei using Luhn's algorithm
        /// https://stackoverflow.com/questions/25229648/is-it-possible-to-validate-imei-number/25229800#25229800
        /// </summary>
        /// <param name="imei"></param>
        /// <returns></returns>
        protected bool ValidateImei(string imei)
        {
            //validate length
            if (string.IsNullOrEmpty(imei))
                return false;

            // is not a number
            if (!long.TryParse(imei, out _))
                return false;


            int sumDigits = 0;

            // temp value
            int d = 0;

            // 1. Get validation number (last number of imei)
            int validationDigit = int.Parse(imei[imei.Length - 1].ToString());


            // 2. Double values in every second value of remaining imei
            imei = imei.Substring(0, imei.Length - 1);
            for (int i = imei.Length - 1; i >= 0; i--)
            {
                string currentNumber = imei.Substring(i, 1);

                d = Convert.ToInt32(currentNumber);
                if (i % 2 != 0)
                {
                    // Double value and add it: 9 -> 18: value to add is 1+8, not 18.
                    d = d * 2;
                    string dString = Convert.ToString(d);
                    d = 0;
                    foreach (char e in dString)
                    {
                        d = d + int.Parse(e.ToString());
                    }
                }
                sumDigits += d;
            }

            // Round up to nearest multiplicative of 10: 52 -> 60
            var sumRounded = (int)Math.Ceiling(((double)sumDigits / 10)) * 10;

            var diffValue = sumRounded - sumDigits;

            return diffValue == validationDigit;
        }

        public void SetActiveStatus(bool isActive)
        {
            IsActive = isActive;
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
            var previousModel = Model;
            Model = model;
            AddDomainEvent(new ModelChangedDomainEvent(this, previousModel));
        }

        public void ChangeSerialNumber(string serialNumber)
        {
            var previousSerialNumber = SerialNumber;
            SerialNumber = serialNumber;
            AddDomainEvent(new SerialNumberChangedDomainEvent(this, previousSerialNumber));
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
            if (AssetCategory.Name == "Mobile phones")
            {
                foreach (string e in Imei.Split(","))
                {
                    if (!ValidateImei(e))
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