using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;

#nullable enable

namespace OrigoApiGateway.Mappings.HardwareServiceOrder.Tests
{
    public class ServiceProviderProfileTests
    {
        private readonly IMapper _mapper;

        public ServiceProviderProfileTests()
        {
            _mapper = new MapperConfiguration(config =>
            {
                config.AddProfile(new ServiceProviderProfile());
                config.AddProfile(new ServiceOrderAddonProfile());
            }).CreateMapper();
        }


        private static Models.HardwareServiceOrder.Backend.ServiceProvider GetServiceProvider()
        {
            HashSet<int>? supportedServiceTypeIds = new()
            {
                1, 2
            };

            List<ServiceOrderAddon> serviceOrderAddons = new()
            {
                { new() { Id = 1, ServiceProviderId = 1, IsCustomerTogglable = true, IsUserSelectable = true } },
                { new() { Id = 2, ServiceProviderId = 1, IsCustomerTogglable = true, IsUserSelectable = false } },
                { new() { Id = 3, ServiceProviderId = 1, IsCustomerTogglable = false, IsUserSelectable = true } },
                { new() { Id = 4, ServiceProviderId = 1, IsCustomerTogglable = false, IsUserSelectable = false} },
            };

            Models.HardwareServiceOrder.Backend.ServiceProvider serviceProvider = new() { Id = 1, Name = "ServiceProvider", OrganizationId = Guid.Parse("cb78642b-5a4e-4325-a651-739012c2ebf5"), RequiresApiUsername = true, RequiresApiPassword = true, OfferedServiceOrderAddons = serviceOrderAddons, SupportedServiceTypeIds = supportedServiceTypeIds };

            return serviceProvider;
        }


        [Fact(DisplayName = "Automapper profile: ServiceProvider to CustomerPortalServiceProvider")]
        public void ServiceProvider_To_CustomerPortalServiceProvider_ProfileTest()
        {
            // Arrange
            var serviceProvider = GetServiceProvider();

            // Act
            var result = _mapper.Map<CustomerPortalServiceProvider>(serviceProvider);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(serviceProvider.Id, result.Id);
            Assert.Equal(serviceProvider.Name, result.Name);
            Assert.Equal(serviceProvider.SupportedServiceTypeIds!.Count, result.SupportedServiceTypeIds!.Count);
            Assert.Equal(2, result.OfferedServiceOrderAddons!.Count); // The result is filtered during the remapping.
        }
    }
}