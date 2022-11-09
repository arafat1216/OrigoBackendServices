using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CustomerServices.UnitTests
{
    public class OktaServicesTests
    {
        private IOktaServices _oktaServices;
        readonly bool _isIntegrationTest = false;
        public OktaServicesTests()
        {
            if (_isIntegrationTest)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets<OktaServicesTests>(optional: true)
                .Build();
                _oktaServices = new OktaServices(Options.Create(configuration.GetSection("Okta").Get<OktaConfiguration>()));
            }
            else
            {
                var mockOktaService = new Mock<IOktaServices>();
                mockOktaService.Setup(x => x.GetOktaUserProfileByLoginEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync(new ServiceModels.OktaUserDTO
                    {
                        Id = "[ID]",
                        Status = "[Active]",
                        Profile = new ServiceModels.OktaUserProfile
                        {
                            AgentSalesId = "[AgentSalesId]",
                            OrganizationNumber = "[OrganizationNumber]"
                        }
                    });
                _oktaServices = mockOktaService.Object;
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetUserProfile()
        {
            var oktaProfile = await _oktaServices.GetOktaUserProfileByLoginEmailAsync("partneradmin@techstep-test.no");
            Assert.NotNull(oktaProfile);
            Assert.NotNull(oktaProfile.Profile);
            Assert.NotNull(oktaProfile.Profile.OrganizationNumber);
        }
    }
}
