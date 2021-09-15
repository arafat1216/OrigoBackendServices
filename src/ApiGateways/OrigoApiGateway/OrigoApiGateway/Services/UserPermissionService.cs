using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Services
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly HttpClient _httpClient;
        private readonly UserConfiguration _options;

        public UserPermissionService(HttpClient httpClient, IOptions<UserConfiguration> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<ClaimsIdentity> GetUserPermissionsIdentityAsync(string sub, string userName,
            CancellationToken cancellationToken)
        {
            var encodedUserName = WebUtility.UrlEncode(userName);
            var userPermissions =
                await _httpClient.GetFromJsonAsync<IList<UserPermissionsDTO>>(
                    $"{_options.ApiPath}/users/{encodedUserName}/permissions", cancellationToken);
            if (userPermissions == null || !userPermissions.Any())
            {
                return new ClaimsIdentity();
            }

            var claimPermissions = userPermissions.First().PermissionNames
                .Select(permissionName => new Claim("Permissions", permissionName)).ToList();
            var claimAccessList = userPermissions.First().AccessList
                .Select(accessTo => new Claim("AllowedAccess", accessTo.ToString())).ToList();

            var permissionsIdentity = new ClaimsIdentity(claimPermissions);
            permissionsIdentity.AddClaims(claimPermissions);
            permissionsIdentity.AddClaims(claimAccessList);
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, userPermissions.First().Role));

            return permissionsIdentity;
        }
    }
}