using System.Text.Json;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Models;
using Microsoft.Extensions.Options;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Handles all service-order statuses for service-type --> "SUR"
    /// </summary>
    public class ServiceOrderStatusHandlerServiceForSUR : ServiceOrderStatusHandlerService
    {
        private readonly IAssetService _assetService;
        private readonly OrigoConfiguration _origoConfiguration;
        private readonly IEmailService _emailService;

        /// <summary>
        ///     Initializes a new <see cref="StatusHandlerFactory"/> class utilizing injections.
        /// </summary>
        /// <param name="options"> The injected <see cref="IOptions{TOptions}"/> interface. </param>
        /// <param name="emailService"> The injected <see cref="IEmailService"/> interface. </param>
        /// <param name="hardwareServiceOrderRepository"> The injected <see cref="IHardwareServiceOrderRepository"/> interface. </param>
        /// <param name="assetService"> The injected <see cref="IAssetService"/> interface. </param>
        public ServiceOrderStatusHandlerServiceForSUR(IOptions<OrigoConfiguration> options,
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IAssetService assetService,
            IEmailService emailService)
            : base(hardwareServiceOrderRepository)
        {
            _origoConfiguration = options.Value;
            _assetService = assetService;
            _emailService = emailService;
        }

        /// <inheritdoc cref="ServiceOrderStatusHandlerService.HandleServiceOrderCanceledStatusAsync"/>
        protected override async Task HandleServiceOrderCanceledStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            await UpdateServiceOrderStatusAsync(orderId, newStatus, newImeis, newSerialNumber);
        }

        /// <inheritdoc cref="ServiceOrderStatusHandlerService.HandleServiceOrderCompletedStatusAsync"/>
        protected override async Task HandleServiceOrderCompletedStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string> newImeis, string? newSerialNumber)
        {
            await UpdateServiceOrderStatusAsync(orderId, newStatus, newImeis, newSerialNumber);
        }

        /// <inheritdoc cref="ServiceOrderStatusHandlerService.HandleServiceOrderOngoingStatusAsync"/>
        protected override async Task HandleServiceOrderOngoingStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string> newImeis, string? newSerialNumber)
        {
            await UpdateServiceOrderStatusAsync(orderId, newStatus, newImeis, newSerialNumber);
        }

        /// <inheritdoc cref="ServiceOrderStatusHandlerService.HandleServiceOrderRegisteredStatusAsync"/>
        protected override async Task HandleServiceOrderRegisteredStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string> newImeis, string? newSerialNumber)
        {
            await UpdateServiceOrderStatusAsync(orderId, newStatus, newImeis, newSerialNumber);
        }

        /// <inheritdoc cref="ServiceOrderStatusHandlerService.HandleServiceOrderUnknownStatusAsync"/>
        protected override async Task HandleServiceOrderUnknownStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string> newImeis, string? newSerialNumber)
        {
            var order = await _hardwareServiceOrderRepository.UpdateOrderStatusAsync(orderId, ServiceStatusEnum.Unknown);

            // Handle email sending
            var parameters = new Dictionary<string, string>
            {
                { "Order", JsonSerializer.Serialize(order) },
                { "OrderLink", string.Format($"{_origoConfiguration.BaseUrl}/{_origoConfiguration.OrderPath}", order.CustomerId, order.ExternalId) }
            };

            await _emailService.SendEmailAsync(_origoConfiguration.DeveloperEmail, $"{ServiceStatusEnum.Unknown}_Subject", $"{ServiceStatusEnum.Unknown}_Body", parameters);
        }

        private async Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            var order = await _hardwareServiceOrderRepository.UpdateOrderStatusAsync(orderId, newStatus);
            
            // Update the asset-microservice
            await _assetService.UpdateAssetLifeCycleStatusAsync(order.AssetLifecycleId, newStatus, newImeis, newSerialNumber);
        }
    }
}
