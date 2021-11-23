using Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AssetServices.Models
{
    public class Tablet : HardwareAsset
    {
        protected Tablet() { }

        public Tablet(Guid externalId, Guid customerId, string alias, AssetCategory assetCategory, string serialNumber, string brand, string productName, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, IList<AssetImei> imei, string macAddress, AssetStatus status, string note, string assetTag, string description, Guid? managedByDepartmentId = null)
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
            Imeis = new ReadOnlyCollection<AssetImei>( imei);
            MacAddress = macAddress;
            Status = status;
            Note = note;
            AssetTag = assetTag;
            Description = description;
            ManagedByDepartmentId = managedByDepartmentId;
            Alias = alias;
        }
    }
}
