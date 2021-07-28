﻿using System;
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
        Task<Asset> AddAssetForCustomerAsync(Guid customerId, string serialNumber, Guid assetCategoryId,
            string brand, string model, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId,
            bool isActive, string imei, string macAddress, Guid? managedByDepartmentId, AssetStatus status);
        Task<Asset> ChangeAssetLifecycleTypeForCustomerAsync(Guid customerId, Guid assetId, LifecycleType newLifecycleType);
        Task<Asset> UpdateAssetStatus(Guid customerId, Guid assetId, AssetStatus status);
        Task<Asset> UpdateActiveStatus(Guid customerId, Guid assetId, bool isActive);
        Task<Asset> UpdateAssetAsync(Guid customerId, Guid assetId, string serialNumber, string brand, string model, DateTime purchaseDate);
        Task<Asset> AssignAsset(Guid customerId, Guid assetId, Guid? userId);
        Task<IList<AssetCategory>> GetAssetCategoriesAsync();
        Task<IList<AssetAuditLog>> GetAssetAuditLog();
        Task<IList<AssetLifecycle>> GetLifecycles();
    }
}