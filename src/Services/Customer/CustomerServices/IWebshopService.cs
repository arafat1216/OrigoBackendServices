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
        Task CheckAndProvisionWebShopUserAsync(string email);
    }
}
