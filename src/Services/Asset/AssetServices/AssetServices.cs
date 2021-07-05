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

        public async Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            return await _assetRepository.GetAssetsForUserAsync(customerId, userId);
        }

        public async Task<PagedModel<Asset>> GetAssetsForCustomerAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken)
        {
            try
            {
                return await _assetRepository.GetAssetsAsync(customerId, search, page, limit, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new ReadingDataException(exception);
            }
        }

        public async Task<Asset> GetAssetForCustomerAsync(Guid customerId, Guid assetId)
        {
            return await _assetRepository.GetAssetAsync(customerId, assetId);
        }

        public async Task<Asset> AddAssetForCustomerAsync(Guid customerId, string serialNumber, Guid assetCategoryId, string brand,
            string model, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, bool isActive, string imei, string macAddress,
            Guid? managedByDepartmentId)
        {
            var assetCategory = await _assetRepository.GetAssetCategoryAsync(assetCategoryId);
            if (assetCategory == null)
            {
                throw new AssetCategoryNotFoundException();
            }

            var newAsset = new Asset(Guid.NewGuid(), customerId, serialNumber, assetCategory, brand, model,
                lifecycleType, purchaseDate, assetHolderId, isActive,imei, macAddress, managedByDepartmentId);

            if (!newAsset.AssetPropertiesAreValid)
            {
                string exceptionMsg = "";
                foreach (string errorMsg in newAsset.ErrorMsgList)
                {
                    if (errorMsg.Contains("Imei"))
                    {
                        exceptionMsg += string.Format("Asset {0}", errorMsg) + " is invalid.\n";
                    }
                    else
                    {
                        exceptionMsg += string.Format("Minimum asset data requirements not set: {0}", errorMsg) + ".\n";
                    }
                }
                throw new InvalidAssetDataException(exceptionMsg);
            }
            return await _assetRepository.AddAsync(newAsset);
        }

        public async Task<IList<AssetLifecycle>> GetLifecycles()
        {
            Array arr = Enum.GetValues(typeof(LifecycleType));
            List<AssetLifecycle> assetLifecycles = new List<AssetLifecycle>();

            foreach (LifecycleType e in arr)
            {
                assetLifecycles.Add(new AssetLifecycle()
                {
                    Name = Enum.GetName(typeof(LifecycleType), e),
                    EnumValue = (int)e
                });
            }

            return assetLifecycles;
        }

        public async Task<Asset> ChangeAssetLifecycleTypeForCustomerAsync(Guid customerId, Guid assetId, LifecycleType newLifecycleType)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);
            if (asset == null)
            {
                return null;
            }

            asset.SetLifeCycleType(newLifecycleType);
            await _assetRepository.SaveChanges();
            return asset;
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

        public async Task<Asset> UpdateAssetAsync(Guid customerId, Guid assetId, string serialNumber, string brand, string model, DateTime purchaseDate)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);

            if (asset == null)
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(serialNumber))
            {
                asset.ChangeSerialNumber(serialNumber);
            }
            if (!string.IsNullOrWhiteSpace(brand))
            {
                asset.UpdateBrand(brand);
            }
            if (!string.IsNullOrWhiteSpace(model))
            {
                asset.UpdateModel(model);
            }
            if (purchaseDate != default)
            {
                asset.ChangePurchaseDate(purchaseDate);
            }

            await _assetRepository.SaveChanges();
            return asset;
        }

        public async Task<Asset> AssignAsset(Guid customerId, Guid assetId, Guid? userId)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);
            if (asset == null)
            {
                return null;
            }
            asset.AssignAssetToUser(userId);
            await _assetRepository.SaveChanges();
            return asset;
        }

        public async Task<IList<AssetCategory>> GetAssetCategoriesAsync()
        {
            return await _assetRepository.GetAssetCategoriesAsync();
        }
    }
}