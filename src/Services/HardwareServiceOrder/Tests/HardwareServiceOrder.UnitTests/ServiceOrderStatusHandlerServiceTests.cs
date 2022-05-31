using Common.Enums;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.Services;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    customerId: Guid.NewGuid(), new User(externalId: Guid.NewGuid(), name: "[Name]", email: "[Email]"),
                    assetLifecycleId: Guid.NewGuid(), assetName: "[assetName]",
                    deliveryAddress: new DeliveryAddress(RecipientTypeEnum.Personal, recipient: "[Recipient]", address1: "[Address1]", address2: "[Address2]", postalCode: "[PostalCode]", city: "[City]", country: "[Country]"),
                    userDescription: "[UserDescription]", serviceProvider: new ServiceProvider(), serviceProviderOrderId1: "[serviceProviderOrderId1]", serviceProviderOrderId2: "[serviceProviderOrderId2]",
                    externalServiceManagementLink: "[externalServiceManagementLink]", new ServiceType(),
                    new ServiceStatus { Id = (int)ServiceStatusEnum.Unknown });

            var emailMock = new Mock<IEmailService>();
            emailMock.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                .ReturnsAsync(order);

            var serviceOrderUnKnownStatusHandlerService = new ServiceOrderUnKnownStatusHandlerService(options: _options, emailService: emailMock.Object, hardwareServiceOrderRepository: hwRepositoryMock.Object);


            await serviceOrderUnKnownStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.Unknown);

            // Verify
            var parameters = new Dictionary<string, string>
            {
                { "Order", JsonSerializer.Serialize(order) },
                {"OrderLink", string.Format($"{_options.Value.BaseUrl}/{_options.Value.OrderPath}", order.CustomerId, order.ExternalId)}
            };

            emailMock.Verify(m => m.SendEmailAsync(_options.Value.DeveloperEmail, $"{ServiceStatusEnum.Unknown}_Subject", $"{ServiceStatusEnum.Unknown}_Body", parameters, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleCanceledStatus()
        {
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                    customerId: Guid.NewGuid(), new User(externalId: Guid.NewGuid(), name: "[Name]", email: "[Email]"),
                    assetLifecycleId: Guid.NewGuid(), assetName: "[assetName]",
                    deliveryAddress: new DeliveryAddress(RecipientTypeEnum.Personal, recipient: "[Recipient]", address1: "[Address1]", address2: "[Address2]", postalCode: "[PostalCode]", city: "[City]", country: "[Country]"),
                    userDescription: "[UserDescription]", serviceProvider: new ServiceProvider(), serviceProviderOrderId1: "[serviceProviderOrderId1]", serviceProviderOrderId2: "[serviceProviderOrderId2]",
                    externalServiceManagementLink: "[externalServiceManagementLink]", new ServiceType(),
                    new ServiceStatus { Id = (int)ServiceStatusEnum.Canceled });

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => m.UpdateAssetLifeCycleStatusAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<AssetLifecycleStatus>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                .ReturnsAsync(order);

            var serviceOrderCancelledStatusHandlerService = new ServiceOrderCanceledStatusHandlerService(hwRepositoryMock.Object, assetMock.Object);


            await serviceOrderCancelledStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.Canceled);

            // Verify
            assetMock.Verify(m => m.UpdateAssetLifeCycleStatusAsync(order.CustomerId, order.AssetLifecycleId, Common.Enums.AssetLifecycleStatus.InUse), Times.Once());
        }

        [Fact]
        public async Task HandleOngoingStatus()
        {
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                    customerId: Guid.NewGuid(), new User(externalId: Guid.NewGuid(), name: "[Name]", email: "[Email]"),
                    assetLifecycleId: Guid.NewGuid(), assetName: "[assetName]",
                    deliveryAddress: new DeliveryAddress(RecipientTypeEnum.Personal, recipient: "[Recipient]", address1: "[Address1]", address2: "[Address2]", postalCode: "[PostalCode]", city: "[City]", country: "[Country]"),
                    userDescription: "[UserDescription]", serviceProvider: new ServiceProvider(), serviceProviderOrderId1: "[serviceProviderOrderId1]", serviceProviderOrderId2: "[serviceProviderOrderId2]",
                    externalServiceManagementLink: "[externalServiceManagementLink]", new ServiceType(),
                    new ServiceStatus { Id = (int)ServiceStatusEnum.Canceled });

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => m.UpdateAssetLifeCycleStatusAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<AssetLifecycleStatus>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                .ReturnsAsync(order);

            var serviceOrderOngoingStatusHandlerService = new ServiceOrderOngoingStatusHandlerService(hwRepositoryMock.Object, assetMock.Object);


            await serviceOrderOngoingStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.Ongoing);

            // Verify
            assetMock.Verify(m => m.UpdateAssetLifeCycleStatusAsync(order.CustomerId, order.AssetLifecycleId, Common.Enums.AssetLifecycleStatus.Repair), Times.Once());
        }

        [Fact]
        public async Task HandleCompletedStatus_CompletedRepaired()
        {
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                    customerId: Guid.NewGuid(), new User(externalId: Guid.NewGuid(), name: "[Name]", email: "[Email]"),
                    assetLifecycleId: Guid.NewGuid(), assetName: "[assetName]",
                    deliveryAddress: new DeliveryAddress(RecipientTypeEnum.Personal, recipient: "[Recipient]", address1: "[Address1]", address2: "[Address2]", postalCode: "[PostalCode]", city: "[City]", country: "[Country]"),
                    userDescription: "[UserDescription]", serviceProvider: new ServiceProvider(), serviceProviderOrderId1: "[serviceProviderOrderId1]", serviceProviderOrderId2: "[serviceProviderOrderId2]",
                    externalServiceManagementLink: "[externalServiceManagementLink]", new ServiceType(),
                    new ServiceStatus { Id = (int)ServiceStatusEnum.Canceled });

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => m.UpdateAssetLifeCycleStatusAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<AssetLifecycleStatus>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                .ReturnsAsync(order);

            var serviceOrderCompletedStatusHandlerService = new ServiceOrderCompletedStatusHandlerService(hwRepositoryMock.Object, assetMock.Object);


            await serviceOrderCompletedStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.CompletedRepaired);

            // Verify
            assetMock.Verify(m => m.UpdateAssetLifeCycleStatusAsync(order.CustomerId, order.AssetLifecycleId, Common.Enums.AssetLifecycleStatus.InUse), Times.Once());
        }

        [Fact]
        public async Task HandleCompletedStatus_CompletedDiscarded()
        {
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                    customerId: Guid.NewGuid(), new User(externalId: Guid.NewGuid(), name: "[Name]", email: "[Email]"),
                    assetLifecycleId: Guid.NewGuid(), assetName: "[assetName]",
                    deliveryAddress: new DeliveryAddress(RecipientTypeEnum.Personal, recipient: "[Recipient]", address1: "[Address1]", address2: "[Address2]", postalCode: "[PostalCode]", city: "[City]", country: "[Country]"),
                    userDescription: "[UserDescription]", serviceProvider: new ServiceProvider(), serviceProviderOrderId1: "[serviceProviderOrderId1]", serviceProviderOrderId2: "[serviceProviderOrderId2]",
                    externalServiceManagementLink: "[externalServiceManagementLink]", new ServiceType(),
                    new ServiceStatus { Id = (int)ServiceStatusEnum.Canceled });

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => m.UpdateAssetLifeCycleStatusAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<AssetLifecycleStatus>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                .ReturnsAsync(order);

            var serviceOrderCompletedStatusHandlerService = new ServiceOrderCompletedStatusHandlerService(hwRepositoryMock.Object, assetMock.Object);


            await serviceOrderCompletedStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.CompletedDiscarded);

            // Verify
            assetMock.Verify(m => m.UpdateAssetLifeCycleStatusAsync(order.CustomerId, order.AssetLifecycleId, Common.Enums.AssetLifecycleStatus.Discarded), Times.Once());
        }

        [Fact]
        public async Task HandleRegisteredStatus()
        {
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                    customerId: Guid.NewGuid(), new User(externalId: Guid.NewGuid(), name: "[Name]", email: "[Email]"),
                    assetLifecycleId: Guid.NewGuid(), assetName: "[assetName]",
                    deliveryAddress: new DeliveryAddress(RecipientTypeEnum.Personal, recipient: "[Recipient]", address1: "[Address1]", address2: "[Address2]", postalCode: "[PostalCode]", city: "[City]", country: "[Country]"),
                    userDescription: "[UserDescription]", serviceProvider: new ServiceProvider(), serviceProviderOrderId1: "[serviceProviderOrderId1]", serviceProviderOrderId2: "[serviceProviderOrderId2]",
                    externalServiceManagementLink: "[externalServiceManagementLink]", new ServiceType(),
                    new ServiceStatus { Id = (int)ServiceStatusEnum.Canceled });

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => m.UpdateAssetLifeCycleStatusAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<AssetLifecycleStatus>()));

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock.Setup(m => m.UpdateOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>()))
                .ReturnsAsync(order);

            var serviceOrderRegisteredStatusHandlerService = new ServiceOrderRegisteredStatusHandlerService(hwRepositoryMock.Object, assetMock.Object);

            await serviceOrderRegisteredStatusHandlerService.UpdateServiceOrderStatusAsync(orderId: Guid.NewGuid(), ServiceStatusEnum.Registered);

            // Verify
            assetMock.Verify(m => m.UpdateAssetLifeCycleStatusAsync(order.CustomerId, order.AssetLifecycleId, Common.Enums.AssetLifecycleStatus.Repair), Times.Once());
        }
    }
}
