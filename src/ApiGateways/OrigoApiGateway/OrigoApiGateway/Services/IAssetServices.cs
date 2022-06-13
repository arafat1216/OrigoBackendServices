using Common.Enums;
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
        Task<HardwareSuperType> GetAssetAsync(Guid customerId, Guid assetId);
        Task<IList<CustomerAssetCount>> GetAllCustomerAssetsCountAsync();
        Task<int> GetAssetsCountAsync(Guid customerId, Guid? departmentId, AssetLifecycleStatus? assetLifecycleStatus);
        Task<IList<object>> GetAssetsForUserAsync(Guid customerId, Guid userId);
        Task<PagedModel<HardwareSuperType>> GetAssetsForCustomerAsync(Guid customerId,FilterOptionsForAsset filterOptions, string search = "", int page = 1, int limit = 1000);
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
        Task<IList<MinBuyoutPrice>> GetBaseMinBuyoutPrice(string? country = null, int? assetCategoryId = null);
        Task<OrigoAsset> ChangeLifecycleType(Guid customerId, Guid assetId, UpdateAssetLifecycleType data);
        Task<OrigoAsset> UpdateAssetAsync(Guid customerId, Guid assetId, OrigoUpdateAssetDTO updateAsset);
        Task<OrigoAsset> AssignAsset(Guid customerId, Guid assetId, AssignAssetToUser data);
        Task<IList<OrigoAssetCategory>> GetAssetCategoriesAsync(string? language = "EN");
        IList<AssetCategoryAttribute> GetAssetCategoryAttributesForCategory(int categoryId);
        Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId, Guid callerId, string role);
        Task<string> CreateAssetSeedData();
        Task<decimal> GetCustomerTotalBookValue(Guid customerId);
        Task<OrigoAsset> MakeAssetAvailableAsync(Guid customerId, MakeAssetAvailable data, Guid callerId);
        Task<IList<LifeCycleSetting>> GetLifeCycleSettingByCustomer(Guid customerId, string currency);
        Task<LifeCycleSetting> SetLifeCycleSettingForCustomerAsync(Guid customerId, NewLifeCycleSetting setting, string currency, Guid callerId);
        string GetCurrencyByCountry(string? country = null);
        Task<OrigoAsset> ReAssignAssetToDepartment(Guid customerId, Guid assetId, ReassignedToDepartmentDTO data);
        Task<OrigoAsset> ReAssignAssetToUser(Guid customerId, Guid assetId, ReassignedToUserDTO data);
        Task<OrigoCustomerAssetsCounter> GetAssetLifecycleCountersAsync(Guid customerId, FilterOptionsForAsset filter, bool manager);

        Task<DisposeSetting> GetDisposeSettingByCustomer(Guid customerId);
        Task<DisposeSetting> SetDisposeSettingForCustomerAsync(Guid customerId, NewDisposeSetting setting, Guid callerId);
    }
}