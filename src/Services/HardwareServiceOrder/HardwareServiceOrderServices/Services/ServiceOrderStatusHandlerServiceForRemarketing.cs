﻿using Common.Enums;
using Common.Extensions;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using Microsoft.Extensions.Options;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Handles all service-order statuses for service-type --> "Remarketing (Recycle and Wipe)"
    /// </summary>
    public class ServiceOrderStatusHandlerServiceForRemarketing : ServiceOrderStatusHandlerService
    {
        /// <inheritdoc />
        public ServiceOrderStatusHandlerServiceForRemarketing(IOptions<OrigoConfiguration> options,
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IAssetService assetService,
            IEmailService emailService) : base(options, hardwareServiceOrderRepository, assetService, emailService)
        {
        }
        
        /// <inheritdoc />
        public override async Task HandleServiceOrderRegisterAsync(HardwareServiceOrder hardwareServiceOrder,
            NewHardwareServiceOrderDTO serviceOrder,
            NewExternalServiceOrderDTO externalServiceOrder, 
            NewExternalServiceOrderResponseDTO externalServiceOrderResponse, 
            CustomerSettingsDTO customerSettings)
        {
            // Todo: Correct Address format
            if (serviceOrder.ServiceOrderAddons.Contains(ServiceOrderAddonsEnum.CONMODO_PACKAGING))
            {
                var orderConfirmationMailForPackaging = new RemarketingPackaging
                {
                    AssetId = serviceOrder.AssetInfo.AssetLifecycleId,
                    AssetName = $"{serviceOrder.AssetInfo.Brand} {serviceOrder.AssetInfo.Model}",
                    FirstName = serviceOrder.OrderedBy.FirstName,
                    OrderDate = hardwareServiceOrder.DateCreated.UtcDateTime,
                    Address = $"{serviceOrder.DeliveryAddress.Address1}", //Todo: Correct Address format
                    Recipient = serviceOrder.OrderedBy.Email,
                    Subject = RemarketingPackaging.SubjectKeyName
                };
                await _emailService.SendOrderConfirmationEmailAsync(orderConfirmationMailForPackaging, "en");
            }
            else
            {
                var orderConfirmationMailForNoPackaging = new RemarketingNoPackaging()
                {
                    AssetId = serviceOrder.AssetInfo.AssetLifecycleId,
                    AssetName = $"{serviceOrder.AssetInfo.Brand} {serviceOrder.AssetInfo.Model}",
                    FirstName = serviceOrder.OrderedBy.FirstName,
                    OrderDate = hardwareServiceOrder.DateCreated.UtcDateTime,
                    Recipient = serviceOrder.OrderedBy.Email,
                    Subject = RemarketingPackaging.SubjectKeyName
                };
                await _emailService.SendOrderConfirmationEmailAsync(orderConfirmationMailForNoPackaging, "en");
            }

            await _assetService.UpdateAssetLifeCycleStatusAsync("recycle", serviceOrder.AssetInfo.AssetLifecycleId,
                new { AssetLifecycleStatus = AssetLifecycleStatus.PendingRecycle, CallerId = Guid.Empty.SystemUserId() });
        }

        /// <inheritdoc />
        protected override async Task HandleServiceOrderCanceledStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            await UpdateServiceOrderStatusAsync(orderId, newStatus, newImeis, newSerialNumber);
        }

        /// <inheritdoc />
        protected override async Task HandleServiceOrderCompletedStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            await UpdateServiceOrderStatusAsync(orderId, newStatus, newImeis, newSerialNumber);
        }

        /// <inheritdoc />
        protected override async Task HandleServiceOrderOngoingStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            await _hardwareServiceOrderRepository.UpdateServiceOrderStatusAsync(orderId, newStatus);
        }

        /// <inheritdoc />
        protected override async Task HandleServiceOrderRegisteredStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            await _hardwareServiceOrderRepository.UpdateServiceOrderStatusAsync(orderId, newStatus);
        }
        
        private async Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            var order = await _hardwareServiceOrderRepository.UpdateServiceOrderStatusAsync(orderId, newStatus);
            await _assetService.UpdateAssetLifeCycleStatusForRemarketingAsync(order.AssetLifecycleId, newStatus, newImeis, newSerialNumber);
        }
    }
}
