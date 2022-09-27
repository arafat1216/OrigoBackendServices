using Common.Extensions;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Exceptions;
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

        /// <inheritdoc cref="ServiceOrderStatusHandlerService.HandleServiceOrderRegisterAsync"/>
        public override async Task HandleServiceOrderRegisterAsync(HardwareServiceOrder hardwareServiceOrder,
            NewHardwareServiceOrderDTO serviceOrder,
            NewExternalServiceOrderDTO externalServiceOrder,
            NewExternalServiceOrderResponseDTO externalServiceOrderResponse,
            CustomerSettingsDTO customerSettings)
        {
            await _assetService.UpdateAssetLifeCycleStatusAsync(HttpMethod.Patch, "send-to-repair", serviceOrder.AssetInfo.AssetLifecycleId, Guid.Empty.SystemUserId());

            // Todo: Is there any Packaging or No Packaging action involved? Need to ask Product Team

            var orderConfirmationMail = new OrderConfirmationEmail
            {
                AssetId = $"{serviceOrder.AssetInfo.AssetLifecycleId}",
                AssetName = $"{serviceOrder.AssetInfo.Brand} {serviceOrder.AssetInfo.Model}",
                FirstName = serviceOrder.OrderedBy.FirstName,
                OrderDate = hardwareServiceOrder.DateCreated.UtcDateTime,
                OrderLink = externalServiceOrderResponse.ExternalServiceManagementLink,
                Recipient = serviceOrder.OrderedBy.Email,
                LoanDeviceContact = customerSettings.LoanDevicePhoneNumber,
                Subject = OrderConfirmationEmail.SubjectKeyName,
                ShippingLabelLink = externalServiceOrderResponse.ExternalServiceManagementLink
            };

            try
            {
                await _emailService.SendOrderConfirmationEmailAsync(orderConfirmationMail, "en");
            }
            catch (EmailException ex)
            {
                //Todo: Need to investigate what should happen if the Email is not being sent
            }

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
