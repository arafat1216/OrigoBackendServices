﻿using AutoMapper;
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
            _hardwareServiceOrderService = new HardwareServiceOrderService(hardwareServiceRepository, _mapper);

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
    }
}
