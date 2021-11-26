using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public class OrigoMobilePhone : HardwareSuperType
    {
        protected OrigoMobilePhone() { }


        public OrigoMobilePhone(AssetDTO asset)
        {
            Id = asset.Id;
            OrganizationId = asset.OrganizationId;
            Note = asset.Note;
            SerialNumber = asset.SerialNumber;
            AssetCategoryId = asset.AssetCategoryId;
            AssetCategoryName = asset.AssetCategoryName;
            Brand = asset.Brand;
            ProductName = asset.ProductName;
            LifecycleType = asset.LifecycleType;
            LifeCycleName = asset.LifecycleName;
            Imei = asset.Imei;
            MacAddress = asset.MacAddress;
            PurchaseDate = asset.PurchaseDate;
            CreatedDate = asset.CreatedDate;
            ManagedByDepartmentId = asset.ManagedByDepartmentId;
            AssetHolderId = asset.AssetHolderId;
            AssetStatus = asset.AssetStatus;
            AssetStatusName = asset.AssetStatusName;
            AssetTag = asset.AssetTag;
            Description = asset.Description;
            Alias = asset.Alias;
        }
    }
}
