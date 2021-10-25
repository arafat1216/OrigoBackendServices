#nullable enable
using OrigoApiGateway.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IUserPermissionService
    {
        /// <summary>
        /// Returns a new identity containing the user permissions as Claims
        /// </summary>
        /// <param name="sub">The user external id (sub claim)</param>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        Task<ClaimsIdentity> GetUserPermissionsIdentityAsync(string sub, string userName, CancellationToken cancellationToken);
        Task<IList<OrigoUserPermissions>> GetUserPermissionsAsync(string userName);
        Task<OrigoUserPermissions> AddUserPermissionsForUserAsync(string userName, NewUserPermissions userPermission);
        Task<OrigoUserPermissions> RemoveUserPermissionsForUserAsync(string userName, NewUserPermissions userPermission);
        Task<IList<string>> GetAllRolesAsync();
    }
}
