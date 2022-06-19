using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Handles Ongoing, User action needed, Repaired, Repaired: Warranty, Replaced, Replaced: Warranty, Discarded, Credited
    /// </summary>
    public class ServiceOrderOngoingStatusHandlerService : ServiceOrderStatusHandlerService
    {
        private readonly List<ServiceStatusEnum> _statues;
        private readonly IAssetService _assetService;

        public ServiceOrderOngoingStatusHandlerService(
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IAssetService assetService)
            : base(hardwareServiceOrderRepository)
        {
            _statues = new List<ServiceStatusEnum>()
            {
                ServiceStatusEnum.Ongoing,
                ServiceStatusEnum.OngoingUserActionNeeded,
                ServiceStatusEnum.OngoingInTransit,
                ServiceStatusEnum.OngoingReadyForPickup
            };
            _assetService = assetService;
        }

        /// <inheritdoc/>
        public override async Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            if (!_statues.Any(m => m == newStatus))
                throw new ArgumentException("This handler can't handle this status");

            var order = await _hardwareServiceOrderRepository.UpdateOrderStatusAsync(orderId, newStatus);

            // Update the asset-miroservice
            await _assetService.UpdateAssetLifeCycleStatusAsync(order.AssetLifecycleId, newStatus, newImeis, newSerialNumber);
        }
    }
}
