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

        Task<OrigoAsset> AddAssetForCustomerAsync(Guid customerId, NewAsset newAsset);
    }
}