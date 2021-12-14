using System;
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
        Task<int> GetAssetsCountAsync(Guid customerId);
        Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<PagedModel<Asset>> GetAssetsForCustomerAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken);
        Task<IList<Asset>> GetAssetsForCustomerFromListAsync(Guid customerId, IList<Guid> assetGuids);
        Task<Asset> GetAssetForCustomerAsync(Guid customerId, Guid assetId);
        Task<IList<CustomerLabel>> AddLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Label> labels);
        Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId);
        Task<IList<CustomerLabel>> GetCustomerLabelsAsync(IList<Guid> customerLabelGuids);
        Task<IList<CustomerLabel>> DeleteLabelsForCustomerAsync(Guid customerId, IList<Guid> labelGuids);
        Task<IList<CustomerLabel>> SoftDeleteLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Guid> labelGuids);
        Task<IList<CustomerLabel>> UpdateLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> updateLabels);
        Task<IList<Asset>> AssignLabelsToAssetsAsync(Guid customerId, Guid callerId, IList<Guid> assetGuids, IList<Guid> labelGuids);
        Task AssignLabelsToAssetAsync(Asset asset, IList<CustomerLabel> customerLabels, Guid callerId);
        Task SoftDeleteAssetLabelsAsync(Guid callerId, IList<int> assetLabelIds);
        Task<IList<Asset>> UnAssignLabelsToAssetsAsync(Guid customerId, Guid callerId, IList<Guid> assetGuids, IList<Guid> labelGuids);
        Task UnAssignLabelsToAssetAsync(Asset asset, IList<CustomerLabel> customerLabels, Guid callerId);
        Task<Asset> AddAssetForCustomerAsync(Guid customerId, Guid callerId, string alias, string serialNumber, int assetCategoryId,
            string brand, string productName, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId,
            IList<long> imei, string macAddress, Guid? managedByDepartmentId, AssetStatus status, string note, string tag, string description);
        Task<Asset> ChangeAssetLifecycleTypeForCustomerAsync(Guid customerId, Guid assetId, Guid callerId, LifecycleType newLifecycleType);
        Task<IList<Asset>> UpdateMultipleAssetsStatus(Guid customerId, Guid callerId, IList<Guid> assetGuidList, AssetStatus status);
        Task<Asset> UpdateAssetAsync(Guid customerId, Guid assetId, string alias, string serialNumber, string brand, string model, DateTime purchaseDate, string note, string tag, string description, IList<long> imei);
        Task<Asset> AssignAsset(Guid customerId, Guid assetId, Guid? userId);
        Task<IList<AssetCategory>> GetAssetCategoriesAsync(string language = "EN");
        Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId);
        IList<AssetLifecycle> GetLifecycles();
    }
}