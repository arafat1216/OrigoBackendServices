using System;
using System.ComponentModel.DataAnnotations;
using Common.Seedwork;
using AssetServices.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;
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
            bool isActive, Guid? managedByDepartmentId = null)
        {
            AssetId = assetId;
            CustomerId = customerId;
            SerialNumber = serialNumber;
            AssetCategoryId = assetCategory.Id;
            AssetCategory = assetCategory;
            Brand = brand;
            Model = model;
            LifecycleType = lifecycleType;
            PurchaseDate = purchaseDate;
            AssetHolderId = assetHolderId;
            IsActive = isActive;
            ManagedByDepartmentId = managedByDepartmentId;
            AssetPropertiesAreValid = ValidateAsset(assetCategory, customerId, brand, model, purchaseDate);
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
        [StringLength(25, ErrorMessage ="Brand max length is 25")]
        public string Brand { get; protected set; }

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Model max length is 50")]
        public string Model { get; protected set; }

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
        /// Defines wether the asset made has the necessary properties set, as defined by ValidateAsset.
        /// </summary>
        [NotMapped]
        public bool AssetPropertiesAreValid { get; protected set; }

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
        //  Additional restrictions exist on specific assetCategories
        /// </summary>
        /// <param name="assetCategory">The type of asset, f.ex "Mobile Phones"</param>
        /// <param name="customerId">The customer the asset is linked to</param>
        /// <param name="brand">The brand of the asset (apple, samsung, etc)</param>
        /// <param name="model">The specific model of the asset, f.ex "Galaxy P9 Lite"</param>
        /// <param name="purchaseDate">Date of purchase</param>
        /// <returns>Boolean value, true if asset has valid properties, false if not</returns>
        protected bool ValidateAsset(AssetCategory assetCategory, Guid customerId, string brand, string model, DateTime purchaseDate)
        {
            // General (all types)
            if (customerId == Guid.Empty || string.IsNullOrEmpty(brand) || string.IsNullOrEmpty(model) || purchaseDate == DateTime.MinValue)
            {
                return false;//throw new InvalidAssetCategoryDataException("One or more asset values are invalid for all asset categories.");
            }

            // Mobile Phones
            if (assetCategory.Name == "Mobile Phones")
            {
                //todo: Implement specifics for Mobile Phones
            }

            return true;
        }
    }
}