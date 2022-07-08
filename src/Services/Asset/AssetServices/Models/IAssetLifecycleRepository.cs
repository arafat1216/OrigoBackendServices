using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Common.Enums;

namespace AssetServices.Models
{
    public interface IAssetLifecycleRepository
    {
        Task<AssetLifecycle> AddAsync(AssetLifecycle assetLifecycle);
        Task UnAssignAssetLifecyclesForUserAsync(Guid customerId, Guid userId, Guid? departmentId, Guid callerId);
        Task<IList<CustomerAssetCount>> GetAssetLifecyclesCountsAsync();
        Task<int> GetAssetLifecyclesCountAsync(Guid customerId, Guid? departmentId, AssetLifecycleStatus? assetLifecycleStatus);
        Task<PagedModel<AssetLifecycle>> GetAssetLifecyclesAsync(Guid customerId, string? userId, IList<AssetLifecycleStatus>? status, IList<Guid?>? department, int[]? category,
           Guid[]? label, bool? isActiveState, bool? isPersonal, DateTime? endPeriodMonth, DateTime? purchaseMonth, string search, int page, int limit, CancellationToken cancellationToken);
        Task<AssetLifecycle?> GetAssetLifecycleAsync(Guid customerId, Guid assetId, string? userId);
        Task<AssetLifecycle?> GetAssetLifecycleAsync(Guid assetLifeCycleId);
        Task<IList<AssetLifecycle>> GetAssetLifecyclesFromListAsync(Guid customerId, IList<Guid> assetGuidList);
        Task<IList<AssetLifecycle>> GetAssetLifecyclesForUserAsync(Guid customerId, Guid userId);
        Task<IList<CustomerLabel>> AddCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId);
        Task<IList<CustomerLabel>> GetCustomerLabelsFromListAsync(IList<Guid> labelsGuid, Guid customerId);
        Task<CustomerLabel?> GetCustomerLabelAsync(Guid labelGuid, Guid customerId);
        Task<IList<CustomerLabel>> DeleteCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<IList<CustomerLabel>> UpdateCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default);
        Task<IList<FunctionalEventLogEntry>> GetAuditLog(Guid assetId);
        Task<User?> GetUser(Guid userId);
        Task<decimal> GetCustomerTotalBookValue(Guid customerId);
        Task<AssetLifecycle?> MakeAssetAvailableAsync(Guid customerId, Guid callerId, Guid assetLifeCycleId);
        Task<IList<AssetLifecycle>> GetAssetForUser(Guid userId);
        Task<CustomerSettings?> GetCustomerSettingsAsync(Guid customerId);

        Task<LifeCycleSetting?> GetCustomerLifeCycleSettingAssetCategory(Guid customerId, int assetCategoryId);
        Task<ServiceModel.CustomerAssetsCounterDTO> GetAssetLifecycleCountForCustomerAsync(Guid customerId, Guid? userId, IList<AssetLifecycleStatus> statuses);
        Task<ServiceModel.CustomerAssetsCounterDTO> GetAssetCountForDepartmentAsync(Guid customerId, Guid? userId, IList<AssetLifecycleStatus> status, IList<Guid?> department);
        Task<int> GetAssetLifecycleCountForUserAsync(Guid customerId, Guid? userId);
        Task<CustomerSettings> AddCustomerSettingAsync(CustomerSettings customerSettings, Guid customerId);
    }
}