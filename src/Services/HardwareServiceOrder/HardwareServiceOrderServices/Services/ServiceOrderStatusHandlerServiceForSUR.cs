using System.Text.Json;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using Microsoft.Extensions.Options;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Handles all service-order statuses for service-type --> "SUR"
    /// </summary>
    public class ServiceOrderStatusHandlerServiceForSUR : ServiceOrderStatusHandlerService
    {
        /// <inheritdoc />
        public ServiceOrderStatusHandlerServiceForSUR(IOptions<OrigoConfiguration> options,
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IAssetService assetService,
            IEmailService emailService)
            : base(options, hardwareServiceOrderRepository, assetService, emailService)
        {
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

        private async Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            var order = await _hardwareServiceOrderRepository.UpdateServiceOrderStatusAsync(orderId, newStatus);
            
            // Update the asset-microservice
            await _assetService.UpdateAssetLifeCycleStatusForSURAsync(order.AssetLifecycleId, newStatus, newImeis, newSerialNumber);
        }
    }
}
