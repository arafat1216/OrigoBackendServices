using Common.Interfaces;
using Common.Models;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IAssetServices
    {
        Task<IList<CustomerAssetCount>> GetAllCustomerAssetsCountAsync();
        Task<int> GetAssetsCountAsync(Guid customerId);
        Task<IList<object>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<PagedModel<HardwareSuperType>> GetAssetsForCustomerAsync(Guid customerId, string search = "", int page = 1, int limit = 1000);
        Task<OrigoPagedAssets> SearchForAssetsForCustomerAsync(Guid customerId, string search = "", int page = 1, int limit = 50);
        Task<OrigoAsset> GetAssetForCustomerAsync(Guid customerId, Guid assetId);
        Task<OrigoAsset> AddAssetForCustomerAsync(Guid customerId, NewAssetDTO newAsset);
        Task<IList<object>> UpdateStatusOnAssets(Guid customerId, UpdateAssetsStatus updatedAssetStatus, Guid callerId);
        Task<IList<Label>> CreateLabelsForCustomerAsync(Guid customerId, AddLabelsData data);
        Task<IList<Label>> GetCustomerLabelsAsync(Guid customerId);
        Task<IList<Label>> DeleteCustomerLabelsAsync(Guid customerId, DeleteCustomerLabelsData data);
        Task<IList<Label>> UpdateLabelsForCustomerAsync(Guid customerId, UpdateCustomerLabelsData data);
        Task<IList<object>> AssignLabelsToAssets(Guid customerId, AssetLabelsDTO assetLabels);
        Task<IList<object>> UnAssignLabelsFromAssets(Guid customerId, AssetLabelsDTO assetLabels);
        Task<IList<OrigoAssetLifecycle>> GetLifecycles();
        Task<OrigoAsset> ChangeLifecycleType(Guid customerId, Guid assetId, UpdateAssetLifecycleType data);
        Task<OrigoAsset> UpdateAssetAsync(Guid customerId, Guid assetId, OrigoUpdateAssetDTO updateAsset);
        Task<OrigoAsset> AssignAsset(Guid customerId, Guid assetId, AssignAssetToUser data);
        Task<IList<OrigoAssetCategory>> GetAssetCategoriesAsync();
        IList<AssetCategoryAttribute> GetAssetCategoryAttributesForCategory(int categoryId);
        Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId);
        Task<string> CreateAssetSeedData();
        Task<decimal> GetCustomerTotalBookValue(Guid customerId);
    }
}