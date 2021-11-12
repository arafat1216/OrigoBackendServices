using Common.Enums;
using System;
using System.Collections.Generic;

namespace AssetServices.Models
{
    public class MobilePhone : HardwareSuperType
    {
        protected MobilePhone() { }

        public MobilePhone(Guid externalId, Guid customerId, AssetCategory assetCategory, string serialNumber, string brand, string productName, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, IList<AssetImei> imei, string macAddress, AssetStatus status, string note, string assetTag, string description, Guid? managedByDepartmentId = null)
        {
            ExternalId = externalId;
            CustomerId = customerId;
            SerialNumber = serialNumber;
            Brand = brand;
            ProductName = productName;
            LifecycleType = lifecycleType;
            PurchaseDate = purchaseDate;
            AssetHolderId = assetHolderId;
            Imeis = imei;
            MacAddress = macAddress;
            Status = status;
            Note = note;
            AssetTag = assetTag;
            Description = description;
            AssetCategory = assetCategory;
            ManagedByDepartmentId = managedByDepartmentId;
        }
    }
}
