using System;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class WebshopService : IWebshopService
    {
        private readonly IOktaServices _oktaServices;
        public WebshopService(IOktaServices oktaServices)
        {
            _oktaServices = oktaServices;
        }

        public async Task CheckAndProvisionWebShopUserAsync(string email)
        {
            var oktaUser = await _oktaServices.GetOktaUserProfileByLoginEmailAsync(email);

            if (oktaUser == null)
                throw new ArgumentException("General request towards web shop failed.");

            if (string.IsNullOrEmpty(oktaUser.Profile?.OrganizationNumber))
                throw new ArgumentException("User missing organization number.");
        }
    }
}
