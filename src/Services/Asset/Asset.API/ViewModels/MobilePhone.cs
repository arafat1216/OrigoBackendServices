using System.Collections.Generic;
using System.Linq;

namespace Asset.API.ViewModels
{
    public record MobilePhone : HardwareType
    {
        protected MobilePhone() { }

        public MobilePhone(AssetServices.Models.MobilePhone asset)
        {
            AssetId = asset.ExternalId;
            OrganizationId = asset.CustomerId;
            SerialNumber = asset.SerialNumber;
            Brand = asset.Brand;
            ProductName = asset.ProductName;
            LifecycleType = asset.LifecycleType;
            PurchaseDate = asset.PurchaseDate;
            AssetHolderId = asset.AssetHolderId;
            Imei = asset.Imeis != null ? asset.Imeis.Select(i => i.Imei).ToList() : new List<long>();
            MacAddress = asset.MacAddress;
            Note = asset.Note;
            AssetTag = asset.AssetTag;
            Description = asset.Description;
            AssetStatus = asset.Status;
            ManagedByDepartmentId = asset.ManagedByDepartmentId;
            AssetCategoryId = asset.AssetCategory != null ? asset.AssetCategory.Id : 0;
            AssetCategoryName = asset.AssetCategory?.Translations?.FirstOrDefault(a => a.Language == "EN")?.Name;
            Alias = asset.Alias;
        }
    }
}
