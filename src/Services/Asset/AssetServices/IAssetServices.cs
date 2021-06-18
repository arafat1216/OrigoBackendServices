using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.Models;
using Common.Interfaces;

namespace AssetServices
{
    public interface IAssetServices
    {
        Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<PagedModel<Asset>> GetAssetsForCustomerAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken);
        Task<Asset> GetAssetForCustomerAsync(Guid customerId, Guid assetId);
        Task<Asset> AddAssetForCustomerAsync(Guid customerId, string serialNumber, Guid assetCategoryId,
            string brand, string model, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId,
            bool isActive, Guid? managedByDepartmentId);
        Task<Asset> ChangeAssetLifecycleTypeForCustomerAsync(Guid customerId, Guid assetId, LifecycleType newLifecycleType);
        Task<Asset> UpdateActiveStatus(Guid customerId, Guid assetId, bool isActive);
        Task<Asset> UpdateAssetAsync(Guid customerId, Guid assetId, string serialNumber, string brand, string model, DateTime purchaseDate);
    }
}