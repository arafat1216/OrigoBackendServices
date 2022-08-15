using AutoMapper;
using HardwareServiceOrderServices.ServiceModels;
using System.Linq;

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