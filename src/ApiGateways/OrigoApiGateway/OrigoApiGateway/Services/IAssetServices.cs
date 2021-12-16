using Common.Models;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IAssetServices
    {
        Task<int> GetAssetsCountAsync(Guid customerId);
        Task<IList<object>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<IList<object>> GetAssetsForCustomerAsync(Guid customerId);
        Task<OrigoPagedAssets> SearchForAssetsForCustomerAsync(Guid customerId, string search = "", int page = 1, int limit = 50);
        Task<OrigoAsset> GetAssetForCustomerAsync(Guid customerId, Guid assetId);
        Task<OrigoAsset> AddAssetForCustomerAsync(Guid customerId, NewAsset newAsset);
        Task<IList<object>> UpdateStatusOnAssets(Guid customerId, IList<Guid> assetGuidList);
        Task<IList<Label>> CreateLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<NewLabel> newLabels);
        Task<IList<Label>> GetCustomerLabelsAsync(Guid customerId);
        Task<IList<Label>> DeleteCustomerLabelsAsync(Guid customerId, Guid callerId, IList<Guid> labelGuids);
        Task<IList<Label>> UpdateLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Label> labels);
        Task<IList<object>> AssignLabelsToAssets(Guid customerId, Guid callerId, AssetLabels assetLabels);
        Task<IList<object>> UnAssignLabelsFromAssets(Guid customerId, Guid callerId, AssetLabels assetLabels);
        Task<IList<OrigoAssetLifecycle>> GetLifecycles();
        Task<OrigoAsset> ChangeLifecycleType(Guid customerId, Guid assetId, int newLifecycleType);
        Task<OrigoAsset> UpdateAssetAsync(Guid customerId, Guid assetId, OrigoUpdateAsset updateAsset);
        Task<OrigoAsset> AssignAsset(Guid customerId, Guid assetId, Guid? userId);
        Task<IList<OrigoAssetCategory>> GetAssetCategoriesAsync();
        IList<AssetCategoryAttribute> GetAssetCategoryAttributesForCategory(int categoryId);
        Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId);
    }
}