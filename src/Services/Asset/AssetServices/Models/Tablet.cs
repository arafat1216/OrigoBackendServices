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
            AddDomainEvent(new AssetCreatedDomainEvent<Tablet>(this, callerId));
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

        public override void UpdateBrand(string brand)
        {
            var previousBrand = Brand;
            Brand = brand;
            AddDomainEvent(new BrandChangedDomainEvent<Tablet>(this, previousBrand));
        }

        public override void UpdateProductName(string model)
        {
            var previousModel = ProductName;
            ProductName = model;
            AddDomainEvent(new ModelChangedDomainEvent<Tablet>(this, previousModel));
        }

        public override void ChangePurchaseDate(DateTime purchaseDate)
        {
            var previousPurchaseDate = PurchaseDate;
            PurchaseDate = purchaseDate;
            AddDomainEvent(new PurchaseDateChangedDomainEvent<Tablet>(this, previousPurchaseDate));
        }

        public override void AssignAssetToUser(Guid? userId)
        {
            var oldUserId = AssetHolderId;
            AssetHolderId = userId;
            AddDomainEvent(new AssignAssetToUserDomainEvent<Tablet>(this, oldUserId));
        }

        public override void UpdateNote(string note)
        {
            var previousNote = Note;
            Note = note;
            AddDomainEvent(new NoteChangedDomainEvent<Tablet>(this, previousNote));
        }

        public override void UpdateDescription(string description)
        {
            var previousDescription = Description;
            Description = description;
            AddDomainEvent(new DescriptionChangedDomainEvent<Tablet>(this, previousDescription));
        }

        public override void UpdateTag(string tag)
        {
            var previousTag = AssetTag;
            AssetTag = tag;
            AddDomainEvent(new TagUpdatedDomainEvent<Tablet>(this, previousTag));
        }
    }
}
