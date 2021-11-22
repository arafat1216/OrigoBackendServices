using Common.Enums;
using System;
using System.Collections.Generic;

namespace AssetServices.Models
{
    public class Subscription : SoftwareAsset
    {
        protected Subscription() { }

        public Subscription(Guid externalId, Guid customerId, string serialNumber, string brand, string productName, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, IList<AssetImei> imei, string macAddress, AssetStatus status, string note, string assetTag, string description, Guid? managedByDepartmentId = null)
        {
            ExternalId = externalId;
            CustomerId = customerId;
            SerialKey = serialNumber;
            Brand = brand;
            ProductName = productName;
            LifecycleType = lifecycleType;
            PurchaseDate = purchaseDate;
            AssetHolderId = assetHolderId;
            Status = status;
            Note = note;
            AssetTag = assetTag;
            Description = description;
            ManagedByDepartmentId = managedByDepartmentId;
        }
    }
}
