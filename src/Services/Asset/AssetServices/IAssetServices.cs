using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetServices.Models;

namespace AssetServices
{
    public interface IAssetServices
    {
        Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<IList<Asset>> GetAssetsForCustomerAsync(Guid customerId);
        Task<Asset> GetAssetForCustomerAsync(Guid customerId, Guid assetId);
        Task<Asset> AddAssetForCustomerAsync(Guid customerId, string serialNumber, Guid assetCategoryId,
            string brand, string model, LifecycleType lifecycleType, DateTime purchaseDate, Guid assetHolderId,
            bool isActive, Guid managedByDepartmentId);
    }
}