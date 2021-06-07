﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.Exceptions;
using AssetServices.Models;
using Common.Interfaces;
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

        public async Task<IList<Asset>> GetAssetsForCustomerAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken)
        {
            return await _assetRepository.GetAssetsAsync(customerId, search, page, limit, cancellationToken);
        }

        public async Task<Asset> GetAssetForCustomerAsync(Guid customerId, Guid assetId)
        {
            return await _assetRepository.GetAssetAsync(customerId, assetId);
        }
        
        public async Task<Asset> AddAssetForCustomerAsync(Guid customerId, string serialNumber, Guid assetCategoryId, string brand,
            string model, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, bool isActive,
            Guid? managedByDepartmentId)
        {
            var assetCategory = await _assetRepository.GetAssetCategoryAsync(assetCategoryId);
            if (assetCategory == null)
            {
                throw new AssetCategoryNotFoundException();
            }

            var newAsset = new Asset(Guid.NewGuid(), customerId, serialNumber, assetCategory.Id, brand, model,
                lifecycleType, purchaseDate, assetHolderId, isActive, managedByDepartmentId);
            return await _assetRepository.AddAsync(newAsset);
        }

        public async Task<Asset> UpdateActiveStatus(Guid customerId, Guid assetId, bool isActive)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);
            if (asset == null)
            {
                return null;
            }

            asset.SetActiveStatus(isActive);
            await _assetRepository.SaveChanges();
            return asset;
        }
    }
}