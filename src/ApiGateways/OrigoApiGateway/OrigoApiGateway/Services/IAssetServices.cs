using Common.Enums;
using Common.Interfaces;
using Common.Models;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.Asset.Backend;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Services
{
    public interface IAssetServices
    {
        Task<PagedModel<CustomerAssetCount>> GetAllCustomerAssetsCountAsync(List<Guid> customerIds, int page = 1, int limit = 25);
        Task<IList<CustomerAssetCount>> GetAllCustomerAssetsCountAsync(List<Guid> customerIds);
        Task<int> GetAssetsCountAsync(Guid customerId, Guid? departmentId, AssetLifecycleStatus? assetLifecycleStatus);
        Task<IList<object>> GetAssetsForUserAsync(Guid customerId, Guid userId, bool includeAsset = false, bool includeImeis = false, bool includeContractHolderUser = false);
        Task<PagedModel<HardwareSuperType>> GetAssetsForCustomerAsync(Guid customerId,
            CancellationToken cancellationToken, FilterOptionsForAsset filterOptions, string search = "", int page = 1,
            int limit = 25, bool includeAsset = false, bool includeImeis = false, bool includeLabels = false,
            bool includeContractHolderUser = false);
        Task<OrigoAsset> GetAssetForCustomerAsync(Guid customerId, Guid assetId, FilterOptionsForAsset? filterOptions,
            bool includeAsset = false, bool includeImeis = false, bool includeLabels = false, bool includeContractHolderUser = false);
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
        Task<OrigoAsset> ReturnDeviceAsync(Guid customerId, Guid assetLifeCycleId, string role, List<Guid?> accessList, Guid returnLocationId, Guid callerId);
        Task<OrigoAsset> BuyoutDeviceAsync(Guid customerId, Guid assetLifeCycleId, string role, List<Guid?> accessList, string payrollContactEmail, Guid callerId);
        Task<OrigoAsset> PendingBuyoutDeviceAsync(Guid customerId, Guid assetLifeCycleId, string role, List<Guid?> accessList, string currency, Guid callerId);
        Task<OrigoAsset> ReportDeviceAsync(Guid customerId, ReportDevice data, string role, List<Guid?> accessList, Guid callerId);
        Task<IList<LifeCycleSetting>> GetLifeCycleSettingByCustomer(Guid customerId, string currency);
        Task<LifeCycleSetting> SetLifeCycleSettingForCustomerAsync(Guid customerId, NewLifeCycleSetting setting, string currency, Guid callerId);
        string GetCurrencyByCountry(string? country = null);
        Task<OrigoAsset> ReAssignAssetToDepartment(Guid customerId, Guid assetId, ReassignedToDepartmentDTO data);
        Task<OrigoAsset> ReAssignAssetToUser(Guid customerId, Guid assetId, ReassignedToUserDTO data);
        Task<OrigoCustomerAssetsCounter> GetAssetLifecycleCountersAsync(Guid customerId, FilterOptionsForAsset filter);

        Task<DisposeSetting> GetDisposeSettingByCustomer(Guid customerId);
        Task<DisposeSetting> SetDisposeSettingForCustomerAsync(Guid customerId, NewDisposeSetting setting, Guid callerId);
        Task<IList<OrigoAsset>> ActivateAssetStatusOnAssetLifecycle(Guid customerId, ChangeAssetStatusDTO changedAssetStatus);
        Task<IList<HardwareSuperType>> DeactivateAssetStatusOnAssetLifecycle(Guid customerId, ChangeAssetStatusDTO changedAssetStatus);
        Task<IList<ReturnLocation>> GetReturnLocationsByCustomer(Guid customerId);
        Task<ReturnLocation> AddReturnLocationsByCustomer(Guid customerId, NewReturnLocation data, IList<Location> officeLocation, Guid callerId);
        Task<ReturnLocation> UpdateReturnLocationsByCustomer(Guid customerId, Guid returnLocationId, NewReturnLocation data, Guid callerId);
        Task<IList<ReturnLocation>> DeleteReturnLocationsByCustomer(Guid customerId, Guid returnLocationId);
        Task<AssetValidationResult> ImportAssetsFileAsync(Organization customer, IFormFile file, bool validateOnly);

#nullable enable
        /// <summary>
        ///     An advanced search that retrieves all <c>Assets</c> that matches the given criteria.
        /// </summary>
        /// <param name="searchParameters"> A class containing all the search-parameters. </param>
        /// <param name="page"> The current page number. </param>
        /// <param name="limit"> The highest number of items that can be added in a single page. </param>
        /// <param name="includeImeis">
        ///     When <c><see langword="true"/></c>, the <c>IMEI</c> property is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <param name="includeLabels">
        ///     When <c><see langword="true"/></c>, the <c>Labels</c> property is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <param name="includeContractHolderUser">
        ///     When <c><see langword="true"/></c>, information about the user is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <returns> The asynchronous task. The task results contains the corresponding search-results. </returns>
        Task<PagedModel<HardwareSuperType>> AssetAdvancedSearch(AssetSearchParameters searchParameters, int page, int limit, bool includeImeis = false, bool includeLabels = false, bool includeContractHolderUser = false);
#nullable restore

    }
}