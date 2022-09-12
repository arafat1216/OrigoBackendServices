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
using HardwareServiceOrderServices.ServiceModels;
using Xunit;

namespace HardwareServiceOrder.UnitTests
{
    /// <summary>
    /// Testing all the service-order status handler related functionalities for service-type "SUR". <see cref="ServiceOrderStatusHandlerServiceForSUR"/> 
    /// </summary>
    public class ServiceOrderStatusHandlerServiceForSURTests
    {
        private readonly IOptions<OrigoConfiguration> _options;

        public ServiceOrderStatusHandlerServiceForSURTests()
        {
            _options = Options.Create(new OrigoConfiguration
            {
                BaseUrl = "https://origov2dev.mytos.no",
                DeveloperEmail = "it@mytos.no",
                OrderPath = "my-business/{0}/hardware-repair/{1}/view"
            });
        }

        private HardwareServiceOrderServices.Models.HardwareServiceOrder CreateHardwareServiceOrder(ServiceStatusEnum statusEnum)
        {
            var hardwareServiceOrder = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                externalId: Guid.NewGuid(),
                customerId: Guid.NewGuid(),
                assetLifecycleId: Guid.NewGuid(),
                assetLifecycleCategoryId: 1,
                assetInfo: new AssetInfo(
                    brand: "[AssetBrand]",
                    model: "[AssetModel]",
                    imei: new HashSet<string>() { "527127734377463" },
                    serialNumber:"[SerialNumber]",
                    purchaseDate: DateOnly.Parse("2020-01-01"),
                    accessories: null
                ),
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
                statusId: (int)statusEnum,
                serviceProviderId: 1,
                null,
                serviceProviderOrderId1: "[ServiceProviderOrderId1]",
                serviceProviderOrderId2: "[ServiceProviderOrderId2]",
                externalServiceManagementLink: "[ExternalServiceManagementLink]",
                serviceEvents: new List<ServiceEvent>()
            );
            return hardwareServiceOrder;
        }

        private ExternalServiceOrderDTO CreateExternalRepairOrderDTO(ServiceStatusEnum statusEnum)
        {
            var externalRepairOrderDto = new ExternalServiceOrderDTO(
                serviceProviderOrderId1: "[ServiceProviderOrderId1]",
                serviceProviderOrderId2: "[ServiceProviderOrderId2]",
                externalServiceEvents: new List<ExternalServiceEventDTO>()
                {
                    new (serviceStatusId: statusEnum, timestamp: DateTimeOffset.Now)
                },
                providedAsset: new AssetInfoDTO(
                    brand: "[Brand]",
                    model: "[Model]",
                    imei: "[Imei]",
                    serialNumber: "[SerialNumber]",
                    accessories: new List<string>()
                ),
                returnedAsset: new AssetInfoDTO(
                    brand: "[Brand]",
                    model: "[Model]",
                    imei: "[Imei]",
                    serialNumber: "[SerialNumber]",
                    accessories: new List<string>()
                ),
                assetIsReplaced: false
            );
            return externalRepairOrderDto;
        }

