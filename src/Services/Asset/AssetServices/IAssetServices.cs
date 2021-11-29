using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.Models;
using Common.Enums;
using Common.Interfaces;
using Common.Models;

namespace AssetServices
{
    public interface IAssetServices
    {
        Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<PagedModel<Asset>> GetAssetsForCustomerAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken);
        Task<Asset> GetAssetForCustomerAsync(Guid customerId, Guid assetId);
        Task<Asset> AddAssetForCustomerAsync(Guid customerId, string alias, string serialNumber, int assetCategoryId,
            string brand, string productName, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId,
            IList<long> imei, string macAddress, Guid? managedByDepartmentId, AssetStatus status, string note, string tag, string description);
        Task<Asset> ChangeAssetLifecycleTypeForCustomerAsync(Guid customerId, Guid assetId, LifecycleType newLifecycleType);
        Task<IList<Asset>> UpdateMultipleAssetsStatus(Guid customerId, IList<Guid> assetGuidList, AssetStatus status);
        Task<Asset> UpdateAssetAsync(Guid customerId, Guid assetId, string alias, string serialNumber, string brand, string model, DateTime purchaseDate, string note, string tag, string description, IList<long> imei);
        Task<Asset> AssignAsset(Guid customerId, Guid assetId, Guid? userId);
        Task<IList<AssetCategory>> GetAssetCategoriesAsync(string language = "EN");
        Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId);
        IList<AssetLifecycle> GetLifecycles();
    }
}