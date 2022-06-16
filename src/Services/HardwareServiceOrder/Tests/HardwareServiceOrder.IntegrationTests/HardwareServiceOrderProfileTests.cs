using AutoMapper;
using HardwareServiceOrder.API.Mappings;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices.ServiceModels;
using System;
using Xunit;

namespace HardwareServiceOrder.IntegrationTests
{
    public class HardwareServiceOrderProfileTests
    {
        private readonly IMapper _mapper;

        public HardwareServiceOrderProfileTests()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new HardwareServiceOrderProfile());
            }).CreateMapper();
        }

        [Fact]
        public void CustomerSettingsToCustomerSettingsDTO()
        {
            CustomerSettings customerSettings1 = new CustomerSettings
            {
                CustomerId = System.Guid.NewGuid(),
                LoanDevice = new LoanDevice
                {
                    Email = "atish@bs.no",
                    PhoneNumber = "+8801724592272"
                }
            };

            var dto = _mapper.Map<CustomerSettingsDTO>(customerSettings1);
            Assert.Equal(customerSettings1.LoanDevice.PhoneNumber, dto.LoanDevicePhoneNumber);
            Assert.Equal(customerSettings1.LoanDevice.Email, dto.LoanDeviceEmail);
        }

        [Fact]
        public void CustomerSettingsToCustomerSettingsDTO_NullLoanDevice()
        {
            CustomerSettings customerSettings1 = new CustomerSettings
            {
                CustomerId = System.Guid.NewGuid()
            };

            var dto = _mapper.Map<CustomerSettingsDTO>(customerSettings1);
            Assert.Null(dto.LoanDevicePhoneNumber);
            Assert.Null(dto.LoanDeviceEmail);
        }

        [Fact]
        public void AssetInfoToAssetInfoDTO()
        {
            var asset = new AssetInfo
            {
                PurchaseDate = new DateOnly(),
                Imei = "123456789",
                SerialNumber = "1234567889"
            };

            var dto = _mapper.Map<AssetInfoDTO>(asset);

            Assert.True(dto.PurchaseDate.HasValue);
        }
    }
}
