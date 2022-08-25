using AutoMapper;
using HardwareServiceOrderServices.ServiceModels;
using System.Linq;

namespace HardwareServiceOrder.API.Mappings.Tests
{
    public class CustomerServiceProviderProfileTests
    {
        private readonly IMapper _mapper;

        public CustomerServiceProviderProfileTests()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CustomerServiceProviderProfile());
                mc.AddProfile(new ObscuredApiCredentialProfile());
            }).CreateMapper();
        }


        [Fact()]
        public void CustomerServiceProviderProfileTest()
        {
            // Arrange
            int id = 1;

            var apiCredentials = new List<ApiCredentialDTO>()
            {
                new(id, (int)ServiceTypeEnum.SUR, "ApiUsername", null)
            };

            CustomerServiceProviderDto customerServiceProviderDto = new(id, Guid.NewGuid(), (int)ServiceProviderEnum.ConmodoNo, apiCredentials);

            // Act
            var mapped = _mapper.Map<ViewModels.CustomerServiceProvider>(customerServiceProviderDto);

            // Assert
            Assert.True(mapped.ApiCredentials?.Count == 1);
            Assert.Equal(customerServiceProviderDto.ServiceProviderId, mapped.ServiceProviderId);
        }
    }
}


namespace HardwareServiceOrderServices.Mappings.Tests
{
    public class CustomerServiceProviderProfileTests
    {
        private readonly IMapper _mapper;

        public CustomerServiceProviderProfileTests()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CustomerServiceProviderProfile());
                mc.AddProfile(new ApiCredentialProfile());
            }).CreateMapper();
        }


        [Fact(DisplayName = "'ApiCredential' EF Entity to 'ApiCredentialDTO' entity remapping")]
        public void CustomerServiceProvider_EfToDto_Test()
        {
            // Arrange
            CustomerServiceProvider customerServiceProvider = new()
            {
                CustomerId = Guid.NewGuid(),
                ServiceProviderId = (int)ServiceProviderEnum.ConmodoNo,

                ApiCredentials = new List<ApiCredential>()
                {
                    new(0, (int)ServiceProviderEnum.ConmodoNo, "[ApiUsername]", "[ApiPassword]"),
                }
            };

            // Act
            var result = _mapper.Map<CustomerServiceProviderDto>(customerServiceProvider);

            // Asserts

            Assert.Equal(customerServiceProvider.CustomerId, result.OrganizationId);
            Assert.Equal(customerServiceProvider.ServiceProviderId, result.ServiceProviderId);

            // The remapped API credentials list should not be null, and should still contain items
            Assert.NotNull(result.ApiCredentials);
            Assert.True(result.ApiCredentials!.Any());
            Assert.Equal(customerServiceProvider.ApiCredentials.Count, result.ApiCredentials!.Count);
        }
    }
}