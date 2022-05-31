using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Abstract class for handling all types of statuses update
    /// </summary>
    public abstract class ServiceOrderStatusHandlerService
    {
        /// <summary>
        /// Repository for managing hardware service order.
        /// </summary>
        protected readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;

        /// <summary>
        /// This constructor must be override by child classes
        /// </summary>
        /// <param name="hardwareServiceOrderRepository"></param>
        protected ServiceOrderStatusHandlerService(IHardwareServiceOrderRepository hardwareServiceOrderRepository)
        {
            _hardwareServiceOrderRepository = hardwareServiceOrderRepository;
        }

        /// <summary>
        /// Constructor when child does not need <see cref="_hardwareServiceOrderRepository"/>
        /// </summary>
        public ServiceOrderStatusHandlerService()
        {

        }

        /// <summary>
        /// Update the status of a service order
        /// </summary>
        /// <param name="orderId">Order Identifier</param>
        /// <param name="newStatus">New status <see cref="ServiceStatusEnum"/></param>
        /// <returns></returns>
        public abstract Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus);

        /// <summary>
        /// Update the status of asset life-cyle
        /// </summary>
        /// <param name="assetLifeCycleId">Asset life-cycle identifier</param>
        /// <param name="newStatus">New status <see cref="ServiceStatusEnum"/></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected async Task UpdateAssetLifeCycleStatusAsync(Guid assetLifeCycleId, ServiceStatusEnum newStatus)
        {
            throw new NotImplementedException();
        }
    }
}
