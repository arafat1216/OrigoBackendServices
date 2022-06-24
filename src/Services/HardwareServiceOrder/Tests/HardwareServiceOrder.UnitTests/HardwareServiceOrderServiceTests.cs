using AutoMapper;
using Common.Configuration;
using Common.Utilities;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Mappings;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using HardwareServiceOrderServices.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Xunit;

namespace HardwareServiceOrder.UnitTests
{
    public class HardwareServiceOrderServiceTests : HardwareServiceOrderServiceBaseTests
    {
        private readonly IMapper? _mapper;
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly HardwareServiceOrderContext _dbContext;
        public HardwareServiceOrderServiceTests() : base(new DbContextOptionsBuilder<HardwareServiceOrderContext>()

        .UseSqlite("Data Source=sqlitehardwareserviceorderservicetests.db").Options)
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddMaps(Assembly.GetAssembly(typeof(CustomerSettingsDTO)));
                });
                _mapper = mappingConfig.CreateMapper();
            }

            _dbContext = new HardwareServiceOrderContext(ContextOptions);

            var hardwareServiceRepository = new HardwareServiceOrderRepository(_dbContext);

            //Email service mock
            var emailService = new Mock<IEmailService>();
            emailService.Setup(x => x.SendOrderConfirmationEmailAsync(new HardwareServiceOrderServices.Email.Models.OrderConfirmationEmail(), "en"));

            //Repair provider mock
            var repairProviderMock = new Mock<IRepairProvider>();

            repairProviderMock.Setup(m => m.CreateRepairOrderAsync(It.IsAny<NewExternalRepairOrderDTO>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new NewExternalRepairOrderResponseDTO(serviceProviderOrderId1: "serviceProviderOrderId1", serviceProviderOrderId2: "serviceProviderOrderId2", externalServiceManagementLink: "externalServiceManagementLink"));

            repairProviderMock.Setup(m => m.GetUpdatedRepairOrdersAsync(It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(new List<ExternalRepairOrderDTO> {
                    new ExternalRepairOrderDTO
                    {
                        AssetIsReplaced = false,
                        ExternalServiceEvents = new List<ExternalServiceEventDTO>{ new ExternalServiceEventDTO {  ServiceStatusId = 2, Timestamp = DateTimeOffset.UtcNow} },
                        ProvidedAsset = new AssetInfoDTO(),
                        ReturnedAsset = new AssetInfoDTO(),
                        ServiceProviderOrderId1 = "serviceProviderOrderId1",
                        ServiceProviderOrderId2 = "serviceProviderOrderId2"
                    }
                });

            var providerFactoryMock = new Mock<IProviderFactory>();

            providerFactoryMock.Setup(m => m.GetRepairProviderAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(repairProviderMock.Object);

            //Status handler mock
            var statusHandlMock = new Dictionary<ServiceStatusEnum, ServiceOrderStatusHandlerService>();
            var serviceOrderStatusHandlerServiceMock = new Mock<ServiceOrderStatusHandlerService>();
            serviceOrderStatusHandlerServiceMock.Setup(m => m.UpdateServiceOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>(), It.IsAny<ISet<string>>(), It.IsAny<string>()));
            statusHandlMock.Add(ServiceStatusEnum.Unknown, serviceOrderStatusHandlerServiceMock.Object);

            _hardwareServiceOrderService = new HardwareServiceOrderService(hardwareServiceRepository, _mapper, providerFactoryMock.Object, statusHandlMock, emailService.Object);

        }

        [Fact]
        public async Task ConfigureCustomerSettings()
        {
            var settings = await _hardwareServiceOrderService.ConfigureCustomerSettingsAsync(CUSTOMER_ONE_ID, CALLER_ONE_ID);
            Assert.NotNull(settings);
        }

        [Fact]
        public async Task ConfigureLoanPhone()
        {
            await _hardwareServiceOrderService.ConfigureLoanPhoneAsync(CUSTOMER_ONE_ID, "[+8801724592272]", "[test@test.com]", true, CALLER_ONE_ID);

            var settings = await _hardwareServiceOrderService.GetSettingsAsync(CUSTOMER_ONE_ID);
            Assert.Equal("[+8801724592272]", settings.LoanDevicePhoneNumber);
            Assert.Equal("[test@test.com]", settings.LoanDeviceEmail);
        }

        [Fact]
        public async Task GetOrders_EF_Owned_Does_Not_Need_Include()
        {
            var order = await _dbContext.HardwareServiceOrders.FirstOrDefaultAsync();
            Assert.NotNull(order);
            Assert.NotNull(order!.Owner);
        }

        [Fact]
        public async Task CreateHardwareServiceOrder()
        {
            var serviceOrderDTO = new NewHardwareServiceOrderDTO
            {
                ErrorDescription = "sd",
                OrderedBy = new ContactDetailsExtendedDTO
                {
                    FirstName = "sd",
                    LastName = "sd",
                    UserId = CUSTOMER_ONE_ID,
                    Email = "sds@as.com",
                    PartnerId = new Guid(),
                    PartnerName = "ved",
                    PartnerOrganizationNumber = "23456",
                    OrganizationId = new Guid(),
                    OrganizationName = "AS",
                    OrganizationNumber = "12",
                    PhoneNumber = "23"
                },
                AssetInfo = new AssetInfoDTO( "sd", "dssd", 1, "500119468586675", "500119468586675", new DateOnly(),
            new List<string>
                {
                        "sdsd"
                }),

                DeliveryAddress = new DeliveryAddressDTO
                {
                    Recipient = "fs",
                    Address1 = "f",
                    Address2 = "f",
                    City = "f",
                    Country = "FS",
                    PostalCode = "erg"
                }
            };

            var hardwareServiceOrder = await _hardwareServiceOrderService.CreateHardwareServiceOrderAsync(CUSTOMER_ONE_ID, serviceOrderDTO);

            Assert.NotNull(hardwareServiceOrder);
            Assert.Equal((int)ServiceStatusEnum.Registered, hardwareServiceOrder.StatusId);
            Assert.Equal((int)ServiceTypeEnum.SUR, hardwareServiceOrder.ServiceTypeId);
            Assert.Equal(CUSTOMER_ONE_ID, hardwareServiceOrder.CustomerId);
        }

        [Fact]
        public async Task Get_My_Orders_By_UserId()
        {
            Guid? userId = CUSTOMER_ONE_ID;
            var order = await _hardwareServiceOrderService.GetHardwareServiceOrdersAsync(CUSTOMER_ONE_ID, userId, false, new System.Threading.CancellationToken());
            Assert.NotNull(order);
            Assert.Equal(1, order.Items.Count);
        }

        [Fact]
        public async Task Get_Active_Orders()
        {
            var order = await _hardwareServiceOrderService.GetHardwareServiceOrdersAsync(CUSTOMER_FOUR_ID, null, true, new System.Threading.CancellationToken());
            Assert.NotNull(order);
            Assert.Equal(1, order.Items.Count);
        }

        [Fact]
        public async Task UpdateOrderStatus()
        {
            await _hardwareServiceOrderService.UpdateOrderStatusAsync();

            var orders = _hardwareServiceOrderService.GetHardwareServiceOrdersAsync(CUSTOMER_ONE_ID, null, false, new System.Threading.CancellationToken()).Result.Items;

            Assert.NotNull(orders);

            Assert.Equal(2, orders[0].ServiceEvents.ElementAt(0).ServiceStatusId);
        }

    }
}
