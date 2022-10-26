using OrigoApiGateway.Models;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IWebshopService
    {
        /// <summary>
        /// Provisions an Implement-specific user into the web shop as an Employee
        /// </summary>
        /// <param name="email">The email of the federated user</param>
        /// <returns></returns>
        Task ProvisionImplementUserAsync(string email);

        /// <summary>
        /// Provisions a non-Implement User into the web shop as an Employee
        /// </summary>
        /// <param name="userId">User Id of the user to be provisioned</param>
        /// <returns></returns>
        Task ProvisionUserWithEmployeeRoleAsync(Guid userId);
    }
}
