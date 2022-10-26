using System;
using System.Net.Http;
using System.Threading.Tasks;
using CustomerServices.Models;

namespace CustomerServices
{
    public interface IWebshopService
    {
        Task<string> GetLitiumTokenAsync();
        Task<LitiumPerson> GetLitiumPersonByEmail(string email);
        Task<LitiumOrganization> GetLitiumOrganizationByOrgnumberAsync(string orgNumber);
        Task<HttpResponseMessage> PostLitiumPerson(LitiumPerson person);

        /// <summary>
        /// Implement-specific web shop user provisioning
        /// </summary>
        /// <param name="email">The email of the user from its federated login</param>
        /// <returns></returns>
        Task CheckAndProvisionImplementWebShopUserAsync(string email);

        /// <summary>
        /// Non-implement specific web shop user provisioning
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task CheckAndProvisionWebShopUserAsync(Guid userId);

        /// <summary>
        /// Common web shop provisioning method that creates or updates a web shop user in Litium, with the Employee organization role
        /// </summary>
        /// <param name="oktaEmail">Valid email address corresponding to an OktaUser</param>
        /// <param name="organizationNumber">The organization number of the organization/customer that the User belongs to</param>
        /// <returns></returns>
        Task ProvisionWebShopUserByOktaEmailAndOrgnumberAsync(string oktaEmail, string organizationNumber);
    }
}
