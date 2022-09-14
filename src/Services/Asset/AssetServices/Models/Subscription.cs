using Common.Enums;
using System;
using System.Collections.Generic;

namespace AssetServices.Models
{
    public class Subscription : SoftwareAsset
    {
        protected Subscription() { }

        public Subscription(Guid externalId, Guid customerId, string serialNumber, string brand, string productName, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, IList<AssetImei> imei, string macAddress, AssetLifecycleStatus lifecycleStatus, string note, string assetTag, string description, Guid? managedByDepartmentId = null)
        {
            ExternalId = externalId;
            SerialKey = serialNumber;
            Brand = brand;
            ProductName = productName[..Math.Min(productName.Length, 50)];
        }
    }
}
