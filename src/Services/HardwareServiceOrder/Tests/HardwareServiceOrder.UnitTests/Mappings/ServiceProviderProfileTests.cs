using AutoMapper;
using HardwareServiceOrderServices.ServiceModels;
using System.Linq;

namespace HardwareServiceOrderServices.Mappings.Tests
{
    public class ServiceProviderProfileTests
    {
        private readonly IMapper _mapper;

        public ServiceProviderProfileTests()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ServiceOrderAddonProfile());
                mc.AddProfile(new ServiceProviderProfile());
            }).CreateMapper();
        }


        [Fact(DisplayName = "'ServiceProvider' to 'ServiceProviderDTO' remapping")]
        public void ServiceProviderProfileTest()
        {
            // Arrange
            List<ServiceProviderServiceType> serviceProviderServiceTypes = new()
            {
                new(1, 1),
            };

            List<ServiceOrderAddon> serviceOrderAddons = new()
            {
                new(1, (int)ServiceProviderEnum.ConmodoNo, "3rd party id", true, true, Guid.NewGuid(), DateTimeOffset.Parse("2020-01-01")),
            };

            ServiceProvider serviceProvider = new ServiceProvider(1, "ServiceProvider", Guid.NewGuid(), serviceProviderServiceTypes, serviceOrderAddons, Guid.NewGuid(), DateTimeOffset.Parse("2020-01-01"));

            // Act
            var result = _mapper.Map<ServiceProviderDTO>(serviceProvider);

            // Assert
            Assert.Equal(serviceProvider.Id, result.Id);
            Assert.Equal(serviceProvider.Name, result.Name);
            Assert.Equal(serviceProvider.OrganizationId, result.OrganizationId);

            Assert.Equal(serviceProvider.SupportedServiceTypes!.Count, result.SupportedServiceTypeIds?.Count());
            Assert.Equal(serviceProvider.OfferedServiceOrderAddons!.Count, result.OfferedServiceOrderAddons?.Count);
        }
    }
}