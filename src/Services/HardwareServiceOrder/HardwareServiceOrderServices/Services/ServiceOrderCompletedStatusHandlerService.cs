using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Handles Not repaired, Repaired, Repaired: Warranty, Replaced, Replaced: Warranty, Discarded, Credited
    /// </summary>
    public class ServiceOrderCompletedStatusHandlerService : ServiceOrderStatusHandlerService
    {
        private readonly List<ServiceStatusEnum> _statues;
        private readonly IAssetService _assetService;

        public ServiceOrderCompletedStatusHandlerService(
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IAssetService assetService) :
            base(hardwareServiceOrderRepository)
        {
            _statues = new List<ServiceStatusEnum>
            {
                ServiceStatusEnum.CompletedNotRepaired,
                ServiceStatusEnum.CompletedRepaired,
                ServiceStatusEnum.CompletedRepairedOnWarranty,
                ServiceStatusEnum.CompletedReplaced,
                ServiceStatusEnum.CompletedReplacedOnWarranty,
                ServiceStatusEnum.CompletedCredited,
                ServiceStatusEnum.CompletedDiscarded
            };
            _assetService = assetService;
        }

        /// <inheritdoc cref="ServiceOrderStatusHandlerService.UpdateServiceOrderStatusAsync(Guid, ServiceStatusEnum)"/>
        public override async Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus)
        {
            var order = await _hardwareServiceOrderRepository.UpdateOrderStatusAsync(orderId, newStatus);

            var assetLifeCycleStatus = newStatus == ServiceStatusEnum.CompletedDiscarded ? Common.Enums.AssetLifecycleStatus.Discarded : Common.Enums.AssetLifecycleStatus.InUse;

            await _assetService.UpdateAssetLifeCycleStatusAsync(order.CustomerId, order.AssetLifecycleId, assetLifeCycleStatus);
        }
    }
}
