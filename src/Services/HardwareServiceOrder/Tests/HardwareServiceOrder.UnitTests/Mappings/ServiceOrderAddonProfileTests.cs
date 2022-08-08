using AutoMapper;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings.Tests
{
    public class ServiceOrderAddonProfileTests
    {
        private readonly IMapper _mapper;

        public ServiceOrderAddonProfileTests()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ServiceOrderAddonProfile());
            }).CreateMapper();
        }


        [Fact(DisplayName = "Validate 'ServiceOrderAddon' to 'ServiceOrderAddonDTO' remapping")]
        public void ServiceOrderAddon_To_ServiceOrderAddonDTO_VerifyMappingTest()
        {
            // Arrange
#pragma warning disable CS0618 // Type or member is obsolete
            ServiceOrderAddon serviceOrderAddon = new(1, (int)ServiceProviderEnum.ConmodoNo, "3rd party id", true, true, Guid.NewGuid(), DateTimeOffset.Parse("2020-01-01"));
#pragma warning restore CS0618 // Type or member is obsolete

            // Act
            var result = _mapper.Map<ServiceOrderAddonDTO>(serviceOrderAddon);

            // Assert
            Assert.Equal(serviceOrderAddon.Id, result.Id);
            Assert.Equal(serviceOrderAddon.ServiceProviderId, serviceOrderAddon.ServiceProviderId);
            Assert.Equal(serviceOrderAddon.ThirdPartyId, serviceOrderAddon.ThirdPartyId);
            Assert.Equal(serviceOrderAddon.IsUserSelectable, serviceOrderAddon.IsUserSelectable);
            Assert.Equal(serviceOrderAddon.IsCustomerTogglable, serviceOrderAddon.IsCustomerTogglable);
            Assert.Equal(serviceOrderAddon.CreatedBy, serviceOrderAddon.CreatedBy);
            Assert.Equal(serviceOrderAddon.DateCreated, serviceOrderAddon.DateCreated);
        }
    }
}