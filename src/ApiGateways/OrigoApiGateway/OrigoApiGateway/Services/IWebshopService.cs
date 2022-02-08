using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IWebshopService
    {
        Task ProvisionUserAsync(string email);
    }
}
