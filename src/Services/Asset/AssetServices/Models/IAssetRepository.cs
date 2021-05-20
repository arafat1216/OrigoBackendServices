using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetServices.Models
{
    public interface IAssetRepository
    {
        Task<Asset> AddAsync(Asset asset);
        Task<IList<Asset>> GetAssetsAsync(Guid customerId);
        Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<Asset> GetAssetAsync(Guid customerId, Guid assetId);
    }
}