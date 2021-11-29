using Common.Models;
using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IAssetServices
    {
        Task<IList<object>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<IList<object>> GetAssetsForCustomerAsync(Guid customerId);
        Task<OrigoPagedAssets> SearchForAssetsForCustomerAsync(Guid customerId, string search = "", int page = 1, int limit = 50);
        Task<OrigoAsset> GetAssetForCustomerAsync(Guid customerId, Guid assetId);
        Task<OrigoAsset> AddAssetForCustomerAsync(Guid customerId, NewAsset newAsset);
        Task<IList<object>> UpdateStatusOnAssets(Guid customerId, IList<Guid> assetGuidList, int assetStatus);
        Task<IList<OrigoAssetLifecycle>> GetLifecycles();
        Task<OrigoAsset> ChangeLifecycleType(Guid customerId, Guid assetId, int newLifecycleType);
        Task<OrigoAsset> UpdateAssetAsync(Guid customerId, Guid assetId, OrigoUpdateAsset updateAsset);
        Task<OrigoAsset> AssignAsset(Guid customerId, Guid assetId, Guid? userId);
        Task<IList<OrigoAssetCategory>> GetAssetCategoriesAsync();
        Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId);
    }
}