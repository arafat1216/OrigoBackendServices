using AutoMapper;
using Common.Configuration;
using Common.Utilities;
using HardwareServiceOrder.API.Mappings;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices;
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
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Xunit;

namespace HardwareServiceOrder.UnitTests
{
    public class HardwareServiceOrderServiceTests : HardwareServiceOrderServiceBaseTests
    {
        private readonly IMapper? _mapper;

        private readonly IMapper mapper;
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly HardwareServiceOrderContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly OrigoConfiguration _origoConfiguration;
        private readonly ProviderFactory _providerFactory;
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
            var statusHandlers = new Dictionary<ServiceStatusEnum, ServiceOrderStatusHandlerService>
            {

            };

            var repairPro = new Mock<IRepairProvider>();

            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));
            var emailOptions = Options.Create(new EmailConfiguration
            {
                BaseUrl = "https://origov2dev.mytos.no"
            });
            _origoConfiguration = new OrigoConfiguration
            {
                BaseUrl = "https://origov2dev.mytos.no",
                OrderPath = "/my-business/{0}/hardware-repair/{1}/view"
            };
            var origoOptions = Options.Create(_origoConfiguration);
            var flatDictionary = new FlatDictionary();


            var _mailmapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new EmailProfile());
            }).CreateMapper();

            _emailService = new EmailService(emailOptions, flatDictionary, resourceManger, _mailmapper, origoOptions, _dbContext);



            var repairProviderMock = new Mock<IRepairProvider>();
            repairProviderMock.Setup(m => m.CreateRepairOrderAsync(It.IsAny<NewExternalRepairOrderDTO>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new NewExternalRepairOrderResponseDTO(serviceProviderOrderId1: "serviceProviderOrderId1", serviceProviderOrderId2: "serviceProviderOrderId2", externalServiceManagementLink: "externalServiceManagementLink"));
            var providerFactoryMock = new Mock<IProviderFactory>();

            providerFactoryMock.Setup(m => m.GetRepairProviderAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(repairProviderMock.Object);

            _hardwareServiceOrderService = new HardwareServiceOrderService(hardwareServiceRepository, _mapper, providerFactoryMock.Object, statusHandlers, _emailService);

        }

        [Fact]
        public async Task ConfigureServiceId()
        {
            var dto = new CustomerSettingsDTO
            {
                ApiUserName = "[ServiceId]",
                AssetCategoryIds = new System.Collections.Generic.List<int> { 1, 2 },
                ProviderId = 1
            };
            var settings = await _hardwareServiceOrderService.ConfigureServiceIdAsync(CUSTOMER_ONE_ID, dto, CALLER_ONE_ID);
            Assert.Equal(dto.ApiUserName, settings.ApiUserName);
        }

        [Fact]
        public async Task ConfigureLoanPhone()
        {
            await _hardwareServiceOrderService.ConfigureLoanPhoneAsync(CUSTOMER_ONE_ID, "[+8801724592272]", "[test@test.com]", CALLER_ONE_ID);

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
        public async Task MapHardwareViewModelToDto()
        {
            var serviceOrder = new API.ViewModels.NewHardwareServiceOrder
            {
                ErrorDescription = "sd",
                OrderedBy = new API.ViewModels.ContactDetails
                {
                    FirstName = "sd",
                    LastName = "sd",
                    Id = new Guid(),
                    Email = "sds@as.com",
                    PartnerId = new Guid(),
                    PartnerName = "ved",
                    PartnerOrganizationNumber = "23456",
                    OrganizationId = new Guid(),
                    OrganizationName = "AS",
                    OrganizationNumber = "12",
                    PhoneNumber = "23"
                },
                AssetInfo = new API.ViewModels.AssetInfo
                {
                    Imei = "500119468586675",
                    //AssetLifecycleId = new Guid(),
                    Accessories = new List<string>
                    {
                        "sdsd"
                    },
                    AssetCategoryId = 3,
                    Model = "wwe",
                    Brand = "wewe",
                    PurchaseDate = new DateOnly(),
                    SerialNumber = "wewew"
                },
                DeliveryAddress = new API.ViewModels.DeliveryAddress
                {
                    Recipient = "fs",
                    Address1 = "f",
                    Address2 = "f",
                    City = "f",
                    Country = "FS",
                    PostalCode = "erg"
                }
            };

            ////TODO: Fixed by salah later
            //var serviceOrderDTO = mapper.Map<API.ViewModels.NewHardwareServiceOrder, HardwareServiceOrderDTO>(serviceOrder);


            //Assert.NotNull(serviceOrderDTO);

            //Assert.Equal(serviceOrder.AssetInfo.Imei, serviceOrderDTO.AssetInfo.Imei);
            //Assert.Equal(serviceOrder.AssetInfo.Model, serviceOrderDTO.AssetInfo.Model);
            //Assert.Equal(serviceOrder.AssetInfo.SerialNumber, serviceOrderDTO.AssetInfo.SerialNumber);
            //Assert.Equal(serviceOrder.AssetInfo.Brand, serviceOrderDTO.AssetInfo.Brand);
            //Assert.Equal(serviceOrder.AssetInfo.Accessories, serviceOrderDTO.AssetInfo.Accessories);
            //Assert.Equal(serviceOrder.AssetInfo.AssetCategoryId, serviceOrderDTO.AssetInfo.AssetCategoryId);

            ////Assert.Equal(serviceOrder.AssetInfo.PurchaseDate, serviceOrderDTO.AssetInfo.PurchaseDate);

            //Assert.Equal(serviceOrder.DeliveryAddress.Address2, serviceOrderDTO.DeliveryAddress.Address2);
            //Assert.Equal(serviceOrder.DeliveryAddress.Address1, serviceOrderDTO.DeliveryAddress.Address1);
            //Assert.Equal(serviceOrder.DeliveryAddress.City, serviceOrderDTO.DeliveryAddress.City);
            //Assert.Equal(serviceOrder.DeliveryAddress.Country, serviceOrderDTO.DeliveryAddress.Country);
            //Assert.Equal(serviceOrder.DeliveryAddress.Recipient, serviceOrderDTO.DeliveryAddress.Recipient);


            //Assert.Equal(serviceOrder.OrderedBy.FirstName, serviceOrderDTO.OrderedBy.FirstName);
            //Assert.Equal(serviceOrder.OrderedBy.LastName, serviceOrderDTO.OrderedBy.LastName);
            //Assert.Equal(serviceOrder.OrderedBy.PartnerName, serviceOrderDTO.OrderedBy.PartnerName);
            //Assert.Equal(serviceOrder.OrderedBy.PartnerOrganizationNumber, serviceOrderDTO.OrderedBy.PartnerOrganizationNumber);
            //Assert.Equal(serviceOrder.OrderedBy.PhoneNumber, serviceOrderDTO.OrderedBy.PhoneNumber);
            //Assert.Equal(serviceOrder.OrderedBy.OrganizationName, serviceOrderDTO.OrderedBy.OrganizationName);

            //Assert.Equal(serviceOrder.ErrorDescription, serviceOrderDTO.ErrorDescription);
        }

        [Fact]
        public async Task MapHardwareServiceOrderDTOToNewExternalRepairOrderDTO()
        {
            //TODO: Fix later
            //var serviceOrderDTO = new HardwareServiceOrderDTO
            //{
            //    ErrorDescription = "sd",
            //    FirstName = "sd",
            //        LastName = "sd",
            //        Id = new Guid(),
            //        Email = "sds@as.com",
            //        PartnerId = new Guid(),
            //        PartnerName = "ved",
            //        PartnerOrganizationNumber = "23456",
            //        OrganizationId = new Guid(),
            //        OrganizationName = "AS",
            //        OrganizationNumber = "12",
            //        PhoneNumber = "23",
            //        UserDescription = "eefeef",
            //        BasicDescription = "sdsd",

            //AssetInfo = new AssetInfoDTO("sdh", "sd", "dssd", 3, "500119468586675", "500119468586675", new DateOnly(),
            //new List<string>
            //    {
            //            "sdsd"
            //    }),

            //    DeliveryAddress = new DeliveryAddressDTO
            //    {
            //        Recipient = "fs",
            //        Address1 = "f",
            //        Address2 = "f",
            //        City = "f",
            //        Country = "FS",
            //        PostalCode = "erg"
            //    },
            //    ServiceProvider = new ServiceProvider
            //    {
            //        OrganizationId = new Guid()
            //    },
            //    ServiceStatus = new ServiceStatus
            //    {
            //        Id = 17
            //    },
            //    ServiceType = new ServiceType
            //    {
            //        Id= 5
            //    }
            //};

            //var customerSettingDto = _hardwareServiceOrderService.CreateHardwareServiceOrderAsync(CUSTOMER_ONE_ID,serviceOrderDTO).Result;


            //  Assert.NotNull(customerSettingDto);

            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.Imei, serviceOrderDTO.AssetInfo.Imei);
            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.Model, serviceOrderDTO.AssetInfo.Model);
            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.SerialNumber, serviceOrderDTO.AssetInfo.SerialNumber);
            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.Brand, serviceOrderDTO.AssetInfo.Brand);
            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.Accessories, serviceOrderDTO.AssetInfo.Accessories);
            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.AssetCategoryId, serviceOrderDTO.AssetInfo.AssetCategoryId);

        }

        [Fact]
        public async Task CreateHardwareServiceOrder()
        {
            var dto = new HardwareServiceOrderDTO
            {
                AssetInfo = new AssetInfoDTO("sdh", "sd", "dssd", 3, "500119468586675", "500119468586675", new DateOnly(),
            new List<string>
                {
                        "sdsd"
                })
            };
        }

    }
}
