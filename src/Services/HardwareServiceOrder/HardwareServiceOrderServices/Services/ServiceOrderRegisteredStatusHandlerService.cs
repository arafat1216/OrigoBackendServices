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

        /// <inheritdoc cref="ServiceOrderStatusHandlerService.UpdateServiceOrderStatusAsync(Guid, ServiceStatusEnum)"/>
        public override async Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus)
        {
            if (!_statues.Any(m => m == newStatus))
                throw new ArgumentException("This handler connot handle this status");

            var order = await _hardwareServiceOrderRepository.UpdateOrderStatusAsync(orderId, newStatus);

            await _assetService.UpdateAssetLifeCycleStatusAsync(order.CustomerId, order.AssetLifecycleId, Common.Enums.AssetLifecycleStatus.Repair);
        }
    }
}
