using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class OktaServicesTests
    {
        private IOktaServices _oktaServices;
        public OktaServicesTests()
        {
            var oktaOptions = Options.Create(new OktaConfiguration
            {
                OktaAppId = "0oa4zl5i8wGJW2jon0i7",
                OktaAuth = "00hHhWmBMgxeHV19AGOAXbCFIarKEWYQAvfia75Tcp",
                OktaGroupId = "00g6o97kdhlojNtZb0i7",
                OktaUrl = "https://techstepportal-admin.okta-emea.com/api/v1/"
            });

            _oktaServices = new OktaServices(oktaOptions);
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
