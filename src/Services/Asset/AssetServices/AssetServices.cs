using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetServices.Models;
using Microsoft.Extensions.Logging;

namespace AssetServices
{
    public class AssetServices : IAssetServices
    {
        private readonly IAssetRepository _assetRepository;
        private readonly ILogger<AssetServices> _logger;

        public AssetServices(ILogger<AssetServices> logger, IAssetRepository assetRepository)
        {
            _logger = logger;
            _assetRepository = assetRepository;
        }

        public async Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId,  Guid userId)
        {
            return await _assetRepository.GetAssetsForUserAsync(customerId, userId);
        }

        public async Task<IList<Asset>> GetAssetsForCustomerAsync(Guid customerId)
        {
            return await _assetRepository.GetAssetsAsync(customerId);
        }

        public async Task<Asset> AddAssetForCustomerAsync(Guid customerId, Asset newAsset)
        {
            throw new NotImplementedException();
        }
    }
}