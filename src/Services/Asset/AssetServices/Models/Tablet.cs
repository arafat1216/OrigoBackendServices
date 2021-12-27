using AssetServices.DomainEvents;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace AssetServices.Models
{
    public class Tablet : HardwareAsset
    {
        [JsonConstructor]
        public Tablet() { }

        public Tablet(Guid externalId, Guid customerId, Guid callerId, string alias, AssetCategory assetCategory, string serialNumber, string brand, string productName, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, IList<AssetImei> imei, string macAddress, AssetStatus status, string note, string assetTag, string description, Guid? managedByDepartmentId = null)
        {
            ExternalId = externalId;
            CustomerId = customerId;
            AssetCategory = assetCategory;
            SerialNumber = serialNumber;
            Brand = brand;
            ProductName = productName;
            LifecycleType = lifecycleType;
            PurchaseDate = purchaseDate;
            AssetHolderId = assetHolderId;
            Imeis = new ReadOnlyCollection<AssetImei>(imei ?? new List<AssetImei>());
            MacAddress = macAddress;
            Status = status;
            Note = note;
            AssetTag = assetTag;
            Description = description;
            ManagedByDepartmentId = managedByDepartmentId;
            Alias = alias;
            CreatedBy = callerId;
            UpdatedBy = callerId;

            // Store tablet created event with its serial number if applicable
            string text = "id: " + ExternalId.ToString();
            if (!string.IsNullOrEmpty(SerialNumber))
            {
                text = "serial number: " + SerialNumber;
            }
            AddDomainEvent(new AssetCreatedDomainEvent<Tablet>(this, callerId, text));
        }

        /// <summary>
        /// Sets the alias of the asset
        /// </summary>
        /// <param name="alias"></param>
        public override void SetAlias(string alias, Guid callerId)
        {
            var previousAlias = Alias;
            Alias = alias;
            AddDomainEvent(new SetAliasDomainEvent<Tablet>(this, callerId, previousAlias));
        }

        public override void SetLifeCycleType(LifecycleType newLifecycleType, Guid callerId)
        {
            var previousLifecycleType = LifecycleType;
            LifecycleType = newLifecycleType;
            AddDomainEvent(new SetLifeCycleTypeDomainEvent<Tablet>(this, callerId, previousLifecycleType));
        }

        public override void UpdateAssetStatus(AssetStatus status, Guid callerId)
        {
            var previousStatus = Status;
            Status = status;
            AddDomainEvent(new UpdateAssetStatusDomainEvent<Tablet>(this, callerId, previousStatus));
        }

        public override void UpdateBrand(string brand, Guid callerId)
        {
            var previousBrand = Brand;
            Brand = brand;
            AddDomainEvent(new BrandChangedDomainEvent<Tablet>(this, callerId, previousBrand));
        }

        public override void UpdateProductName(string model, Guid callerId)
        {
            var previousModel = ProductName;
            ProductName = model;
            AddDomainEvent(new ModelChangedDomainEvent<Tablet>(this, callerId, previousModel));
        }

        public override void ChangePurchaseDate(DateTime purchaseDate, Guid callerId)
        {
            var previousPurchaseDate = PurchaseDate;
            PurchaseDate = purchaseDate;
            AddDomainEvent(new PurchaseDateChangedDomainEvent<Tablet>(this, callerId, previousPurchaseDate));
        }

        public override void AssignAssetToUser(Guid? userId, Guid callerId)
        {
            var oldUserId = AssetHolderId;
            AssetHolderId = userId;
            AddDomainEvent(new AssignAssetToUserDomainEvent<Tablet>(this, callerId, oldUserId));
        }

        public override void UpdateNote(string note, Guid callerId)
        {
            var previousNote = Note;
            Note = note;
            AddDomainEvent(new NoteChangedDomainEvent<Tablet>(this, callerId, previousNote));
        }

        public override void UpdateDescription(string description, Guid callerId)
        {
            var previousDescription = Description;
            Description = description;
            AddDomainEvent(new DescriptionChangedDomainEvent<Tablet>(this, callerId, previousDescription));
        }

        public override void UpdateTag(string tag, Guid callerId)
        {
            var previousTag = AssetTag;
            AssetTag = tag;
            AddDomainEvent(new TagUpdatedDomainEvent<Tablet>(this, callerId, previousTag));
        }

        public override void ChangeSerialNumber(string serialNumber, Guid callerId)
        {
            var previousSerialNumber = SerialNumber;
            SerialNumber = serialNumber;
            AddDomainEvent(new SerialNumberChangedDomainEvent<Tablet>(this, callerId, previousSerialNumber));
        }
    }
}
