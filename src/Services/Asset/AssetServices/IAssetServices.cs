﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.Models;
using AssetServices.ServiceModel;
using Common.Enums;
using Common.Interfaces;
using Common.Models;

namespace AssetServices
{
    public interface IAssetServices
    {
        Task<IList<CustomerAssetCount>> GetAllCustomerAssetsCountAsync();
        Task<int> GetAssetsCountAsync(Guid customerId, Guid? departmentId = null, AssetLifecycleStatus assetLifecycleStatus = AssetLifecycleStatus.Active);
        Task<IList<AssetLifecycleDTO>> GetAssetLifecyclesForUserAsync(Guid customerId, Guid userId);
        Task UnAssignAssetLifecyclesForUserAsync(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<PagedModel<AssetLifecycleDTO>> GetAssetLifecyclesForCustomerAsync(Guid customerId, string search, int page, int limit, AssetLifecycleStatus? status, CancellationToken cancellationToken);
        Task<AssetLifecycleDTO?> GetAssetLifecyclesForCustomerAsync(Guid customerId, Guid assetId);
        Task<IList<CustomerLabel>> AddLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Label> labels);
        Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId);
        Task<IList<CustomerLabel>> DeleteLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Guid> labelIds);
        Task<IList<CustomerLabel>> UpdateLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> updateLabels);
        Task<AssetLifecycleDTO> AddAssetLifecycleForCustomerAsync(Guid customerId, Guid callerId, string? alias, string? serialNumber, int assetCategoryId,
            string brand, string productName, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId,
            IList<long> imei, string? macAddress, Guid? managedByDepartmentId, string? note, string? description, decimal? PaidByCompany);
        Task<AssetLifecycleDTO?> ChangeAssetLifecycleTypeForCustomerAsync(Guid customerId, Guid assetId, Guid callerId, LifecycleType newLifecycleType);
        Task<IList<AssetLifecycleDTO>> UpdateStatusForMultipleAssetLifecycles(Guid customerId, Guid callerId, IList<Guid> assetLifecycleIdList, AssetLifecycleStatus lifecycleStatus);
        Task<AssetLifecycleDTO> MakeAssetAvailableAsync(Guid customerId, Guid callerId, Guid assetLifeCycleId);
        Task<AssetLifecycleDTO> UpdateAssetAsync(Guid customerId, Guid assetId, Guid callerId, string alias, string serialNumber, string brand, string model, DateTime purchaseDate, string note, string tag, string description, IList<long> imei);
        Task<AssetLifecycleDTO?> AssignAsset(Guid customerId, Guid assetId, Guid userId, Guid callerId);
        IList<AssetCategory> GetAssetCategories(string language = "EN");
        Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId, Guid callerId,string role);
        IList<(string Name, int EnumValue)> GetLifecycles();

        Task<IList<AssetLifecycleDTO>> AssignLabelsToAssetsAsync(Guid customerId, Guid callerId, IList<Guid> assetGuids,
            IList<Guid> labelGuids);

        Task<IList<AssetLifecycleDTO>> UnAssignLabelsToAssetsAsync(Guid customerId, Guid callerId,
            IList<Guid> assetGuids, IList<Guid> labelGuids);

        Task<decimal> GetCustomerTotalBookValue(Guid customerId);

    }
}