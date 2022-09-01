using Xunit;
using AutoMapper;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.ServiceModels;
using HardwareServiceOrderServices.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HardwareServiceOrder.UnitTests
{
    public class HardwareServiceOrderServiceTests : HardwareServiceOrderServiceBaseTests
    {
        private readonly IMapper? _mapper;
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly HardwareServiceOrderContext _dbContext;
        public HardwareServiceOrderServiceTests() :
            base(new DbContextOptionsBuilder<HardwareServiceOrderContext>()
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

            // Status handler mock
            var statusHandlerFactoryMock = new Mock<IStatusHandlerFactory>();

            var serviceOrderStatusHandlerServiceMock = new Mock<ServiceOrderStatusHandlerService>();

            statusHandlerFactoryMock.Setup(m => m.GetStatusHandler(It.IsAny<ServiceTypeEnum>()))
                .Returns(serviceOrderStatusHandlerServiceMock.Object);

            serviceOrderStatusHandlerServiceMock
                .Setup(m =>
                    m.HandleServiceOrderStatusAsync(It.IsAny<HardwareServiceOrderServices.Models.HardwareServiceOrder>(), It.IsAny<ExternalRepairOrderDTO>()));

            _hardwareServiceOrderService = new HardwareServiceOrderService(hardwareServiceRepository, _mapper, providerFactoryMock.Object, statusHandlerFactoryMock.Object, emailService.Object, new EphemeralDataProtectionProvider());

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
                AssetInfo = new AssetInfoDTO("sd", "dssd", 1, "500119468586675", "500119468586675", new DateOnly(),
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

        [Fact]
        public async Task ConfigureCustomerServiceProvider()
        {
            await _hardwareServiceOrderService.ConfigureCustomerServiceProviderAsync(providerId: 1, customerId: CUSTOMER_ONE_ID, apiUsername: "123456", apiPassword: "password123");

            var apiUserName = await _hardwareServiceOrderService.GetServicerProvidersUsernameAsync(CUSTOMER_ONE_ID, 1);

            Assert.Equal("123456", apiUserName);
        }



        [Theory()]
        [InlineData(0)]
        [InlineData(2, 500, 501)] // Two plain entries
        [InlineData(2, 500, 501, 501)] // Duplicates should not be added.
        public async Task AddServiceOrderAddonsToCustomerServiceProviderTest(int expectedAddonItems, params int[] addonIds)
        {
            // Arrange

            // Add new service-order addons to existing providers
            ServiceOrderAddon serviceOrderAddon1 = new(500, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon1);
            ServiceOrderAddon serviceOrderAddon2 = new(501, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon2);
            ServiceOrderAddon serviceOrderAddon3 = new(502, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon3);
            ServiceOrderAddon serviceOrderAddon4 = new(503, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon4);

            await _dbContext.SaveChangesAsync();

            // Act
            await _hardwareServiceOrderService.AddServiceOrderAddonsToCustomerServiceProvider(CUSTOMER_ONE_ID, (int)ServiceProviderEnum.ConmodoNo, addonIds.ToHashSet());

            CustomerServiceProvider? customerServiceProvider = await _dbContext.CustomerServiceProviders
                                                                               .Include(e => e.ActiveServiceOrderAddons)
                                                                               .FirstOrDefaultAsync(e => e.CustomerId == CUSTOMER_ONE_ID && e.ServiceProviderId == (int)ServiceProviderEnum.ConmodoNo);

            // Assert
            Assert.NotNull(customerServiceProvider);
            Assert.Equal(expectedAddonItems, customerServiceProvider!.ActiveServiceOrderAddons!.Count);
        }

        [Theory()]
        [InlineData(500, 504)] // 504 belongs to another service-provider, and should not be allowed
        [InlineData(500, 501, 999)] // One of the IDs don't exist, and should not be allowed
        public async Task AddServiceOrderAddonsToCustomerServiceProvider_Exceptions_Test(params int[] addonIds)
        {
            /*
             * Arrange
             */

            // Add new service-order addons to existing providers
            ServiceOrderAddon serviceOrderAddon1 = new(500, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon1);
            ServiceOrderAddon serviceOrderAddon2 = new(501, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon2);
            ServiceOrderAddon serviceOrderAddon3 = new(502, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon3);
            ServiceOrderAddon serviceOrderAddon4 = new(503, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon4);

            // Add a new provider, so we can check agains not-valid combinations.
            List<ServiceOrderAddon> serviceOrderAddons = new()
            {
                 new(504, 600, "", true, true, Guid.Empty, DateTimeOffset.UtcNow)
            };
            ServiceProvider serviceProvider = new(600, "Test", Guid.NewGuid(), false, false, null, serviceOrderAddons, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceProvider);

            await _dbContext.SaveChangesAsync();

            // Act


            // Assert
            Assert.ThrowsAny<Exception>(_hardwareServiceOrderService.AddServiceOrderAddonsToCustomerServiceProvider(CUSTOMER_ONE_ID, (int)ServiceProviderEnum.ConmodoNo, addonIds.ToHashSet()).Wait);
        }

        [Theory()]
        [InlineData(2, 500, 501)]
        [InlineData(3, 500, 501, 502)]
        [InlineData(1, 500, 504)] // 504 is not valid, and should not be removed.
        public async Task RemoveServiceOrderAddonsFromCustomerServiceProviderTest(int expectedItemsToBeRemoved, params int[] addonIds)
        {
            // Arrange

            // Add new service-order addons to existing providers
            ServiceOrderAddon serviceOrderAddon1 = new(500, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon1);
            ServiceOrderAddon serviceOrderAddon2 = new(501, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon2);
            ServiceOrderAddon serviceOrderAddon3 = new(502, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon3);
            ServiceOrderAddon serviceOrderAddon4 = new(503, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.Add(serviceOrderAddon4);

            await _dbContext.SaveChangesAsync();
            await _hardwareServiceOrderService.AddServiceOrderAddonsToCustomerServiceProvider(CUSTOMER_ONE_ID, (int)ServiceProviderEnum.ConmodoNo, new HashSet<int>() { 500, 501, 502, 503 });

            CustomerServiceProvider? customerServiceProviderPreRemoval = await _dbContext.CustomerServiceProviders
                                                                                         .Include(e => e.ActiveServiceOrderAddons)
                                                                                         .FirstOrDefaultAsync(e => e.CustomerId == CUSTOMER_ONE_ID && e.ServiceProviderId == (int)ServiceProviderEnum.ConmodoNo);

            int originalNumberOfAddons = customerServiceProviderPreRemoval!.ActiveServiceOrderAddons!.Count;

            // Act

            await _hardwareServiceOrderService.RemoveServiceOrderAddonsFromCustomerServiceProvider(CUSTOMER_ONE_ID, (int)ServiceProviderEnum.ConmodoNo, addonIds.ToHashSet());

            CustomerServiceProvider? customerServiceProviderPostRemoval = await _dbContext.CustomerServiceProviders
                                                                                          .Include(e => e.ActiveServiceOrderAddons)
                                                                                          .FirstOrDefaultAsync(e => e.CustomerId == CUSTOMER_ONE_ID && e.ServiceProviderId == (int)ServiceProviderEnum.ConmodoNo);


            // Assert
            Assert.Equal((originalNumberOfAddons - expectedItemsToBeRemoved), customerServiceProviderPostRemoval!.ActiveServiceOrderAddons!.Count);
        }

    }
}
