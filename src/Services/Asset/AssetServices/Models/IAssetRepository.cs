using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

namespace AssetServices.Models
{
    public interface IAssetRepository
    {
        Task<Asset> AddAsync(Asset asset);
        Task<PagedModel<Asset>> GetAssetsAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken);
        Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<Asset> GetAssetAsync(Guid customerId, Guid assetId);
        Task<AssetCategory> GetAssetCategoryAsync(int assetAssetCategoryId);
        Task<IList<AssetCategory>> GetAssetCategoriesAsync(string language = "EN");
        Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default);
        Task<IList<FunctionalEventLogEntry>> GetAuditLog(Guid assetId);
    }
}