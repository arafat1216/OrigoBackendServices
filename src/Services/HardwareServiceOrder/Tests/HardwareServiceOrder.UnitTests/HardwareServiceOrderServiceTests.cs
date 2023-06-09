﻿using Xunit;
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
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HardwareServiceOrder.UnitTests
{
    public class HardwareServiceOrderServiceTests : HardwareServiceOrderServiceBaseTests
    {
        private readonly IMapper? _mapper;
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;
        private readonly HardwareServiceOrderContext _dbContext;


        public HardwareServiceOrderServiceTests() : base(new DbContextOptionsBuilder<HardwareServiceOrderContext>().UseSqlite("Data Source=sqlitehardwareserviceorderservicetests.db").Options)
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
            _hardwareServiceOrderRepository = new HardwareServiceOrderRepository(_dbContext, new EphemeralDataProtectionProvider());


            #region Email service mock
            var emailService = new Mock<IEmailService>();
            emailService.Setup(x => x.SendOrderConfirmationEmailAsync(new HardwareServiceOrderServices.Email.Models.OrderConfirmationEmail(), "en"));
            #endregion

            #region IGenericProviderOfferings mock
            var genericProviderOfferingsMock = new Mock<IGenericProviderOfferings>();

            genericProviderOfferingsMock.Setup(m => m.GetOrdersUpdatedSinceAsync(It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(new List<ExternalServiceOrderDTO> {
                    new ExternalServiceOrderDTO
                    {
                        AssetIsReplaced = false,
                        ExternalServiceEvents = new List<ExternalServiceEventDTO>
                        {
                            new ExternalServiceEventDTO (ServiceStatusEnum.Canceled, DateTimeOffset.UtcNow)
                        },
                        ProvidedAsset = new AssetInfoDTO("Samsung", "Galaxy S21 Ultra", null, "494018268170953", "S/N: 10000", DateOnly.Parse("2020-01-01"),null),
                        ReturnedAsset = null,
                        ServiceProviderOrderId1 = "serviceProviderOrderId1",
                        ServiceProviderOrderId2 = "serviceProviderOrderId2"
                    },
                    new ExternalServiceOrderDTO
                    {
                        AssetIsReplaced = true,
                        ExternalServiceEvents = new List<ExternalServiceEventDTO>
                        {
                            new ExternalServiceEventDTO (ServiceStatusEnum.CompletedReplaced, DateTimeOffset.UtcNow)
                        },
                        ProvidedAsset = new AssetInfoDTO("Samsung", "Galaxy S21 Ultra", null, "494018268170953", "S/N: 10000", DateOnly.Parse("2020-01-01"),null),
                        ReturnedAsset = new AssetInfoDTO("Samsung", "Galaxy S22 Ultra", "440148139378553", "S/N: 10001"),
                        ServiceProviderOrderId1 = "serviceProviderOrderId1",
                        ServiceProviderOrderId2 = "serviceProviderOrderId2"
                    }
            });
            #endregion


            #region Repair provider mock
            var repairProviderMock = new Mock<IRepairProvider>();

            repairProviderMock.Setup(m => m.CreateRepairOrderAsync(It.IsAny<NewExternalServiceOrderDTO>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new NewExternalServiceOrderResponseDTO(serviceProviderOrderId1: "serviceProviderOrderId1", serviceProviderOrderId2: "serviceProviderOrderId2", externalServiceManagementLink: "externalServiceManagementLink"));

            repairProviderMock.Setup(m => m.GetOrdersUpdatedSinceAsync(It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(new List<ExternalServiceOrderDTO> {
                    new ExternalServiceOrderDTO
                    {
                        AssetIsReplaced = false,
                        ExternalServiceEvents = new List<ExternalServiceEventDTO>
                        {
                            new ExternalServiceEventDTO (ServiceStatusEnum.Canceled, DateTimeOffset.UtcNow)
                        },
                        ProvidedAsset = new AssetInfoDTO("Samsung", "Galaxy S21 Ultra", null, "494018268170953", "S/N: 10000", DateOnly.Parse("2020-01-01"),null),
                        ReturnedAsset = null,
                        ServiceProviderOrderId1 = "serviceProviderOrderId1",
                        ServiceProviderOrderId2 = "serviceProviderOrderId2"
                    },
                    new ExternalServiceOrderDTO
                    {
                        AssetIsReplaced = true,
                        ExternalServiceEvents = new List<ExternalServiceEventDTO>
                        {
                            new ExternalServiceEventDTO (ServiceStatusEnum.CompletedReplaced, DateTimeOffset.UtcNow)
                        },
                        ProvidedAsset = new AssetInfoDTO("Samsung", "Galaxy S21 Ultra", null, "494018268170953", "S/N: 10000", DateOnly.Parse("2020-01-01"),null),
                        ReturnedAsset = new AssetInfoDTO("Samsung", "Galaxy S22 Ultra", "440148139378553", "S/N: 10001"),
                        ServiceProviderOrderId1 = "serviceProviderOrderId1",
                        ServiceProviderOrderId2 = "serviceProviderOrderId2"
                    }
            });

            #endregion

            #region Aftermarket provider mock
            var aftermarketProviderMock = new Mock<IAftermarketProvider>();

            aftermarketProviderMock.Setup(m => m.CreateAftermarketOrderAsync(It.IsAny<NewExternalServiceOrderDTO>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new NewExternalServiceOrderResponseDTO(serviceProviderOrderId1: "serviceProviderOrderId1", serviceProviderOrderId2: "serviceProviderOrderId2", externalServiceManagementLink: "externalServiceManagementLink"));
            #endregion

            #region IProviderFactory mock
            var providerFactoryMock = new Mock<IProviderFactory>();

            providerFactoryMock.Setup(m => m.GetRepairProviderAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(repairProviderMock.Object);

            providerFactoryMock.Setup(m => m.GetAftermarketProviderAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(aftermarketProviderMock.Object);

            #endregion

            #region Status handler mock
            var statusHandlerFactoryMock = new Mock<IStatusHandlerFactory>();

            var serviceOrderStatusHandlerServiceMock = new Mock<ServiceOrderStatusHandlerService>();

            statusHandlerFactoryMock.Setup(m => m.GetStatusHandler(It.IsAny<ServiceTypeEnum>()))
                .Returns(serviceOrderStatusHandlerServiceMock.Object);

            serviceOrderStatusHandlerServiceMock
                .Setup(m =>
                    m.HandleServiceOrderStatusAsync(It.IsAny<HardwareServiceOrderServices.Models.HardwareServiceOrder>(), It.IsAny<ExternalServiceOrderDTO>()));

            #endregion

            _hardwareServiceOrderService = new HardwareServiceOrderService(_hardwareServiceOrderRepository, _mapper, providerFactoryMock.Object, statusHandlerFactoryMock.Object, emailService.Object);
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

        [Theory]
        [InlineData(ServiceTypeEnum.SUR)]
        [InlineData(ServiceTypeEnum.Remarketing)]
        public async Task CreateHardwareServiceOrder(ServiceTypeEnum serviceType)
        {
            ServiceOrderAddon serviceOrderAddon1 = new(500, (int)ServiceProviderEnum.ConmodoNo, "", true, true, Guid.Empty, DateTimeOffset.UtcNow);
            ServiceOrderAddon serviceOrderAddon2 = new(501, (int)ServiceProviderEnum.ConmodoNo, "", false, true, Guid.Empty, DateTimeOffset.UtcNow);
            _dbContext.AddRange(serviceOrderAddon1, serviceOrderAddon2);
            await _dbContext.SaveChangesAsync();

            await _hardwareServiceOrderService.AddServiceOrderAddonsToCustomerServiceProviderAsync(
                CUSTOMER_ONE_ID, (int)ServiceProviderEnum.ConmodoNo, new HashSet<int>() { 500, 501 });


            var serviceOrderDTO = new NewHardwareServiceOrderDTO
            {
                UserDescription = "sd",
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
                },
                ServiceProviderId = (int)ServiceProviderEnum.ConmodoNo,
                ServiceTypeId = (int)serviceType,
                UserSelectedServiceOrderAddonIds = new HashSet<int>() { 500 }
            };

            var hardwareServiceOrder = await _hardwareServiceOrderService.CreateHardwareServiceOrderAsync(CUSTOMER_ONE_ID, serviceOrderDTO);

            Assert.NotNull(hardwareServiceOrder);
            Assert.Equal((int)ServiceStatusEnum.Registered, hardwareServiceOrder.StatusId);
            Assert.Equal((int)serviceType, hardwareServiceOrder.ServiceTypeId);
            Assert.Equal(CUSTOMER_ONE_ID, hardwareServiceOrder.CustomerId);
        }

        [Fact]
        public async Task Get_My_Orders_By_UserId()
        {
            Guid? userId = CUSTOMER_ONE_ID;
            var order = await _hardwareServiceOrderService.GetAllServiceOrdersForOrganizationAsync(CUSTOMER_ONE_ID, userId, null, false, new System.Threading.CancellationToken());
            Assert.NotNull(order);
            Assert.Equal(1, order.Items.Count);
        }

        [Fact]
        public async Task Get_Active_Orders()
        {
            var order = await _hardwareServiceOrderService.GetAllServiceOrdersForOrganizationAsync(CUSTOMER_FOUR_ID, null, null, true, new System.Threading.CancellationToken());
            Assert.NotNull(order);
            Assert.Equal(1, order.Items.Count);
        }

        [Fact]
        public async Task UpdateOrderStatus()
        {
            var customerServiceProviders = await _hardwareServiceOrderRepository.GetCustomerServiceProvidersByFilterAsync(null, true, false, true);

            await _hardwareServiceOrderService.UpdateOrderStatusAsync();

            var updatedCustomerServiceProviders = await _hardwareServiceOrderRepository.GetCustomerServiceProvidersByFilterAsync(null, true, false, true);

            var orders = _hardwareServiceOrderService.GetAllServiceOrdersForOrganizationAsync(CUSTOMER_ONE_ID, null, null, false, new System.Threading.CancellationToken()).Result.Items;

            Assert.NotNull(orders);
            Assert.Null(customerServiceProviders?.FirstOrDefault()?.ApiCredentials?.FirstOrDefault()?.LastUpdateFetched);
            Assert.NotNull(updatedCustomerServiceProviders?.FirstOrDefault()?.ApiCredentials?.FirstOrDefault()?.LastUpdateFetched);

            Assert.Equal((int)ServiceStatusEnum.Canceled, orders[0].ServiceEvents.ElementAt(0).ServiceStatusId);
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
            await _hardwareServiceOrderService.AddServiceOrderAddonsToCustomerServiceProviderAsync(CUSTOMER_ONE_ID, (int)ServiceProviderEnum.ConmodoNo, addonIds.ToHashSet());

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
            Assert.ThrowsAny<Exception>(_hardwareServiceOrderService.AddServiceOrderAddonsToCustomerServiceProviderAsync(CUSTOMER_ONE_ID, (int)ServiceProviderEnum.ConmodoNo, addonIds.ToHashSet()).Wait);
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
            await _hardwareServiceOrderService.AddServiceOrderAddonsToCustomerServiceProviderAsync(CUSTOMER_ONE_ID, (int)ServiceProviderEnum.ConmodoNo, new HashSet<int>() { 500, 501, 502, 503 });

            CustomerServiceProvider? customerServiceProviderPreRemoval = await _dbContext.CustomerServiceProviders
                                                                                         .Include(e => e.ActiveServiceOrderAddons)
                                                                                         .FirstOrDefaultAsync(e => e.CustomerId == CUSTOMER_ONE_ID && e.ServiceProviderId == (int)ServiceProviderEnum.ConmodoNo);

            int originalNumberOfAddons = customerServiceProviderPreRemoval!.ActiveServiceOrderAddons!.Count;

            // Act

            await _hardwareServiceOrderService.RemoveServiceOrderAddonsFromCustomerServiceProviderAsync(CUSTOMER_ONE_ID, (int)ServiceProviderEnum.ConmodoNo, addonIds.ToHashSet());

            CustomerServiceProvider? customerServiceProviderPostRemoval = await _dbContext.CustomerServiceProviders
                                                                                          .Include(e => e.ActiveServiceOrderAddons)
                                                                                          .FirstOrDefaultAsync(e => e.CustomerId == CUSTOMER_ONE_ID && e.ServiceProviderId == (int)ServiceProviderEnum.ConmodoNo);


            // Assert
            Assert.Equal((originalNumberOfAddons - expectedItemsToBeRemoved), customerServiceProviderPostRemoval!.ActiveServiceOrderAddons!.Count);
        }



        [Fact()]
        public async Task GetHardwareServiceOrderById_NoMatchingId_Test()
        {
            // Arrange

            // Act
            HardwareServiceOrderDTO? result = await _hardwareServiceOrderService.GetServiceOrderByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        
        [Fact()]
        public async Task GetHardwareServiceOrderById_MatchFound_Test()
        {
            // Arrange

            // Since we don't know any IDs, let's grab one that exist
            Guid orderId = await _dbContext.HardwareServiceOrders
                                           .Select(e => e.ExternalId)
                                           .FirstOrDefaultAsync();

            // Act
            HardwareServiceOrderDTO? result = await _hardwareServiceOrderService.GetServiceOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
        }



        [Fact()]
        public async Task AddOrUpdateCustomerSettings_NoExistingItems_Test()
        {
            // Arrange
            CustomerSettingsDTO newDTO = new(Guid.NewGuid(), true, "[Phone]", "[Email]");

            // Act
            var newAdded = await _hardwareServiceOrderService.AddOrUpdateCustomerSettings(newDTO);
            var newRetrieved = await _dbContext.CustomerSettings.FirstOrDefaultAsync(e => e.CustomerId == newDTO.CustomerId);

            // Assert 
            Assert.NotNull(newAdded);
            Assert.NotNull(newRetrieved);

            Assert.Equal(newAdded.ProvidesLoanDevice, newRetrieved.ProvidesLoanDevice);
            Assert.Equal(newAdded.LoanDevicePhoneNumber, newRetrieved.LoanDevicePhoneNumber);
            Assert.Equal(newAdded.LoanDeviceEmail, newRetrieved.LoanDeviceEmail);
        }


        [Fact()]
        public async Task AddOrUpdateCustomerSettings_ExistingItems_Test()
        {
            // Arrange
            CustomerSettingsDTO originalDTO = new(Guid.NewGuid(), false, "[Old Phone]", "[Old Email]");
            CustomerSettingsDTO newDTO = new(originalDTO.CustomerId, true, "[New Phone]", "[New Email]");
            
            var originalAdded = await _hardwareServiceOrderService.AddOrUpdateCustomerSettings(originalDTO);
            var originalRetrieved = await _dbContext.CustomerSettings.FirstOrDefaultAsync(e => e.CustomerId == originalDTO.CustomerId);
            int originalId = originalRetrieved!.Id;

            // Act (the 'originalRetrieved' is tracked by EF, and also gets updated by some of these calls. Make a copy of any required values before referencing them)
            var newAdded = await _hardwareServiceOrderService.AddOrUpdateCustomerSettings(newDTO);
            var newRetrievedList = await _dbContext.CustomerSettings.Where(e => e.CustomerId == newDTO.CustomerId).ToListAsync();
            var newRetrieved = newRetrievedList.FirstOrDefault();

            // Assert 
            Assert.NotNull(newAdded);
            Assert.True(newRetrievedList.Count == 1);
            Assert.NotNull(newRetrieved);
            Assert.Equal(originalId, newRetrieved.Id);

            Assert.NotEqual(originalAdded.ProvidesLoanDevice, newRetrieved.ProvidesLoanDevice);
            Assert.NotEqual(originalAdded.LoanDevicePhoneNumber, newRetrieved.LoanDevicePhoneNumber);
            Assert.NotEqual(originalAdded.LoanDeviceEmail, newRetrieved.LoanDeviceEmail);
        }

        [Fact]
        public void Encrypt_Decrypt_Same_Key_Test()
        {
            var customerId = Guid.NewGuid();
            var text = "TEXT TO ENCRYPT";
            var encryptedText = _hardwareServiceOrderRepository.Encrypt(text, customerId.ToString());
            var decryptedText = _hardwareServiceOrderRepository.Decrypt(encryptedText, customerId.ToString());
            Assert.Equal(text, decryptedText);
        }

        [Fact]
        public void Encrypt_Decrypt_Different_Key_Test()
        {
            var customerId1 = Guid.NewGuid();
            var customerId2 = Guid.NewGuid();
            var text = "TEXT TO ENCRYPT";
            var encryptedText = _hardwareServiceOrderRepository.Encrypt(text, customerId1.ToString());
            Assert.Throws<CryptographicException>(() => _hardwareServiceOrderRepository.Decrypt(encryptedText, customerId2.ToString()));
        }

    }
}
