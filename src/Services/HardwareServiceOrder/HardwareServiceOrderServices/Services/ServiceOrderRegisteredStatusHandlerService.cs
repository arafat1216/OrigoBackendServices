using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Handles Registered, In shipping and User action needed statuses
    /// </summary>
    public class ServiceOrderRegisteredStatusHandlerService : ServiceOrderStatusHandlerService
    {
        private readonly List<ServiceStatusEnum> _statues;
        private readonly IAssetService _assetService;

        public ServiceOrderRegisteredStatusHandlerService(
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IAssetService assetService)
            : base(hardwareServiceOrderRepository)
        {
            _statues = new List<ServiceStatusEnum>
            {
                ServiceStatusEnum.Registered,
                ServiceStatusEnum.RegisteredInTransit,
                ServiceStatusEnum.RegisteredUserActionNeeded
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
