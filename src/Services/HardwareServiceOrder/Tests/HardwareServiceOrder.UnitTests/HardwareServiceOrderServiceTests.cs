using AutoMapper;
using HardwareServiceOrder.API.Mappings;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using HardwareServiceOrderServices.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Reflection;
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
                cfg.CreateMap<API.ViewModels.Location, DeliveryAddressDTO>();

                cfg.CreateMap<API.ViewModels.HardwareServiceOrder, HardwareServiceOrderDTO>()
                    .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress));
            });

            mapper = config.CreateMapper();

            _dbContext = new HardwareServiceOrderContext(ContextOptions);
            var hardwareServiceRepository = new HardwareServiceOrderRepository(_dbContext);
            var statusHandlers = new Dictionary<ServiceStatusEnum, ServiceOrderStatusHandlerService>
            {

            };

            _hardwareServiceOrderService = new HardwareServiceOrderService(hardwareServiceRepository, _mapper, new Mock<IProviderFactory>().Object, statusHandlers);

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
                Id = new Guid("42447F76-D9A8-4F0A-B0FF-B4683ACEDD62"),
                BasicDescription = "sd",
                UserDescription = "de",
                FaultType = "sd",
                AssetId = new Guid("42447F76-D9A8-4F0A-B0FF-B4683ACEDD62"),
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
            Assert.Equal(serviceOrder.FaultType, serviceOrderDTO.FaultType);
            Assert.Equal(serviceOrder.DeliveryAddress.Address2, serviceOrderDTO.DeliveryAddress.Address2);
            Assert.Equal(serviceOrder.DeliveryAddress.Address1, serviceOrderDTO.DeliveryAddress.Address1);
            Assert.Equal(serviceOrder.DeliveryAddress.City, serviceOrderDTO.DeliveryAddress.City);
            Assert.Equal(serviceOrder.DeliveryAddress.Country, serviceOrderDTO.DeliveryAddress.Country);
            Assert.Equal(serviceOrder.DeliveryAddress.Recipient, serviceOrderDTO.DeliveryAddress.Recipient);
            Assert.Equal(serviceOrder.AssetId, serviceOrderDTO.AssetId);
            Assert.Equal(serviceOrder.BasicDescription, serviceOrderDTO.BasicDescription);
            Assert.Equal(serviceOrder.UserDescription, serviceOrderDTO.UserDescription);
        }

    }
}
