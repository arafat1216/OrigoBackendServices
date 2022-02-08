using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IWebshopService
    {
        Task CheckAndProvisionWebShopUserAsync(string email);
    }
}
