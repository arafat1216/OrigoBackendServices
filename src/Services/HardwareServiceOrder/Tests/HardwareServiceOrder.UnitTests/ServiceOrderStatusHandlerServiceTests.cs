using Common.Enums;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.Services;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace HardwareServiceOrder.UnitTests
{
    public class ServiceOrderStatusHandlerServiceTests
    {
        private readonly IOptions<OrigoConfiguration> _options;

        public ServiceOrderStatusHandlerServiceTests()
        {
            _options = Options.Create(new OrigoConfiguration
            {
                BaseUrl = "https://origov2dev.mytos.no",
                DeveloperEmail = "it@mytos.no",
                OrderPath = "my-business/{0}/hardware-repair/{1}/view"
            });
        }

        [Fact]
        public async Task HandleUnknownStatus()
        {
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                callerId: Guid.NewGuid(),
                externalId: Guid.NewGuid(),
                customerId: Guid.NewGuid(),
                assetLifecycleId: Guid.NewGuid(),
                userDescription: "[UserDescription]",
                owner: new ContactDetails(
                    userId: Guid.NewGuid(),
                    firstName: "[FirstName]",
                    lastName: "[LastName]",
                    email: "[Email]",
                    phoneNumber: "[PhoneNumber]"
                ),
                deliveryAddress: new DeliveryAddress(
                    recipientType: RecipientTypeEnum.Personal,
                    recipient: "[Recipient]",
                    address1: "[Address1]",
                    address2: "[Address2]",
                    postalCode: "[PostalCode]",
                    city: "[City]",
                    country: "[Country]"),
                serviceTypeId: (int)ServiceTypeEnum.SUR,
                statusId: (int)ServiceStatusEnum.Unknown,
                serviceProviderId: 1,
                serviceProviderOrderId1: "[ServiceProviderOrderId1]",
                serviceProviderOrderId2: "[ServiceProviderOrderId2]",
                externalServiceManagementLink: "[ExternalServiceManagementLink]",
                serviceEvents: new List<ServiceEvent>()
            );

            var emailMock = new Mock<IEmailService>();
            emailMock.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                            .ReturnsAsync(order);

            var serviceOrderUnKnownStatusHandlerService = new ServiceOrderUnknownStatusHandlerService(options: _options, emailService: emailMock.Object, hardwareServiceOrderRepository: hwRepositoryMock.Object);


            await serviceOrderUnKnownStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.Unknown, null, null);

            // Verify
            var parameters = new Dictionary<string, string>
            {
                { "Order", JsonSerializer.Serialize(order) },
                { "OrderLink", string.Format($"{_options.Value.BaseUrl}/{_options.Value.OrderPath}", order.CustomerId, order.ExternalId) }
            };

            emailMock.Verify(m => m.SendEmailAsync(_options.Value.DeveloperEmail, $"{ServiceStatusEnum.Unknown}_Subject", $"{ServiceStatusEnum.Unknown}_Body", parameters, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleCanceledStatus()
        {
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                callerId: Guid.NewGuid(),
                externalId: Guid.NewGuid(),
                customerId: Guid.NewGuid(),
                assetLifecycleId: Guid.NewGuid(),
                userDescription: "[UserDescription]",
                owner: new ContactDetails(
                    userId: Guid.NewGuid(),
                    firstName: "[FirstName]",
                    lastName: "[LastName]",
                    email: "[Email]",
                    phoneNumber: "[PhoneNumber]"
                ),
                deliveryAddress: new DeliveryAddress(
                    recipientType: RecipientTypeEnum.Personal,
                    recipient: "[Recipient]",
                    address1: "[Address1]",
                    address2: "[Address2]",
                    postalCode: "[PostalCode]",
                    city: "[City]",
                    country: "[Country]"),
                serviceTypeId: (int)ServiceTypeEnum.SUR,
                statusId: (int)ServiceStatusEnum.Canceled,
                serviceProviderId: 1,
                serviceProviderOrderId1: "[ServiceProviderOrderId1]",
                serviceProviderOrderId2: "[ServiceProviderOrderId2]",
                externalServiceManagementLink: "[ExternalServiceManagementLink]",
                serviceEvents: new List<ServiceEvent>()
            );

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => m.UpdateAssetLifeCycleStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<string?>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                            .ReturnsAsync(order);

            var serviceOrderCancelledStatusHandlerService = new ServiceOrderCanceledStatusHandlerService(hwRepositoryMock.Object, assetMock.Object);


            await serviceOrderCancelledStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.Canceled, null, null);

            // Verify
            assetMock.Verify(m => m.UpdateAssetLifeCycleStatusAsync(order.AssetLifecycleId, ServiceStatusEnum.Canceled, null, null), Times.Once());
        }

        [Fact]
        public async Task HandleOngoingStatus()
        {
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                callerId: Guid.NewGuid(),
                externalId: Guid.NewGuid(),
                customerId: Guid.NewGuid(),
                assetLifecycleId: Guid.NewGuid(),
                userDescription: "[UserDescription]",
                owner: new ContactDetails(
                userId: Guid.NewGuid(),
                firstName: "[FirstName]",
                lastName: "[LastName]",
                email: "[Email]",
                phoneNumber: "[PhoneNumber]"
                ),
                deliveryAddress: new DeliveryAddress(
                    recipientType: RecipientTypeEnum.Personal,
                    recipient: "[Recipient]",
                    address1: "[Address1]",
                    address2: "[Address2]",
                    postalCode: "[PostalCode]",
                    city: "[City]",
                    country: "[Country]"),
                serviceTypeId: (int)ServiceTypeEnum.SUR,
                statusId: (int)ServiceStatusEnum.Ongoing,
                serviceProviderId: 1,
                serviceProviderOrderId1: "[ServiceProviderOrderId1]",
                serviceProviderOrderId2: "[ServiceProviderOrderId2]",
                externalServiceManagementLink: "[ExternalServiceManagementLink]",
                serviceEvents: new List<ServiceEvent>()
            );

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => m.UpdateAssetLifeCycleStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<string?>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                            .ReturnsAsync(order);

            var serviceOrderOngoingStatusHandlerService = new ServiceOrderOngoingStatusHandlerService(hwRepositoryMock.Object, assetMock.Object);


            await serviceOrderOngoingStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.Ongoing, null, null);

            // Verify
            assetMock.Verify(m => m.UpdateAssetLifeCycleStatusAsync(order.AssetLifecycleId, ServiceStatusEnum.Ongoing, null, null), Times.Once());
        }

        [Fact]
        public async Task HandleCompletedStatus_CompletedRepaired()
        {
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                callerId: Guid.NewGuid(),
                externalId: Guid.NewGuid(),
                customerId: Guid.NewGuid(),
                assetLifecycleId: Guid.NewGuid(),
                userDescription: "[UserDescription]",
                owner: new ContactDetails(
                userId: Guid.NewGuid(),
                firstName: "[FirstName]",
                lastName: "[LastName]",
                email: "[Email]",
                phoneNumber: "[PhoneNumber]"
                ),
                deliveryAddress: new DeliveryAddress(
                    recipientType: RecipientTypeEnum.Personal,
                    recipient: "[Recipient]",
                    address1: "[Address1]",
                    address2: "[Address2]",
                    postalCode: "[PostalCode]",
                    city: "[City]",
                    country: "[Country]"),
                serviceTypeId: (int)ServiceTypeEnum.SUR,
                statusId: (int)ServiceStatusEnum.CompletedRepaired,
                serviceProviderId: 1,
                serviceProviderOrderId1: "[ServiceProviderOrderId1]",
                serviceProviderOrderId2: "[ServiceProviderOrderId2]",
                externalServiceManagementLink: "[ExternalServiceManagementLink]",
                serviceEvents: new List<ServiceEvent>()
            );

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => m.UpdateAssetLifeCycleStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<string?>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                            .ReturnsAsync(order);

            var serviceOrderCompletedStatusHandlerService = new ServiceOrderCompletedStatusHandlerService(hwRepositoryMock.Object, assetMock.Object);


            await serviceOrderCompletedStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.CompletedRepaired, new List<string>() { "IMEI" }, "SERIAL");

            // Verify
            assetMock.Verify(m => m.UpdateAssetLifeCycleStatusAsync(order.AssetLifecycleId, ServiceStatusEnum.CompletedRepaired, new List<string>() { "IMEI" }, "SERIAL"), Times.Once());
        }

        [Fact]
        public async Task HandleCompletedStatus_CompletedDiscarded()
        {
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                          callerId: Guid.NewGuid(),
                          externalId: Guid.NewGuid(),
                          customerId: Guid.NewGuid(),
                          assetLifecycleId: Guid.NewGuid(),
                          userDescription: "[UserDescription]",
                          owner: new ContactDetails(
                            userId: Guid.NewGuid(),
                            firstName: "[FirstName]",
                            lastName: "[LastName]",
                            email: "[Email]",
                            phoneNumber: "[PhoneNumber]"
                          ),
                          deliveryAddress: new DeliveryAddress(
                              recipientType: RecipientTypeEnum.Personal,
                              recipient: "[Recipient]",
                              address1: "[Address1]",
                              address2: "[Address2]",
                              postalCode: "[PostalCode]",
                              city: "[City]",
                              country: "[Country]"),
                          serviceTypeId: (int)ServiceTypeEnum.SUR,
                          statusId: (int)ServiceStatusEnum.CompletedDiscarded,
                          serviceProviderId: (int)ServiceProviderEnum.ConmodoNo,
                          serviceProviderOrderId1: "[ServiceProviderOrderId1]",
                          serviceProviderOrderId2: "[ServiceProviderOrderId2]",
                          externalServiceManagementLink: "[ExternalServiceManagementLink]",
                          serviceEvents: new List<ServiceEvent>()
                      );

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => m.UpdateAssetLifeCycleStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<string?>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                            .ReturnsAsync(order);

            var serviceOrderCompletedStatusHandlerService = new ServiceOrderCompletedStatusHandlerService(hwRepositoryMock.Object, assetMock.Object);


            await serviceOrderCompletedStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.CompletedDiscarded, null, null);

            // Verify
            assetMock.Verify(m => m.UpdateAssetLifeCycleStatusAsync(order.AssetLifecycleId, ServiceStatusEnum.CompletedDiscarded, null, null), Times.Once());
        }

        [Fact]
        public async Task HandleRegisteredStatus()
        {
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                callerId: Guid.NewGuid(),
                externalId: Guid.NewGuid(),
                customerId: Guid.NewGuid(),
                assetLifecycleId: Guid.NewGuid(),
                userDescription: "[UserDescription]",
                owner: new ContactDetails(
                    userId: Guid.NewGuid(),
                    firstName: "[FirstName]",
                    lastName: "[LastName]",
                    email: "[Email]",
                    phoneNumber: "[PhoneNumber]"
                ),
                deliveryAddress: new DeliveryAddress(
                    recipientType: RecipientTypeEnum.Personal,
                    recipient: "[Recipient]",
                    address1: "[Address1]",
                    address2: "[Address2]",
                    postalCode: "[PostalCode]",
                    city: "[City]",
                    country: "[Country]"),
                serviceTypeId: (int)ServiceTypeEnum.SUR,
                statusId: (int)ServiceStatusEnum.Registered,
                serviceProviderId: (int)ServiceProviderEnum.ConmodoNo,
                serviceProviderOrderId1: "[ServiceProviderOrderId1]",
                serviceProviderOrderId2: "[ServiceProviderOrderId2]",
                externalServiceManagementLink: "[ExternalServiceManagementLink]",
                serviceEvents: new List<ServiceEvent>()
            );

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => m.UpdateAssetLifeCycleStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<string?>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                            .ReturnsAsync(order);

            var serviceOrderRegisteredStatusHandlerService = new ServiceOrderRegisteredStatusHandlerService(hwRepositoryMock.Object, assetMock.Object);

            await serviceOrderRegisteredStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.Registered, null, null);

            // Verify
            assetMock.Verify(m => m.UpdateAssetLifeCycleStatusAsync(order.AssetLifecycleId, ServiceStatusEnum.Registered, null, null), Times.Once());
        }
    }
}
