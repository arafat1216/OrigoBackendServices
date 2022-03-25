using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

namespace AssetServices.Models
{
    public interface IAssetLifecycleRepository
    {
        Task<AssetLifecycle> AddAsync(AssetLifecycle assetLifecycle);
        Task<IList<CustomerAssetCount>> GetAssetLifecyclesCountsAsync();
        Task<int> GetAssetLifecyclesCountAsync(Guid customerId);
        Task<PagedModel<AssetLifecycle>> GetAssetLifecyclesAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken);
        Task<IList<AssetLifecycle>> GetAssetLifecyclesFromListAsync(Guid customerId, IList<Guid> assetGuidList);
        Task<IList<AssetLifecycle>> GetAssetLifecyclesForUserAsync(Guid customerId, Guid userId);
        Task<AssetLifecycle?> GetAssetLifecycleAsync(Guid customerId, Guid assetId);
        Task<IList<CustomerLabel>> AddCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId);
        Task<IList<CustomerLabel>> GetCustomerLabelsFromListAsync(IList<Guid> labelsGuid);
        Task<CustomerLabel> GetCustomerLabelAsync(Guid labelGuid);
        Task<IList<CustomerLabel>> DeleteCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<IList<CustomerLabel>> UpdateCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels);
        Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default);
        Task<IList<FunctionalEventLogEntry>> GetAuditLog(Guid assetId);
        Task<User?> GetUser(Guid userId);
    }
}