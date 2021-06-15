using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IAssetServices
    {
        Task<IList<OrigoAsset>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<IList<OrigoAsset>> GetAssetsForCustomerAsync(Guid customerId);
        Task<OrigoPagedAssets> SearchForAssetsForCustomerAsync(Guid customerId, string search = "", int page = 1, int limit = 50);
        Task<OrigoAsset> GetAssetForCustomerAsync(Guid customerId, Guid assetId);
        Task<OrigoAsset> AddAssetForCustomerAsync(Guid customerId, NewAsset newAsset);
        Task<OrigoAsset> UpdateActiveStatus(Guid customerId, Guid assetId, bool isActive);
        Task<OrigoAsset> UpdateAssetAsync(Guid customerId, Guid assetId, OrigoUpdateAsset updateAsset);
    }
}