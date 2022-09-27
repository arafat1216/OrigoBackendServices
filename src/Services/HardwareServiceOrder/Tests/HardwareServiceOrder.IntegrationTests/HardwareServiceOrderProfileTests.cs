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
            API.ViewModels.CustomerSettings customerSettings1 = new()
            {
                CustomerId = Guid.NewGuid(),
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
            API.ViewModels.CustomerSettings customerSettings1 = new()
            {
                CustomerId = Guid.NewGuid()
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
            var serviceOrder = new NewHardwareServiceOrder
            {
                UserDescription = "Something is wrong!",
                OrderedBy = new ContactDetailsExtended
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    UserId = Guid.NewGuid(),
                    Email = "user@email.com",
                    PartnerId = Guid.NewGuid(),
                    PartnerName = "PartnerName",
                    PartnerOrganizationNumber = "22997733",
                    OrganizationId = Guid.NewGuid(),
                    OrganizationName = "CustomerOrganizationName",
                    OrganizationNumber = "888777666",
                    PhoneNumber = "+4799999999"
                },
                AssetInfo = new API.ViewModels.AssetInfo
                {
                    Imei = "500119468586675",
                    //AssetLifecycleId = new Guid(),
                    Accessories = new List<string>
                    {
                        "Charger"
                    },
                    AssetCategoryId = 3,
                    Model = "Model",
                    Brand = "Brand",
                    PurchaseDate = new DateOnly(),
                    SerialNumber = "S/N 123456",
                    AssetLifecycleId = Guid.NewGuid(),
                    AssetName = "AssetName"
                },
                DeliveryAddress = new API.ViewModels.DeliveryAddress
                {
                    Recipient = "Recipient",
                    Address1 = "Address1",
                    Address2 = "Address2",
                    City = "Oslo",
                    Country = "NO",
                    PostalCode = "0275",
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

            Assert.Equal(serviceOrder.UserDescription, serviceOrderDTO.UserDescription);
        }
    }
}
