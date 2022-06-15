using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Handles Canceled status
    /// </summary>
    public class ServiceOrderCanceledStatusHandlerService : ServiceOrderStatusHandlerService
    {
        private readonly IAssetService _assetService;

        public ServiceOrderCanceledStatusHandlerService(
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IAssetService assetService)
            : base(hardwareServiceOrderRepository)
        {
            _assetService = assetService;
        }

        /// <inheritdoc/>
        public override async Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus, IEnumerable<string>? newImeis, string? newSerialNumber)
        {
            if (newStatus != ServiceStatusEnum.Canceled)
                throw new ArgumentException("This handler can't handle this status");

            var order = await _hardwareServiceOrderRepository.UpdateOrderStatusAsync(orderId, newStatus);

            // Update the asset-miroservice
            await _assetService.UpdateAssetLifeCycleStatusAsync(order.AssetLifecycleId, newStatus, newImeis, newSerialNumber);
        }
    }
}
