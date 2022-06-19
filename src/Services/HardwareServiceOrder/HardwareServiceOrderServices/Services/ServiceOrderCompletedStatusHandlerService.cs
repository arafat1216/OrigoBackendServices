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

        /// <inheritdoc/>
        public override async Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            var order = await _hardwareServiceOrderRepository.UpdateOrderStatusAsync(orderId, newStatus);
            
            // Update the asset-miroservice
            await _assetService.UpdateAssetLifeCycleStatusAsync(order.AssetLifecycleId, newStatus, newImeis, newSerialNumber);
        }
    }
}
