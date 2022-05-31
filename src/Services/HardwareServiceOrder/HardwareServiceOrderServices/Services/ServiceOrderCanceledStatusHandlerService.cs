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

        /// <inheritdoc cref="ServiceOrderStatusHandlerService.UpdateServiceOrderStatusAsync(Guid, ServiceStatusEnum)"/>
        public override async Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus)
        {
            if (newStatus != ServiceStatusEnum.Canceled)
                throw new ArgumentException("This handler connot handle this status");

            var order = await _hardwareServiceOrderRepository.UpdateOrderStatusAsync(orderId, newStatus);

            //Update asset
            await _assetService.UpdateAssetLifeCycleStatusAsync(order.CustomerId, order.AssetLifecycleId, Common.Enums.AssetLifecycleStatus.InUse);
        }
    }
}
