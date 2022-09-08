using System.Text.Json;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using Microsoft.Extensions.Options;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Abstract class for handling all types of statuses update
    /// </summary>
    public abstract class ServiceOrderStatusHandlerService
    {
        /// <summary>
        /// Repository for managing hardware service order. <see cref="IHardwareServiceOrderRepository"/>
        /// </summary>
        protected readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;
        /// <summary>
        /// This Service is used to connect to Asset-microservice <see cref="IAssetService"/> which updates the status of an Asset.
        /// </summary>
        protected readonly IAssetService _assetService;
        /// <summary>
        /// Application configuration
        /// </summary>
        protected readonly OrigoConfiguration _origoConfiguration;
        /// <summary>
        /// Service to manage Email related functionalities <see cref="IEmailService"/>
        /// </summary>
        protected readonly IEmailService _emailService;
 
        /// <summary>
        ///     Initializes a new <see cref="StatusHandlerFactory"/> class utilizing injections.
        /// </summary>
        /// <param name="options"> The injected <see cref="IOptions{TOptions}"/> interface. </param>
        /// <param name="hardwareServiceOrderRepository"> The injected <see cref="IHardwareServiceOrderRepository"/> interface. </param>
        /// <param name="assetService"> The injected <see cref="IAssetService"/> interface. </param>
        /// <param name="emailService"> The injected <see cref="IEmailService"/> interface. </param>
        protected ServiceOrderStatusHandlerService(IOptions<OrigoConfiguration> options,
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IAssetService assetService,
            IEmailService emailService)
        {
            _origoConfiguration = options.Value;
            _hardwareServiceOrderRepository = hardwareServiceOrderRepository;
            _assetService = assetService;
            _emailService = emailService;
        }
        
        /// <summary>
        /// Constructor when child does not need <see cref="_hardwareServiceOrderRepository"/>
        /// </summary>
        protected ServiceOrderStatusHandlerService()
        {
        }

        /// <summary>
        /// Checks Whether the Status of a Service order has been updated or not. If so, then
        /// selects and executes the appropriate Function/Method based on the State and Status of a service order.
        /// The implementation can be overridden by subclass if the service-type/status/state requires different
        /// kinds of business logic.
        /// </summary>
        /// <param name="hardwareServiceOrder">Order Identifier</param>
        /// <param name="repairOrderDto">Represents the repair-order details that is retrieved from the external service-provider.<see cref="ExternalRepairServiceOrderDTO"/></param>
        /// <returns> The awaitable task. </returns>
        /// <exception cref="ArgumentException">Thrown if the latest status extracted from <paramref name="repairOrderDto"/>
        /// is invalid/unsupported.</exception>
        public virtual async Task HandleServiceOrderStatusAsync(HardwareServiceOrder hardwareServiceOrder, ExternalServiceOrderDTO repairOrderDto)
        {
            var lastOrderStatus = repairOrderDto.ExternalServiceEvents.OrderByDescending(m => m.Timestamp).FirstOrDefault()?.ServiceStatusId;
            lastOrderStatus = lastOrderStatus ?? 0;
            var lastOrderStatusEnum = (ServiceStatusEnum)lastOrderStatus;

            // Checking Whether Service order status has changed or not
            if (hardwareServiceOrder.StatusId == lastOrderStatus)
                return;

            switch (lastOrderStatusEnum)
            {
                case ServiceStatusEnum.Canceled:
                    await HandleServiceOrderCanceledStatusAsync(hardwareServiceOrder.ExternalId, lastOrderStatusEnum, new HashSet<string>() { repairOrderDto.ReturnedAsset?.Imei }, repairOrderDto.ReturnedAsset.SerialNumber);
                    break;
                case ServiceStatusEnum.CompletedNotRepaired:
                case ServiceStatusEnum.CompletedRepaired:
                case ServiceStatusEnum.CompletedRepairedOnWarranty:
                case ServiceStatusEnum.CompletedReplaced:
                case ServiceStatusEnum.CompletedReplacedOnWarranty:
                case ServiceStatusEnum.CompletedCredited:
                case ServiceStatusEnum.CompletedDiscarded:
                    await HandleServiceOrderCompletedStatusAsync(hardwareServiceOrder.ExternalId, lastOrderStatusEnum, new HashSet<string>() { repairOrderDto.ReturnedAsset?.Imei }, repairOrderDto.ReturnedAsset.SerialNumber);
                    break;
                case ServiceStatusEnum.Ongoing:
                case ServiceStatusEnum.OngoingUserActionNeeded:
                case ServiceStatusEnum.OngoingInTransit:
                case ServiceStatusEnum.OngoingReadyForPickup:
                    await HandleServiceOrderOngoingStatusAsync(hardwareServiceOrder.ExternalId, lastOrderStatusEnum, new HashSet<string>() { repairOrderDto.ReturnedAsset?.Imei }, repairOrderDto.ReturnedAsset.SerialNumber);
                    break;
                case ServiceStatusEnum.Registered:
                case ServiceStatusEnum.RegisteredInTransit:
                case ServiceStatusEnum.RegisteredUserActionNeeded:
                    await HandleServiceOrderRegisteredStatusAsync(hardwareServiceOrder.ExternalId, lastOrderStatusEnum, new HashSet<string>() { repairOrderDto.ReturnedAsset?.Imei }, repairOrderDto.ReturnedAsset.SerialNumber);
                    break;
                case ServiceStatusEnum.Unknown:
                    await HandleServiceOrderUnknownStatusAsync(hardwareServiceOrder.ExternalId, lastOrderStatusEnum, new HashSet<string>() { repairOrderDto.ReturnedAsset?.Imei }, repairOrderDto.ReturnedAsset.SerialNumber);
                    break;
                case ServiceStatusEnum.Null:
                default:
                    throw new ArgumentException("This handler can't handle this status");
            }
        }

        /// <summary>
        /// Handles the status of a service order for "Canceled" state
        /// </summary>
        /// <param name="orderId">Order Identifier</param>
        /// <param name="newStatus">New status <see cref="ServiceStatusEnum"/></param>
        /// <param name="newImeis"> If the device is replaced, a list containing the new asset's IMEI numbers. </param>
        /// <param name="newSerialNumber"> If the device is replaced, the asset's new serial-number. </param>
        /// <returns> The awaitable task. </returns>
        protected abstract Task HandleServiceOrderCanceledStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber);

        /// <summary>
        /// Handles the status of a service order for "Completed" state
        /// </summary>
        /// <param name="orderId">Order Identifier</param>
        /// <param name="newStatus">New status <see cref="ServiceStatusEnum"/></param>
        /// <param name="newImeis"> If the device is replaced, a list containing the new asset's IMEI numbers. </param>
        /// <param name="newSerialNumber"> If the device is replaced, the asset's new serial-number. </param>
        /// <returns> The awaitable task. </returns>
        protected abstract Task HandleServiceOrderCompletedStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber);

        /// <summary>
        /// Handles the status of a service order for "Ongoing" state
        /// </summary>
        /// <param name="orderId">Order Identifier</param>
        /// <param name="newStatus">New status <see cref="ServiceStatusEnum"/></param>
        /// <param name="newImeis"> If the device is replaced, a list containing the new asset's IMEI numbers. </param>
        /// <param name="newSerialNumber"> If the device is replaced, the asset's new serial-number. </param>
        /// <returns> The awaitable task. </returns>
        protected abstract Task HandleServiceOrderOngoingStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber);

        /// <summary>
        /// Handles the status of a service order for "Registered" state
        /// </summary>
        /// <param name="orderId">Order Identifier</param>
        /// <param name="newStatus">New status <see cref="ServiceStatusEnum"/></param>
        /// <param name="newImeis"> If the device is replaced, a list containing the new asset's IMEI numbers. </param>
        /// <param name="newSerialNumber"> If the device is replaced, the asset's new serial-number. </param>
        /// <returns> The awaitable task. </returns>
        protected abstract Task HandleServiceOrderRegisteredStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber);
 
        /// <summary>
        /// Handles the status of a service order for "Unknown" state
        /// </summary>
        /// <param name="orderId">Order Identifier</param>
        /// <param name="newStatus">New status <see cref="ServiceStatusEnum"/></param>
        /// <param name="newImeis"> If the device is replaced, a list containing the new asset's IMEI numbers. </param>
        /// <param name="newSerialNumber"> If the device is replaced, the asset's new serial-number. </param>
        /// <returns> The awaitable task. </returns>
        protected virtual async Task HandleServiceOrderUnknownStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string> newImeis, string? newSerialNumber)
        {
            var order = await _hardwareServiceOrderRepository.UpdateServiceOrderStatusAsync(orderId, ServiceStatusEnum.Unknown);

            // Handle email sending
            var parameters = new Dictionary<string, string>
            {
                { "Order", JsonSerializer.Serialize(order) },
                { "OrderLink", string.Format($"{_origoConfiguration.BaseUrl}/{_origoConfiguration.OrderPath}", order.CustomerId, order.ExternalId) }
            };

            await _emailService.SendEmailAsync(_origoConfiguration.DeveloperEmail, $"{ServiceStatusEnum.Unknown}_Subject", $"{ServiceStatusEnum.Unknown}_Body", parameters);
        }
    }
}
