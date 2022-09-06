using AutoMapper;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;

namespace OrigoApiGateway.Mappings.HardwareServiceOrder.Tests
{
    public class CustomerServiceProviderProfileTests
    {
        private readonly IMapper _mapper;

        public CustomerServiceProviderProfileTests()
        {
            _mapper = new MapperConfiguration(config =>
            {
                config.AddProfile(new CustomerServiceProviderProfile());
                config.AddProfile(new ServiceOrderAddonProfile());
            }).CreateMapper();
        }


        private static CustomerServiceProvider GetCustomerServiceProvider()
        {
            int serviceProviderId = 1;

            List<ServiceOrderAddon> serviceOrderAddons = new()
            {
                { new() { Id = 1, ServiceProviderId = serviceProviderId, IsCustomerTogglable = true, IsUserSelectable = true } },
                { new() { Id = 2, ServiceProviderId = serviceProviderId, IsCustomerTogglable = true, IsUserSelectable = false } },
                { new() { Id = 3, ServiceProviderId = serviceProviderId, IsCustomerTogglable = false, IsUserSelectable = true, } },
                { new() { Id = 4, ServiceProviderId = serviceProviderId, IsCustomerTogglable = false, IsUserSelectable = false } }
            };

            List<ObscuredApiCredential> obscuredApiCredentials = new()
            {
                { new() {ServiceTypeId = 1, ApiPasswordFilled = true, ApiUsernameFilled = true } },
                { new() {ServiceTypeId = 2, ApiPasswordFilled = false, ApiUsernameFilled = true } },
                { new() {ServiceTypeId = 3, ApiPasswordFilled = true, ApiUsernameFilled = false } }
            };

            CustomerServiceProvider customerServiceProvider = new()
            {
                ServiceProviderId = serviceProviderId,
                ActiveServiceOrderAddons = serviceOrderAddons,
                ApiCredentials = obscuredApiCredentials,
            };

            return customerServiceProvider;
        }


        [Fact(DisplayName = "Automapper profile: CustomerServiceProvider to CustomerPortalCustomerServiceProvider")]
        public void CustomerServiceProvider_To_CustomerPortalCustomerServiceProvider_ProfileTest()
        {
            // Arrange
            CustomerServiceProvider customerServiceProvider = GetCustomerServiceProvider();

            // Act
            var result = _mapper.Map<CustomerPortalCustomerServiceProvider>(customerServiceProvider);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.ActiveServiceOrderAddons!.Count);
            Assert.Equal(customerServiceProvider.ServiceProviderId, result.ServiceProviderId);
            Assert.Equal(customerServiceProvider.ServiceProviderId, result.ServiceProviderId);
        }


        [Fact(DisplayName = "Automapper profile: CustomerServiceProvider to CustomerPortalCustomerServiceProvider (null check)")]
        public void CustomerServiceProvider_To_CustomerPortalCustomerServiceProvider_NullValue_ProfileTest()
        {
            // Arrange
            CustomerServiceProvider customerServiceProvider = GetCustomerServiceProvider();
            customerServiceProvider.ApiCredentials = null;
            customerServiceProvider.ActiveServiceOrderAddons = null;

            // Act
            var result = _mapper.Map<CustomerPortalCustomerServiceProvider>(customerServiceProvider);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.ActiveServiceOrderAddons!.Count);
            Assert.Equal(customerServiceProvider.ServiceProviderId, result.ServiceProviderId);
        }


        [Fact(DisplayName = "Automapper profile: CustomerServiceProvider to UserPortalCustomerServiceProvider")]
        public void CustomerServiceProvider_To_UserPortalCustomerServiceProvider_ProfileTest()
        {
            // Arrange
            CustomerServiceProvider customerServiceProvider = GetCustomerServiceProvider();

            // Act
            var result = _mapper.Map<UserPortalCustomerServiceProvider>(customerServiceProvider);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.ActiveServiceOrderAddons!.Count);
            Assert.Equal(customerServiceProvider.ServiceProviderId, result.ServiceProviderId);
            Assert.Equal(customerServiceProvider.ServiceProviderId, result.ServiceProviderId);
        }


        [Fact(DisplayName = "Automapper profile: CustomerServiceProvider to UserPortalCustomerServiceProvider (null check)")]
        public void CustomerServiceProvider_To_UserPortalCustomerServiceProvider_NullValue_ProfileTest()
        {
            // Arrange
            CustomerServiceProvider customerServiceProvider = GetCustomerServiceProvider();
            customerServiceProvider.ApiCredentials = null;
            customerServiceProvider.ActiveServiceOrderAddons = null;

            // Act
            var result = _mapper.Map<UserPortalCustomerServiceProvider>(customerServiceProvider);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.ActiveServiceOrderAddons!.Count);
            Assert.Equal(customerServiceProvider.ServiceProviderId, result.ServiceProviderId);
            Assert.Equal(customerServiceProvider.ServiceProviderId, result.ServiceProviderId);
        }
    }
}