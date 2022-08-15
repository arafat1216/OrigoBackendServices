using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.Models;
using AssetServices.ServiceModel;
using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Http;

namespace AssetServices
{
    public interface IAssetServices
    {
        Task<IList<CustomerAssetCount>> GetAllCustomerAssetsCountAsync();
        Task<int> GetAssetsCountAsync(Guid customerId, AssetLifecycleStatus? assetLifecycleStatus, Guid? departmentId = null);
        Task<IList<AssetLifecycleDTO>> GetAssetLifecyclesForUserAsync(Guid customerId, Guid userId);
        Task UnAssignAssetLifecyclesForUserAsync(Guid customerId, Guid userId, Guid? departmentId, Guid callerId);
        Task<PagedModel<AssetLifecycleDTO>> GetAssetLifecyclesForCustomerAsync(Guid customerId,string? userId, IList<AssetLifecycleStatus>? status, IList<Guid?>? department, int[]? category,
           Guid[]? label, bool? isActiveState, bool? isPersonal, DateTime? endPeriodMonth, DateTime? purchaseMonth, string search, int page, int limit, CancellationToken cancellationToken);
        Task<AssetLifecycleDTO?> GetAssetLifecycleForCustomerAsync(Guid customerId, Guid assetId, string? userId, IList<Guid?>? department);
        Task<IList<CustomerLabel>> AddLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Label> labels);
        Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId);
        Task<IList<CustomerLabel>> DeleteLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Guid> labelIds);
        Task<IList<CustomerLabel>> UpdateLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> updateLabels);
        Task<AssetLifecycleDTO?> ChangeAssetLifecycleTypeForCustomerAsync(Guid customerId, Guid assetId, Guid callerId, LifecycleType newLifecycleType);
        Task<IList<AssetLifecycleDTO>> UpdateStatusForMultipleAssetLifecycles(Guid customerId, Guid callerId, IList<Guid> assetLifecycleIdList, AssetLifecycleStatus lifecycleStatus);
        Task<AssetLifecycleDTO> MakeAssetAvailableAsync(Guid customerId, MakeAssetAvailableDTO data);
        Task<AssetLifecycleDTO> UpdateAssetAsync(Guid customerId, Guid assetId, Guid callerId, string? alias, string? serialNumber, string? brand, string? model, DateTime? purchaseDate, string? note, string? tag, string? description, IList<long>? imei, string? macAddress);
        IList<AssetCategory> GetAssetCategories(string language = "EN");
        Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId, Guid callerId,string role);
        IList<(string Name, int EnumValue)> GetLifecycles();

        Task<IList<AssetLifecycleDTO>> AssignLabelsToAssetsAsync(Guid customerId, Guid callerId, IList<Guid> assetGuids,
            IList<Guid> labelGuids);

        Task<IList<AssetLifecycleDTO>> UnAssignLabelsToAssetsAsync(Guid customerId, Guid callerId,
            IList<Guid> assetGuids, IList<Guid> labelGuids);

        Task<decimal> GetCustomerTotalBookValue(Guid customerId);

        Task<AssetLifecycleDTO> AddAssetLifecycleForCustomerAsync(Guid customerId, NewAssetDTO newAssetDTO);
        Task<LifeCycleSettingDTO> AddLifeCycleSettingForCustomerAsync(Guid customerId, LifeCycleSettingDTO lifeCycleSettingDTO, Guid CallerId);
        Task<LifeCycleSettingDTO> UpdateLifeCycleSettingForCustomerAsync(Guid customerId, LifeCycleSettingDTO lifeCycleSettingDTO, Guid CallerId);
        Task<IList<LifeCycleSettingDTO>> GetLifeCycleSettingByCustomer(Guid customerId);
        IList<MinBuyoutPriceBaseline> GetBaseMinBuyoutPrice(string? country, int? assetCategoryId);
        Task<AssetLifecycleDTO> AssignAssetLifeCycleToHolder(Guid customerId, Guid assetId, AssignAssetDTO assignAssetDTO);
        Task AssetLifeCycleSendToRepair(Guid assetLifecycleId, Guid callerId);
        Task AssetLifeCycleRepairCompleted(Guid assetLifecycleId, AssetLifeCycleRepairCompleted assetLifeCycleRepairCompleted);
        Task<DisposeSettingDTO> AddDisposeSettingForCustomerAsync(Guid customerId, DisposeSettingDTO disposeSettingDTO, Guid CallerId);
        Task<DisposeSettingDTO> UpdateDisposeSettingForCustomerAsync(Guid customerId, DisposeSettingDTO disposeSettingDTO, Guid CallerId);
        Task<DisposeSettingDTO> GetDisposeSettingByCustomer(Guid customerId);
        Task<CustomerAssetsCounterDTO> GetAssetLifecycleCountersAsync(Guid customerId, IList<AssetLifecycleStatus>? filterStatus, IList<Guid?>? departmentId, Guid? userId);
        Task<AssetLifecycleDTO> ReturnDeviceAsync(Guid customerId, ReturnDeviceDTO data);
        Task<IList<AssetLifecycleDTO>> ActivateAssetLifecycleStatus(Guid customerId, ChangeAssetStatus assetLifecycles);
        Task<IList<AssetLifecycleDTO>> DeactivateAssetLifecycleStatus(Guid customerId, ChangeAssetStatus assetLifecycles);
        Task<AssetLifecycleDTO> BuyoutDeviceAsync(Guid customerId, BuyoutDeviceDTO data);
        Task<AssetLifecycleDTO> ReportDeviceAsync(Guid customerId, ReportDeviceDTO data);
        Task<ReturnLocationDTO> AddReturnLocationsByCustomer(Guid customerId, ReturnLocationDTO returnLocationDTO, Guid callerId);
        Task<ReturnLocationDTO> UpdateReturnLocationsByCustomer(Guid customerId, Guid returnLocationId, ReturnLocationDTO returnLocationDTO, Guid callerId);
        Task<IList<ReturnLocationDTO>> RemoveReturnLocationsByCustomer(Guid customerId, Guid returnLocationId, Guid callerId);
        Task<IList<ReturnLocationDTO>> GetReturnLocationsByCustomer(Guid customerId);
        Task<AssetLifecycleDTO> MakeAssetExpiredAsync(Guid customerId, Guid assetId, Guid callerId);
        Task<AssetLifecycleDTO> MakeAssetExpiresSoonAsync(Guid customerId, Guid assetId, Guid callerId);
        Task SyncDepartmentForUserToAssetLifecycleAsync(Guid customerId, Guid userId, Guid? departmentId, Guid callerId);
        Task<AssetLifecycleDTO> PendingBuyoutDeviceAsync(Guid customerId, PendingBuyoutDeviceDTO data);
        Task CancelUserOffboarding(Guid customerId, Guid userId, Guid callerId);
        AssetValidationResult ImportAssetsFromFile(Guid customerId, IFormFile file, bool validateOnly);
        Task<AssetLifecycleDTO> ConfirmBuyoutDeviceAsync(Guid customerId, BuyoutDeviceDTO data);
        /// <summary>
        /// Updates the asset lifecycle to an active state when a order to cancel a return is made.
        /// Also calculates if the asset lifecycle has expired or is expiring soon based on the assets end date. 
        /// </summary>
        /// <param name="assetLifecycleId">Asset lifecycle id.</param>
        /// <param name="callerId">The id of the user making the request.</param>
        /// <returns>Asynchronously task that returns no value.</returns>
        Task CancelReturn(Guid assetLifecycleId, Guid callerId);

        /// <summary>
        /// Makes change to asset status based on recycle order requirements. 
        /// </summary>
        /// <param name="assetLifecycleId">Asset lifecycle id.</param>
        /// <param name="assetLifecycleStatus">The asset lifecycle status the asset is to be changed to.</param>
        /// <param name="callerId">The identification for the caller who is making the request.</param>
        /// <returns>Asynchronously task that returns no value.</returns>
        Task RecycleAssetLifecycle(Guid assetLifecycleId, int assetLifecycleStatus, Guid callerId);

    }
}