        [Theory]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.Canceled)]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.CompletedNotRepaired)]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.CompletedRepaired)]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.CompletedRepairedOnWarranty)]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.CompletedCredited)]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.CompletedDiscarded)]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.Ongoing)]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.OngoingUserActionNeeded)]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.OngoingInTransit)]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.OngoingReadyForPickup)]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.RegisteredInTransit)]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.RegisteredUserActionNeeded)]
        public async Task UpdateOrderStatus_WHEN_VALID_STATUS_UPDATED(ServiceStatusEnum currentStatus, ServiceStatusEnum newStatus)
        {
            //Arrange
            var hardwareServiceOrder = CreateHardwareServiceOrder(currentStatus);
            var externalRepairOrder = CreateExternalRepairOrderDTO(newStatus);

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock
                .Setup(m => m.UpdateServiceOrderStatusAsync(hardwareServiceOrder.ExternalId, newStatus))
                .ReturnsAsync(hardwareServiceOrder);

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => 
                m.UpdateAssetLifeCycleStatusForSURAsync(
                    hardwareServiceOrder.AssetLifecycleId, 
                    newStatus, 
                    new HashSet<string>() { externalRepairOrder.ReturnedAsset.Imei }, 
                    externalRepairOrder.ReturnedAsset.SerialNumber));

            var emailMock = new Mock<IEmailService>();
            emailMock.Setup(m => 
                m.SendEmailAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<Dictionary<string, string>>(), 
                    It.IsAny<string>()));
            var parameters = new Dictionary<string, string>
            {
                { "Order", JsonSerializer.Serialize(hardwareServiceOrder) },
                { "OrderLink", string.Format($"{_options.Value.BaseUrl}/{_options.Value.OrderPath}", hardwareServiceOrder.CustomerId, hardwareServiceOrder.ExternalId) }
            };

            var sut = new ServiceOrderStatusHandlerServiceForSUR(options: _options, hwRepositoryMock.Object, assetMock.Object, emailMock.Object);

            // ACT
            await sut.HandleServiceOrderStatusAsync(hardwareServiceOrder, externalRepairOrder);

            // Assert/Verify
            hwRepositoryMock.Verify(m => 
                    m.UpdateServiceOrderStatusAsync(hardwareServiceOrder.ExternalId, newStatus), 
                Times.Once);

            assetMock.Verify(m => 
                    m.UpdateAssetLifeCycleStatusForSURAsync(
                        hardwareServiceOrder.AssetLifecycleId, 
                        newStatus,
                        new HashSet<string>() { externalRepairOrder.ReturnedAsset.Imei }, 
                        externalRepairOrder.ReturnedAsset.SerialNumber), 
                Times.Once);
            
            emailMock.Verify(m => 
                    m.SendEmailAsync(
                        _options.Value.DeveloperEmail, 
                        $"{ServiceStatusEnum.Unknown}_Subject", 
                        $"{ServiceStatusEnum.Unknown}_Body", 
                        parameters, 
                        It.IsAny<string>()), 
                Times.Never);
        }

        [Theory]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.Registered)]
        [InlineData(ServiceStatusEnum.Ongoing, ServiceStatusEnum.Ongoing)]
        [InlineData(ServiceStatusEnum.Canceled, ServiceStatusEnum.Canceled)]
        public async Task UpdateOrderStatus_WHEN_VALID_STATUS_NOT_UPDATED(ServiceStatusEnum currentStatus, ServiceStatusEnum newStatus)
        {
            //Arrange
            var hardwareServiceOrder = CreateHardwareServiceOrder(currentStatus);
            var externalRepairOrder = CreateExternalRepairOrderDTO(newStatus);

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock
                .Setup(m => m.UpdateServiceOrderStatusAsync(hardwareServiceOrder.ExternalId, newStatus))
                .ReturnsAsync(hardwareServiceOrder);

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => 
                m.UpdateAssetLifeCycleStatusForSURAsync(
                    hardwareServiceOrder.AssetLifecycleId, 
                    newStatus, 
                    new HashSet<string>() { externalRepairOrder.ReturnedAsset.Imei }, 
                    externalRepairOrder.ReturnedAsset.SerialNumber));

            var emailMock = new Mock<IEmailService>();
            emailMock.Setup(m => 
                m.SendEmailAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<Dictionary<string, string>>(), 
                    It.IsAny<string>()));
            var parameters = new Dictionary<string, string>
            {
                { "Order", JsonSerializer.Serialize(hardwareServiceOrder) },
                { "OrderLink", string.Format($"{_options.Value.BaseUrl}/{_options.Value.OrderPath}", hardwareServiceOrder.CustomerId, hardwareServiceOrder.ExternalId) }
            };

            var sut = new ServiceOrderStatusHandlerServiceForSUR(options: _options, hwRepositoryMock.Object, assetMock.Object, emailMock.Object);

            // ACT
            await sut.HandleServiceOrderStatusAsync(hardwareServiceOrder, externalRepairOrder);

            // Assert/Verify
            hwRepositoryMock.Verify(m => 
                    m.UpdateServiceOrderStatusAsync(hardwareServiceOrder.ExternalId, newStatus), 
                Times.Never);

            assetMock.Verify(m => 
                    m.UpdateAssetLifeCycleStatusForSURAsync(
                        hardwareServiceOrder.AssetLifecycleId, 
                        newStatus,
                        new HashSet<string>() { externalRepairOrder.ReturnedAsset.Imei }, 
                        externalRepairOrder.ReturnedAsset.SerialNumber), 
                Times.Never);
            
            emailMock.Verify(m => 
                    m.SendEmailAsync(
                        _options.Value.DeveloperEmail, 
                        $"{ServiceStatusEnum.Unknown}_Subject", 
                        $"{ServiceStatusEnum.Unknown}_Body", 
                        parameters, 
                        It.IsAny<string>()), 
                Times.Never);
        }

        [Theory]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.Unknown)]
        public async Task UpdateOrderStatus_FOR_UNKNOWN_STATUS(ServiceStatusEnum currentStatus, ServiceStatusEnum newStatus)
        {
            //Arrange
            var hardwareServiceOrder = CreateHardwareServiceOrder(currentStatus);
            var externalRepairOrder = CreateExternalRepairOrderDTO(newStatus);

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock
                .Setup(m => m.UpdateServiceOrderStatusAsync(hardwareServiceOrder.ExternalId, newStatus))
                .ReturnsAsync(hardwareServiceOrder);

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => 
                m.UpdateAssetLifeCycleStatusForSURAsync(
                    hardwareServiceOrder.AssetLifecycleId, 
                    newStatus, 
                    new HashSet<string>() { externalRepairOrder.ReturnedAsset.Imei }, 
                    externalRepairOrder.ReturnedAsset.SerialNumber));

            var emailMock = new Mock<IEmailService>();
            emailMock.Setup(m => 
                m.SendEmailAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<Dictionary<string, string>>(), 
                    It.IsAny<string>()));
            var parameters = new Dictionary<string, string>
            {
                { "Order", JsonSerializer.Serialize(hardwareServiceOrder) },
                { "OrderLink", string.Format($"{_options.Value.BaseUrl}/{_options.Value.OrderPath}", hardwareServiceOrder.CustomerId, hardwareServiceOrder.ExternalId) }
            };

            var sut = new ServiceOrderStatusHandlerServiceForSUR(options: _options, hwRepositoryMock.Object, assetMock.Object, emailMock.Object);

            // ACT
            await sut.HandleServiceOrderStatusAsync(hardwareServiceOrder, externalRepairOrder);
            
            // Assert/Verify
            hwRepositoryMock.Verify(m => 
                    m.UpdateServiceOrderStatusAsync(hardwareServiceOrder.ExternalId, newStatus), 
                Times.Once);

            assetMock.Verify(m => 
                    m.UpdateAssetLifeCycleStatusForSURAsync(
                        hardwareServiceOrder.AssetLifecycleId, 
                        newStatus,
                        new HashSet<string>() { externalRepairOrder.ReturnedAsset.Imei }, 
                        externalRepairOrder.ReturnedAsset.SerialNumber), 
                Times.Never);
            
            emailMock.Verify(m => 
                m.SendEmailAsync(
                    _options.Value.DeveloperEmail, 
                    $"{ServiceStatusEnum.Unknown}_Subject", 
                    $"{ServiceStatusEnum.Unknown}_Body", 
                    parameters, 
                    It.IsAny<string>()), 
                Times.Once);
        }
        
        [Theory]
        [InlineData(ServiceStatusEnum.Registered, ServiceStatusEnum.Null)]
        [InlineData(ServiceStatusEnum.Registered, (ServiceStatusEnum)100)]
        public async Task UpdateOrderStatus_FOR_INVALID_STATUS(ServiceStatusEnum currentStatus, ServiceStatusEnum newStatus)
        {
            //Arrange
            var hardwareServiceOrder = CreateHardwareServiceOrder(currentStatus);
            var externalRepairOrder = CreateExternalRepairOrderDTO(newStatus);

            var hwRepositoryMock = new Mock<IHardwareServiceOrderRepository>();
            hwRepositoryMock
                .Setup(m => m.UpdateServiceOrderStatusAsync(hardwareServiceOrder.ExternalId, newStatus))
                .ReturnsAsync(hardwareServiceOrder);

            var assetMock = new Mock<IAssetService>();
            assetMock.Setup(m => 
                m.UpdateAssetLifeCycleStatusForSURAsync(
                    hardwareServiceOrder.AssetLifecycleId, 
                    newStatus, 
                    new HashSet<string>() { externalRepairOrder.ReturnedAsset.Imei }, 
                    externalRepairOrder.ReturnedAsset.SerialNumber));

            var emailMock = new Mock<IEmailService>();
            emailMock.Setup(m => 
                m.SendEmailAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<Dictionary<string, string>>(), 
                    It.IsAny<string>()));

            var sut = new ServiceOrderStatusHandlerServiceForSUR(options: _options, hwRepositoryMock.Object, assetMock.Object, emailMock.Object);
            
            // ACT
            Func<Task> act = () => sut.HandleServiceOrderStatusAsync(hardwareServiceOrder, externalRepairOrder);

            // ASSERT
            await Assert.ThrowsAsync<ArgumentException>(act);
        }

    }
}
