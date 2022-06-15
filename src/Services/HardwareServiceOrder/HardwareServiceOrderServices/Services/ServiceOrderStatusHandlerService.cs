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
        protected ServiceOrderStatusHandlerService()
        {
        }

        /// <summary>
        /// Update the status of a service order
        /// </summary>
        /// <param name="orderId">Order Identifier</param>
        /// <param name="newStatus">New status <see cref="ServiceStatusEnum"/></param>
        /// <param name="newImeis"> If the device is replaced, a list containing the new asset's IMEI numbers. </param>
        /// <param name="newSerialNumber"> If the device is replaced, the asset's new serial-number. </param>
        /// <returns> The awaitiable task. </returns>
        public abstract Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus, IEnumerable<string>? newImeis, string? newSerialNumber);
    }
}
