using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Services
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly HttpClient _httpClient;
        private readonly UserConfiguration _options;
        private HttpClient HttpClient { get; }

        public UserPermissionService(HttpClient httpClient, IOptions<UserConfiguration> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
            HttpClient = new HttpClient();
        }

        public async Task<ClaimsIdentity> GetUserPermissionsIdentityAsync(string sub, string userName, CancellationToken cancellationToken)
        {
            var encodedUserName = WebUtility.UrlEncode(userName);
            var userPermissions = await _httpClient.GetFromJsonAsync<IList<UserPermissionsDTO>>(
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

        public async Task<ClaimsIdentity> AddUserPermissionsForUserAsync(string userName, NewUserPermissions userPermission)
        {
            var encodedUserName = WebUtility.UrlEncode(userName);
            var response = await _httpClient.PutAsync($"{_options.ApiPath}/users/{encodedUserName}/permissions", JsonContent.Create(userPermission));
            if (response == null)
            {
                return new ClaimsIdentity();
            }
            var userPermissions = await response.Content.ReadFromJsonAsync<UserPermissionsDTO>();
            var claimPermissions = userPermissions.PermissionNames.Select(permissionName => new Claim("Permissions", permissionName)).ToList();
            var claimAccessList = userPermissions.AccessList.Select(accessTo => new Claim("AllowedAccess", accessTo.ToString())).ToList();

            var permissionsIdentity = new ClaimsIdentity(claimPermissions);
            permissionsIdentity.AddClaims(claimPermissions);
            permissionsIdentity.AddClaims(claimAccessList);
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, userPermissions.Role));

            return permissionsIdentity;
        }

        public async Task<ClaimsIdentity> RemoveUserPermissionsForUserAsync(string userName, NewUserPermissions userPermission)
        {
            var encodedUserName = WebUtility.UrlEncode(userName);
            var requestUri = $"{_options.ApiPath}/users/{encodedUserName}/permissions";
            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = JsonContent.Create(userPermission),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };
            var response = await HttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var exception = new BadHttpRequestException("Unable to remove the asset category to the customer.", (int)response.StatusCode);
                throw exception;
            }
            var userPermissions = await response.Content.ReadFromJsonAsync<UserPermissionsDTO>();
            if (userPermissions == null)
            {
                return new ClaimsIdentity();
            }

            var claimPermissions = userPermissions.PermissionNames.Select(permissionName => new Claim("Permissions", permissionName)).ToList();
            var claimAccessList = userPermissions.AccessList.Select(accessTo => new Claim("AllowedAccess", accessTo.ToString())).ToList();

            var permissionsIdentity = new ClaimsIdentity(claimPermissions);
            permissionsIdentity.AddClaims(claimPermissions);
            permissionsIdentity.AddClaims(claimAccessList);
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, userPermissions.Role));

            return permissionsIdentity;
        }
    }
}