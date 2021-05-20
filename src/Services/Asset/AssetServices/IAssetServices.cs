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
        Task<Asset> AddAssetForCustomerAsync(Asset newAsset);
    }
}