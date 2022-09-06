using AutoMapper;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;

namespace OrigoApiGateway.Mappings.HardwareServiceOrder.Tests
{
    public class ServiceOrderAddonProfileTests
    {
        private readonly IMapper _mapper;

        public ServiceOrderAddonProfileTests()
        {
            _mapper = new MapperConfiguration(config =>
            {
                config.AddProfile(new ServiceOrderAddonProfile());
            }).CreateMapper();
        }


        [Fact(DisplayName = "Automapper profile: ServiceOrderAddon to CustomerPortalServiceOrderAddon")]
        public void ServiceOrderAddon_To_CustomerPortalServiceOrderAddon_ProfileTest()
        {
            // Arrange
            ServiceOrderAddon serviceOrderAddon = new() { Id = 1, ServiceProviderId = 1, IsCustomerTogglable = true, IsUserSelectable = true };

            // Act
            var result = _mapper.Map<CustomerPortalServiceOrderAddon>(serviceOrderAddon);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(serviceOrderAddon.Id, result.Id);
            Assert.Equal(serviceOrderAddon.ServiceProviderId, result.ServiceProviderId);
            Assert.Equal(serviceOrderAddon.IsUserSelectable, result.IsUserSelectable);
        }


        [Fact(DisplayName = "Automapper profile: ServiceOrderAddon to UserPortalServiceOrderAddon")]
        public void ServiceOrderAddon_To_UserPortalServiceOrderAddon_ProfileTest()
        {
            // Arrange
            ServiceOrderAddon serviceOrderAddon = new() { Id = 1, ServiceProviderId = 1, IsCustomerTogglable = true, IsUserSelectable = true };

            // Act
            var result = _mapper.Map<UserPortalServiceOrderAddon>(serviceOrderAddon);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(serviceOrderAddon.Id, result.Id);
            Assert.Equal(serviceOrderAddon.ServiceProviderId, result.ServiceProviderId);
        }
    }
}