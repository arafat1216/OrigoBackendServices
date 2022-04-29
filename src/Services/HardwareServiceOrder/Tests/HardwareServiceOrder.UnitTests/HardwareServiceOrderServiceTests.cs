using AutoMapper;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.ServiceModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace HardwareServiceOrder.UnitTests
{
    public class HardwareServiceOrderServiceTests : HardwareServiceOrderServiceBaseTests
    {
        private readonly IMapper? _mapper;
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
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

            var hardwareServiceRepository = new HardwareServiceOrderRepository(new HardwareServiceOrderContext(ContextOptions));
            _hardwareServiceOrderService = new HardwareServiceOrderService(hardwareServiceRepository, _mapper);
        }

        [Fact]
        public async Task ConfigureServiceId()
        {
            var serviceId = "[ServiceId]";
            var settings = await _hardwareServiceOrderService.ConfigureServiceIdAsync(CUSTOMER_ONE_ID, serviceId, CALLER_ONE_ID);
            Assert.Equal(serviceId, settings.ServiceId);
        }

        [Fact]
        public async Task ConfigureLoanPhone()
        {
            var serviceId = "[ServiceId]";
            await _hardwareServiceOrderService.ConfigureServiceIdAsync(CUSTOMER_ONE_ID, serviceId, CALLER_ONE_ID);
            await _hardwareServiceOrderService.ConfigureLoanPhoneAsync(CUSTOMER_ONE_ID, "[+8801724592272]", "[test@test.com]", CALLER_ONE_ID);

            var settings = await _hardwareServiceOrderService.GetSettingsAsync(CUSTOMER_ONE_ID);
            Assert.Equal(serviceId, settings.ServiceId);
            Assert.Equal("[+8801724592272]", settings.LoanDevicePhoneNumber);
            Assert.Equal("[test@test.com]", settings.LoanDeviceEmail);
        }
    }
}
