using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

namespace AssetServices.Models
{
    public interface IAssetRepository
    {
        Task<Asset> AddAsync(Asset asset);
        Task<int> GetAssetsCountAsync(Guid customerId);
        Task<PagedModel<Asset>> GetAssetsAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken);
        Task<IList<Asset>> GetAssetsFromListAsync(Guid customerId, IList<Guid> assetGuidList);
        Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<Asset> GetAssetAsync(Guid customerId, Guid assetId);
        Task<IList<CustomerLabel>> AddCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId);
        Task<IList<CustomerLabel>> GetCustomerLabelsFromListAsync(IList<Guid> labelsGuid);
        Task<CustomerLabel> GetCustomerLabelAsync(Guid labelGuid);
        Task<IList<CustomerLabel>> DeleteCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<IList<CustomerLabel>> UpdateCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<IList<AssetLabel>> GetAssetLabelsFromListAsync(IList<int> labelInts);
        Task<AssetLabel> GetAssetLabelForAssetAsync(int assetId, int labelId);
        Task AddAssetLabelsForAsset(IList<AssetLabel> labels);
        Task DeleteLabelsFromAssetLabels(IList<int> labelIds);
        Task<AssetCategory> GetAssetCategoryAsync(int assetAssetCategoryId);
        Task<IList<AssetCategory>> GetAssetCategoriesAsync(string language = "EN");
        Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default);
        Task<IList<FunctionalEventLogEntry>> GetAuditLog(Guid assetId);
    }
}