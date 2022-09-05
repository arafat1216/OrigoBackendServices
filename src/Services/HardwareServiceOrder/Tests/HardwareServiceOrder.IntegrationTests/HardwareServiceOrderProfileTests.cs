using AutoMapper;
using HardwareServiceOrder.API.Mappings;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

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
            API.ViewModels.CustomerSettings customerSettings1 = new API.ViewModels.CustomerSettings
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
            API.ViewModels.CustomerSettings customerSettings1 = new API.ViewModels.CustomerSettings
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
            var asset = new API.ViewModels.AssetInfo
            {
                PurchaseDate = new DateOnly(),
                Imei = "123456789",
                SerialNumber = "1234567889"
            };

            var dto = _mapper.Map<AssetInfoDTO>(asset);

            Assert.True(dto.PurchaseDate.HasValue);
        }

        [Fact]
        public void MapHardwareViewModelToDto()
        {
            var serviceOrder = new API.ViewModels.NewHardwareServiceOrder
            {
                ErrorDescription = "sd",
                OrderedBy = new API.ViewModels.ContactDetailsExtended
                {
                    FirstName = "sd",
                    LastName = "sd",
                    UserId = new Guid(),
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
                    SerialNumber = "wewew",
                    AssetLifecycleId = new Guid(),
                    AssetName = "sd"
                },
                DeliveryAddress = new API.ViewModels.DeliveryAddress
                {
                    Recipient = "fs",
                    Address1 = "f",
                    Address2 = "f",
                    City = "f",
                    Country = "FS",
                    PostalCode = "erg",
                    RecipientType = RecipientTypeEnum.Personal
                }
            };

            var serviceOrderDTO = _mapper.Map<NewHardwareServiceOrderDTO>(serviceOrder);

            Assert.NotNull(serviceOrderDTO);

            Assert.Equal(serviceOrder.AssetInfo.Imei, serviceOrderDTO.AssetInfo.Imei);
            Assert.Equal(serviceOrder.AssetInfo.Model, serviceOrderDTO.AssetInfo.Model);
            Assert.Equal(serviceOrder.AssetInfo.SerialNumber, serviceOrderDTO.AssetInfo.SerialNumber);
            Assert.Equal(serviceOrder.AssetInfo.Brand, serviceOrderDTO.AssetInfo.Brand);
            Assert.Equal(serviceOrder.AssetInfo.Accessories, serviceOrderDTO.AssetInfo.Accessories);
            Assert.Equal(serviceOrder.AssetInfo.AssetCategoryId, serviceOrderDTO.AssetInfo.AssetCategoryId);

            Assert.Equal(serviceOrder.DeliveryAddress.Address2, serviceOrderDTO.DeliveryAddress.Address2);
            Assert.Equal(serviceOrder.DeliveryAddress.Address1, serviceOrderDTO.DeliveryAddress.Address1);
            Assert.Equal(serviceOrder.DeliveryAddress.City, serviceOrderDTO.DeliveryAddress.City);
            Assert.Equal(serviceOrder.DeliveryAddress.Country, serviceOrderDTO.DeliveryAddress.Country);
            Assert.Equal(serviceOrder.DeliveryAddress.Recipient, serviceOrderDTO.DeliveryAddress.Recipient);


            Assert.Equal(serviceOrder.OrderedBy.FirstName, serviceOrderDTO.OrderedBy.FirstName);
            Assert.Equal(serviceOrder.OrderedBy.LastName, serviceOrderDTO.OrderedBy.LastName);
            Assert.Equal(serviceOrder.OrderedBy.PartnerName, serviceOrderDTO.OrderedBy.PartnerName);
            Assert.Equal(serviceOrder.OrderedBy.PartnerOrganizationNumber, serviceOrderDTO.OrderedBy.PartnerOrganizationNumber);
            Assert.Equal(serviceOrder.OrderedBy.PhoneNumber, serviceOrderDTO.OrderedBy.PhoneNumber);
            Assert.Equal(serviceOrder.OrderedBy.OrganizationName, serviceOrderDTO.OrderedBy.OrganizationName);

            Assert.Equal(serviceOrder.ErrorDescription, serviceOrderDTO.ErrorDescription);
        }
    }
}
