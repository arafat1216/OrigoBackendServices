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
                ServiceId = "[ServiceId]",
                AssetCategoryIds = new System.Collections.Generic.List<int> { 1, 2 },
                CustomerId = System.Guid.NewGuid(),
                LoanDevice = new LoanDevice
                {
                    Email = "atish@bs.no",
                    PhoneNumber = "+8801724592272"
                },
                ProviderId = 1
            };

            var dto = _mapper.Map<CustomerSettingsDTO>(customerSettings1);

            Assert.Equal(customerSettings1.ServiceId, dto.ApiUserName);
            Assert.Equal(customerSettings1.AssetCategoryIds, dto.AssetCategoryIds);
            Assert.Equal(customerSettings1.CustomerId, dto.CustomerId);
            Assert.Equal(customerSettings1.ProviderId, dto.ProviderId);
            Assert.Equal(customerSettings1.LoanDevice.PhoneNumber, dto.LoanDevicePhoneNumber);
            Assert.Equal(customerSettings1.LoanDevice.Email, dto.LoanDeviceEmail);
        }

        [Fact]
        public void CustomerSettingsToCustomerSettingsDTO_NullLoanDevice()
        {
            CustomerSettings customerSettings1 = new CustomerSettings
            {
                ServiceId = "[ServiceId]",
                AssetCategoryIds = new System.Collections.Generic.List<int> { 1, 2 },
                CustomerId = System.Guid.NewGuid(),
                ProviderId = 1
            };

            var dto = _mapper.Map<CustomerSettingsDTO>(customerSettings1);

            Assert.Equal(customerSettings1.ServiceId, dto.ApiUserName);
            Assert.Equal(customerSettings1.AssetCategoryIds, dto.AssetCategoryIds);
            Assert.Equal(customerSettings1.CustomerId, dto.CustomerId);
            Assert.Equal(customerSettings1.ProviderId, dto.ProviderId);
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
