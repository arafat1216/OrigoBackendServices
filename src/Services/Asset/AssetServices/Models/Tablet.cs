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
            CreatedBy = callerId;
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
            AddDomainEvent(new AssetCreatedDomainEvent<Tablet>(this, callerId));
        }

        /// <summary>
        /// Sets the alias of the asset
        /// </summary>
        /// <param name="alias"></param>
        public override void SetAlias(string alias, Guid callerId)
        {
            base.SetAlias(alias, callerId); 
        }

        public override void SetLifeCycleType(LifecycleType newLifecycleType, Guid callerId)
        {
            base.SetLifeCycleType(newLifecycleType, callerId);
        }

        public override void UpdateAssetStatus(AssetStatus status, Guid callerId)
        {
            base.UpdateAssetStatus(status, callerId);
        }

        public override void UpdateBrand(string brand, Guid callerId)
        {
            base.UpdateBrand(brand, callerId);
        }

        public override void UpdateProductName(string model, Guid callerId)
        {
            base.UpdateProductName(model, callerId);    
        }

        public override void ChangePurchaseDate(DateTime purchaseDate, Guid callerId)
        {
            base.ChangePurchaseDate(purchaseDate, callerId);
        }

        public override void AssignAssetToUser(Guid? userId, Guid callerId)
        {
            base.AssignAssetToUser(userId, callerId);
        }

        public override void UpdateNote(string note, Guid callerId)
        {
            base.UpdateNote(note, callerId);
        }

        public override void UpdateDescription(string description, Guid callerId)
        {
            base.UpdateDescription(description, callerId);
        }

        public override void UpdateTag(string tag, Guid callerId)
        {
            base.UpdateTag(tag, callerId);
        }

        public override void ChangeSerialNumber(string serialNumber, Guid callerId)
        {
            base.ChangeSerialNumber(serialNumber, callerId);
        }
    }
}
