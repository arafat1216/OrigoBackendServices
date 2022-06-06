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

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Location, DeliveryAddressDTO>();
                cfg.CreateMap<AssetInfo, AssetInfoDTO>();

                cfg.CreateMap<API.ViewModels.HardwareServiceOrder, HardwareServiceOrderDTO>()
                    .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress))
                    .ForMember(m => m.AssetInfo, opts => opts.MapFrom(s => s.AssetInfo))
                .ForMember(m => m.FirstName, opts => opts.MapFrom(m => m.OrderedBy.FistName))
                .ForMember(m => m.LastName, opts => opts.MapFrom(m => m.OrderedBy.LastName))
                .ForMember(m => m.PartnerName, opts => opts.MapFrom(m => m.OrderedBy.PartnerName))
                .ForMember(m => m.PartnerId, opts => opts.MapFrom(m => m.OrderedBy.PartnerId))
                .ForMember(m => m.PartnerOrganizationNumber, opts => opts.MapFrom(m => m.OrderedBy.PartnerOrganizationNumber))
                .ForMember(m => m.OrganizationId, opts => opts.MapFrom(m => m.OrderedBy.OrganizationId))
                .ForMember(m => m.OrganizationName, opts => opts.MapFrom(m => m.OrderedBy.OrganizationName))
                .ForMember(m => m.OrganizationNumber, opts => opts.MapFrom(m => m.OrderedBy.OrganizationNumber))
                .ForMember(m => m.PhoneNumber, opts => opts.MapFrom(m => m.OrderedBy.PhoneNumber))
                .ForMember(m => m.Id, opts => opts.MapFrom(m => m.OrderedBy.Id))
                .ForMember(m => m.Email, opts => opts.MapFrom(m => m.OrderedBy.Email));

                cfg.CreateMap<HardwareServiceOrderDTO, NewExternalRepairOrderDTO>()
                    .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress))
                    .ForMember(m => m.AssetInfo, opts => opts.MapFrom(s => s.AssetInfo))
                .ForMember(m => m.FirstName, opts => opts.MapFrom(m => m.FirstName))
                .ForMember(m => m.LastName, opts => opts.MapFrom(m => m.LastName))
                .ForMember(m => m.PartnerName, opts => opts.MapFrom(m => m.PartnerName))
                .ForMember(m => m.PartnerId, opts => opts.MapFrom(m => m.PartnerId))
                .ForMember(m => m.PartnerOrganizationNumber, opts => opts.MapFrom(m => m.PartnerOrganizationNumber))
                .ForMember(m => m.OrganizationId, opts => opts.MapFrom(m => m.OrganizationId))
                .ForMember(m => m.OrganizationName, opts => opts.MapFrom(m => m.OrganizationName))
                .ForMember(m => m.OrganizationNumber, opts => opts.MapFrom(m => m.OrganizationNumber))
                .ForMember(m => m.PhoneNumber, opts => opts.MapFrom(m => m.PhoneNumber))
                .ForMember(m => m.Email, opts => opts.MapFrom(m => m.Email));
            });

            mapper = config.CreateMapper();

            _dbContext = new HardwareServiceOrderContext(ContextOptions);
            var hardwareServiceRepository = new HardwareServiceOrderRepository(_dbContext);
            var statusHandlers = new Dictionary<ServiceStatusEnum, ServiceOrderStatusHandlerService>
            {

            };

            _hardwareServiceOrderService = new HardwareServiceOrderService(hardwareServiceRepository, _mapper, new Mock<IProviderFactory>().Object, statusHandlers);

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

            //_providerFactory = new ProviderFactory(Options.Create(new ServiceProviderConfiguration
            //{
            //    Providers = new Dictionary<string, ProviderConfiguration>
            //    {
            //        {"ConmodoNO",new ProviderConfiguration{ ApiBaseUrl ="",ApiPassword="",ApiUsername=""} }
            //    }
            //}));
            var providerFactory = new Mock<ProviderFactory>(Options.Create(new ServiceProviderConfiguration
            {
                Providers = new Dictionary<string, ProviderConfiguration>
                {
                    {"ConmodoNO",new ProviderConfiguration{ApiBaseUrl ="",ApiPassword="",ApiUsername=""} }
                }
            }));

            _hardwareServiceOrderService = new HardwareServiceOrderService(hardwareServiceRepository,mapper, _emailService, providerFactory.Object);
            
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
            var serviceOrder = new API.ViewModels.HardwareServiceOrder
            {
                ErrorDescription = "sd",
                OrderedBy = new API.ViewModels.OrderedByUserDTO
                {
                    FistName = "sd",
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
                    DeliveryAddress = new API.ViewModels.Location
                    {
                        Recipient = "fs",
                        Address1 = "f",
                        Address2 = "f",
                        City = "f",
                        Country = "FS",
                        PostalCode = "erg"
                    }
                };

            
            var serviceOrderDTO = mapper.Map<API.ViewModels.HardwareServiceOrder, HardwareServiceOrderDTO>(serviceOrder);


            Assert.NotNull(serviceOrderDTO);

            Assert.Equal(serviceOrder.AssetInfo.Imei, serviceOrderDTO.AssetInfo.Imei);
            Assert.Equal(serviceOrder.AssetInfo.Model, serviceOrderDTO.AssetInfo.Model);
            Assert.Equal(serviceOrder.AssetInfo.SerialNumber, serviceOrderDTO.AssetInfo.SerialNumber);
            Assert.Equal(serviceOrder.AssetInfo.Brand, serviceOrderDTO.AssetInfo.Brand);
            Assert.Equal(serviceOrder.AssetInfo.Accessories, serviceOrderDTO.AssetInfo.Accessories);
            Assert.Equal(serviceOrder.AssetInfo.AssetCategoryId, serviceOrderDTO.AssetInfo.AssetCategoryId);
            Assert.Equal(serviceOrder.AssetInfo.PurchaseDate, serviceOrderDTO.AssetInfo.PurchaseDate);

            Assert.Equal(serviceOrder.DeliveryAddress.Address2, serviceOrderDTO.DeliveryAddress.Address2);
            Assert.Equal(serviceOrder.DeliveryAddress.Address1, serviceOrderDTO.DeliveryAddress.Address1);
            Assert.Equal(serviceOrder.DeliveryAddress.City, serviceOrderDTO.DeliveryAddress.City);
            Assert.Equal(serviceOrder.DeliveryAddress.Country, serviceOrderDTO.DeliveryAddress.Country);
            Assert.Equal(serviceOrder.DeliveryAddress.Recipient, serviceOrderDTO.DeliveryAddress.Recipient);


            Assert.Equal(serviceOrder.OrderedBy.FistName, serviceOrderDTO.FirstName);
            Assert.Equal(serviceOrder.OrderedBy.LastName, serviceOrderDTO.LastName);
            Assert.Equal(serviceOrder.OrderedBy.PartnerName, serviceOrderDTO.PartnerName);
            Assert.Equal(serviceOrder.OrderedBy.PartnerOrganizationNumber, serviceOrderDTO.PartnerOrganizationNumber);
            Assert.Equal(serviceOrder.OrderedBy.PhoneNumber, serviceOrderDTO.PhoneNumber);
            Assert.Equal(serviceOrder.OrderedBy.OrganizationName, serviceOrderDTO.OrganizationName);

            Assert.Equal(serviceOrder.ErrorDescription, serviceOrderDTO.ErrorDescription);
        }

        [Fact]
        public async Task MapHardwareServiceOrderDTOToNewExternalRepairOrderDTO()
        {
            var serviceOrderDTO = new HardwareServiceOrderDTO
            {
                ErrorDescription = "sd",
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
                    PhoneNumber = "23",
                    UserDescription = "eefeef",
                    BasicDescription = "sdsd",
                
                AssetInfo = new AssetInfoDTO("sdh","sd","dssd",3, "500119468586675", "500119468586675",new DateOnly(),
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
                ServiceProvider = new ServiceProvider
                {
                    OrganizationId = new Guid()
                },
                ServiceStatus = new ServiceStatus
                {
                    Id = 17
                },
                ServiceType = new ServiceType
                {
                    Id= 5
                }
            };

            var customerSettingDto = _hardwareServiceOrderService.CreateHardwareServiceOrderAsync(CUSTOMER_ONE_ID,serviceOrderDTO).Result;

            
            Assert.NotNull(customerSettingDto);

            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.Imei, serviceOrderDTO.AssetInfo.Imei);
            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.Model, serviceOrderDTO.AssetInfo.Model);
            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.SerialNumber, serviceOrderDTO.AssetInfo.SerialNumber);
            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.Brand, serviceOrderDTO.AssetInfo.Brand);
            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.Accessories, serviceOrderDTO.AssetInfo.Accessories);
            //Assert.Equal(newExternalRepairOrderDTO.AssetInfo.AssetCategoryId, serviceOrderDTO.AssetInfo.AssetCategoryId);
            
        }

    }
}
