using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Common.Enums;
using AssetServices.ServiceModel;

namespace AssetServices.Models
{
    public interface IAssetLifecycleRepository
    {
        Task<AssetLifecycle> AddAsync(AssetLifecycle assetLifecycle);
        Task UnAssignAssetLifecyclesForUserAsync(Guid customerId, Guid userId, Guid? departmentId, Guid callerId);
        Task<PagedModel<CustomerAssetCount>> GetAssetLifecyclesCountsAsync(IList<Guid> customerIds, int page, int limit, CancellationToken cancellationToken);
        Task<IList<CustomerAssetCount>> GetAssetLifecyclesCountsAsync(IList<Guid> customerIds);
        Task<int> GetAssetLifecyclesCountAsync(Guid customerId, Guid? departmentId, AssetLifecycleStatus? assetLifecycleStatus);
        Task<PagedModel<AssetLifecycle>> GetAssetLifecyclesAsync(Guid customerId, string? userId, IList<AssetLifecycleStatus>? status, IList<Guid?>? department, int[]? category,
           Guid[]? label, bool? isActiveState, bool? isPersonal, DateTime? endPeriodMonth, DateTime? purchaseMonth, string? search, int page, int limit, CancellationToken cancellationToken,
           bool includeAsset = false, bool includeImeis = false, bool includeLabels = false, bool includeContractHolderUser = false);
        Task<AssetLifecycle?> GetAssetLifecycleAsync(Guid customerId, Guid assetLifecycleId, string? userId, IList<Guid?>? department,
            bool includeAsset = false, bool includeImeis = false, bool includeLabels = false, bool includeContractHolderUser = false, bool asNoTracking = false);
        Task<AssetLifecycle?> GetAssetLifecycleAsync(Guid assetLifeCycleId);
        Task<IList<AssetLifecycle>> GetAssetLifecyclesFromListAsync(Guid customerId, IList<Guid> assetGuidList, bool asNoTracking, bool includeAsset = false, bool includeImeis = false, bool includeContractHolderUser = false, bool includeLabels = false);
        Task<IList<AssetLifecycle>> GetAssetLifecyclesForUserAsync(Guid customerId, Guid userId, bool asNoTracking, bool includeAsset = false, bool includeImeis = false, bool includeContractHolderUser = false);
        Task<IList<CustomerLabel>> AddCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId, bool asNoTracking);
        Task<IList<CustomerLabel>> GetCustomerLabelsFromListAsync(IList<Guid> labelsGuid, Guid customerId);
        Task<CustomerLabel?> GetCustomerLabelAsync(Guid labelGuid, Guid customerId);
        Task<IList<CustomerLabel>> DeleteCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<IList<CustomerLabel>> UpdateCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default);
        Task<IList<FunctionalEventLogEntry>> GetAuditLog(Guid assetId);
        Task<User?> GetUser(Guid userId, bool asNoTracking = false);
        Task<decimal> GetCustomerTotalBookValue(Guid customerId);
        Task<AssetLifecycle?> MakeAssetAvailableAsync(Guid customerId, Guid callerId, Guid assetLifeCycleId);
        Task<IList<AssetLifecycle>> GetAssetForUser(Guid userId);
        Task<CustomerSettings?> GetCustomerSettingsAsync(Guid customerId, bool asNoTracking = false);

        Task<LifeCycleSetting?> GetCustomerLifeCycleSettingAssetCategory(Guid customerId, int assetCategoryId);
        Task<ServiceModel.CustomerAssetsCounterDTO> GetAssetLifecycleCountForCustomerAsync(Guid customerId, Guid? userId, IList<AssetLifecycleStatus> statuses);
        Task<ServiceModel.CustomerAssetsCounterDTO> GetAssetCountForDepartmentAsync(Guid customerId, Guid? userId, IList<AssetLifecycleStatus> status, IList<Guid?> department);
        Task<int> GetAssetLifecycleCountForUserAsync(Guid customerId, Guid? userId);
        Task<CustomerSettings> AddCustomerSettingAsync(CustomerSettings customerSettings, Guid customerId);
        /// <summary>
        ///     Get Imeis list that are Active and not deleted
        /// </summary>
        /// <param name="imeis"> List if imeis to search for </param>
        /// <returns></returns>
        Task<IList<string>> GetActiveImeisList(List<string> imeis);

        Task<PagedModel<AssetLifecycle>> AdvancedSearchAsync(SearchParameters searchParameters, int page, int limit, CancellationToken cancellationToken);
    }
}