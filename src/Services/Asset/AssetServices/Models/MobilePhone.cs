using AssetServices.DomainEvents;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AssetServices.Models
{
    public class MobilePhone : HardwareAsset
    {
        
        public MobilePhone() { }
        
        public MobilePhone(Guid externalId, Guid customerId, Guid callerId, string alias, AssetCategory assetCategory, string serialNumber, string brand, string productName,
            LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, IList<AssetImei> imei, string macAddress, AssetStatus status, string note, string assetTag,
            string description, Guid? managedByDepartmentId = null)
        {
            ExternalId = externalId;
            CustomerId = customerId;
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
            AssetCategory = assetCategory;
            ManagedByDepartmentId = managedByDepartmentId;
            Alias = alias;
            AddDomainEvent(new AssetCreatedDomainEvent<MobilePhone>(this, callerId));
        }

        public override void SetLifeCycleType(LifecycleType newLifecycleType)
        {
            var previousLifecycleType = LifecycleType; 
            LifecycleType = newLifecycleType;
            AddDomainEvent(new SetLifeCycleTypeDomainEvent<MobilePhone>(this, previousLifecycleType));
        }

        public override void UpdateAssetStatus(AssetStatus status)
        {
            var previousStatus = Status;
            Status = status;
            AddDomainEvent(new UpdateAssetStatusDomainEvent<MobilePhone>(this, previousStatus));
        }

        public override void UpdateBrand(string brand)
        {
            var previousBrand = Brand;
            Brand = brand;
            AddDomainEvent(new BrandChangedDomainEvent<MobilePhone>(this, previousBrand));
        }

        public override void UpdateProductName(string model)
        {
            var previousModel = ProductName;
            ProductName = model;
            AddDomainEvent(new ModelChangedDomainEvent<MobilePhone>(this, previousModel));
        }

        public override void ChangePurchaseDate(DateTime purchaseDate)
        {
            var previousPurchaseDate = PurchaseDate;
            PurchaseDate = purchaseDate;
            AddDomainEvent(new PurchaseDateChangedDomainEvent<MobilePhone>(this, previousPurchaseDate));
        }

        public override void AssignAssetToUser(Guid? userId)
        {
            var oldUserId = AssetHolderId;
            AssetHolderId = userId;
            AddDomainEvent(new AssignAssetToUserDomainEvent<MobilePhone>(this, oldUserId));
        }

        public override void UpdateNote(string note)
        {
            var previousNote = Note;
            Note = note;
            AddDomainEvent(new NoteChangedDomainEvent<MobilePhone>(this, previousNote));
        }

        public override void UpdateDescription(string description)
        {
            var previousDescription = Description;
            Description = description;
            AddDomainEvent(new DescriptionChangedDomainEvent<MobilePhone>(this, previousDescription));
        }

        public override void UpdateTag(string tag)
        {
            var previousTag = AssetTag;
            AssetTag = tag;
            AddDomainEvent(new TagUpdatedDomainEvent<MobilePhone>(this, previousTag));
        }
    }
}
