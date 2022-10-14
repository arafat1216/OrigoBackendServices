using System.Net;
using System.Security.Claims;
using AutoMapper;
using Common.Infrastructure;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.ProductCatalog;

namespace OrigoApiGateway.Services;

public class UserPermissionService : IUserPermissionService
{
    private readonly UserPermissionsConfigurations _options;
    private readonly ILogger<UserPermissionService> _logger;
    private readonly IMapper _mapper;
    private readonly IProductCatalogServices _productCatalogServices;
    private readonly ICacheService _cacheService;
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient HttpClient => _httpClientFactory.CreateClient("userpermissionservices");

    public UserPermissionService(ILogger<UserPermissionService> logger, IHttpClientFactory httpClientFactory,
        IOptions<UserPermissionsConfigurations> options, IMapper mapper, IProductCatalogServices productCatalogServices, ICacheService cacheService)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
        _mapper = mapper;
        _productCatalogServices = productCatalogServices;
        _cacheService = cacheService;
    }

    private async Task<IEnumerable<T>> GetFromCache<T>(string key, Func<Task<IEnumerable<T>>> lambda)
    {
        var values = await _cacheService.Get<IEnumerable<T>>(key);
        if (values is null)
        {
            values = await lambda();
            if (values.Any())
            {
                await _cacheService.Save(key, values, $"{5 * 60}");
            }
        }
        return values;
    }

    public async Task<ClaimsIdentity> GetUserPermissionsIdentityAsync(string sub, string userName,
        CancellationToken cancellationToken)
    {
        var encodedUserName = WebUtility.UrlEncode(userName);

        var userPermissions = await GetFromCache<UserPermissionsDTO>(
            $"{nameof(UserPermissionsDTO)}-{userName}", 
            async () => await HttpClient.GetFromJsonAsync<IList<UserPermissionsDTO>>($"{_options.ApiPath}/users/{encodedUserName}/permissions", cancellationToken));

        if (userPermissions == null || !userPermissions.Any())
        {
            return new ClaimsIdentity();
        }

        var claimPermissions = userPermissions.First()?.PermissionNames
            ?.Select(permissionName => new Claim("Permissions", permissionName)).ToList();
        var claimAccessList = userPermissions.First()?.AccessList
            ?.Select(accessTo => new Claim("AccessList", accessTo.ToString())).ToList();

        if (claimPermissions == null)
        {
            return new ClaimsIdentity();
        }

        var permissionsIdentity = new ClaimsIdentity(claimPermissions);
        if (claimAccessList != null)
        {
            permissionsIdentity.AddClaims(claimAccessList);
        }

        var mainOrganizationId = userPermissions.First()?.MainOrganizationId;
        if (mainOrganizationId != null)
        {
            var productPermissions = await GetFromCache(
                $"ProductPermissionsForOrganization-{mainOrganizationId.ToString()}", 
                async () => await _productCatalogServices.GetProductPermissionsForOrganizationAsync(mainOrganizationId.Value));

            permissionsIdentity.AddClaims(productPermissions.Select(p => new Claim("Permissions", p)));
            permissionsIdentity.AddClaim(new Claim("MainOrganization", mainOrganizationId.Value.ToString()));
        }

        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, userPermissions.First().Role));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, userPermissions.First().UserId.ToString()));

        return permissionsIdentity;
    }

    public async Task<IList<OrigoUserPermissions>> GetUserPermissionsAsync(string userName)
    {
        try
        {
            var encodedUserName = WebUtility.UrlEncode(userName);
            var userPermissions =
                await HttpClient.GetFromJsonAsync<IList<UserPermissionsDTO>>(
                    $"{_options.ApiPath}/users/{encodedUserName}/permissions");
            return _mapper.Map<List<OrigoUserPermissions>>(userPermissions);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "GetUserPermissionsAsync failed with HttpRequestException.");
            throw;
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "GetUserPermissionsAsync failed with content type is not valid.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "GetUserPermissionsAsync unknown error.");
            throw;
        }
    }

    public async Task<OrigoUserPermissions> AddUserPermissionsForUserAsync(string userName,
        NewUserPermissionsDTO userPermission)
    {
        var encodedUserName = WebUtility.UrlEncode(userName);
        var response = await HttpClient.PutAsync($"{_options.ApiPath}/users/{encodedUserName}/permissions",
            JsonContent.Create(userPermission));
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new BadHttpRequestException("Unable to save user permissions", (int)response.StatusCode);
        }

        var userPermissions = await response.Content.ReadFromJsonAsync<UserPermissionsDTO>();
        return userPermissions == null ? null : _mapper.Map<OrigoUserPermissions>(userPermissions);
    }

    public async Task<OrigoUserPermissions> RemoveUserPermissionsForUserAsync(string userName,
        NewUserPermissionsDTO userPermission)
    {
        var encodedUserName = WebUtility.UrlEncode(userName);
        var requestUri = $"{_options.ApiPath}/users/{encodedUserName}/permissions";
        var request = new HttpRequestMessage
        {
            Content = JsonContent.Create(userPermission),
            Method = HttpMethod.Delete,
            RequestUri = new Uri(requestUri, UriKind.Relative)
        };
        var response = await HttpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var exception = new BadHttpRequestException("Unable to remove the asset category to the customer.",
                (int)response.StatusCode);
            throw exception;
        }

        var userPermissions = await response.Content.ReadFromJsonAsync<UserPermissionsDTO>();
        return userPermissions == null ? null : _mapper.Map<OrigoUserPermissions>(userPermissions);
    }

    public async Task<IList<string>> GetAllRolesAsync()
    {
        try
        {
            var allRoles = await HttpClient.GetFromJsonAsync<IList<string>>($"{_options.ApiPath}/roles");
            return allRoles?.ToList();
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "GetUserPermissionsAsync failed with HttpRequestException.");
            throw;
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "GetUserPermissionsAsync failed with content type is not valid.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "GetUserPermissionsAsync unknown error.");
            throw;
        }
    }

    public async Task<IList<UserAdminDTO>> GetAllUserAdminsAsync(Guid? partnerId = null)
    {
        try
        {
            IList<UserAdminDTO> allAdmins;
            if (partnerId.HasValue)
            {
                allAdmins = await HttpClient.GetFromJsonAsync<IList<UserAdminDTO>>(
                    $"{_options.ApiPath}/admins?partnerId={partnerId}");
            }
            else
            {
                allAdmins = await HttpClient.GetFromJsonAsync<IList<UserAdminDTO>>($"{_options.ApiPath}/admins");
            }

            return allAdmins?.ToList();
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "GetAllUserAdminsAsync failed with HttpRequestException.");
            throw;
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "GetAllUserAdminsAsync failed with content type is not valid.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "GetAllUserAdminsAsync unknown error.");
            throw;
        }
    }

    public async Task<IList<UserAdminDTO>> GetAllCustomerAdminsAsync(Guid customerId)
    {
        try
        {
            var allAdmins =
                await HttpClient.GetFromJsonAsync<IList<UserAdminDTO>>(
                    $"{_options.ApiPath}/{customerId}/customer-admins");
            return allAdmins?.ToList();
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "GetAllCustomerAdminsAsync failed with HttpRequestException.");
            throw;
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "GetAllCustomerAdminsAsync failed with content type is not valid.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "GetAllCustomerAdminsAsync unknown error.");
            throw;
        }
    }

    public async Task<OrigoUsersPermissions> AddUsersPermissionsForUsersAsync(NewUsersPermissionsDTO userPermission)
    {
        var response = await HttpClient.PutAsJsonAsync($"{_options.ApiPath}/users/permissions", userPermission);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new BadHttpRequestException("Unable to save user permissions", (int)response.StatusCode);
        }

        var userPermissions = await response.Content.ReadFromJsonAsync<OrigoUsersPermissions>();
        return userPermissions;
    }
    public async Task<IEnumerable<ProductGet>> GetOrderedModuleProductsByPartnerAndOrganizationAsync(Guid partnerId, Guid organizationId, bool includeTranslations)
    {
        var products = await _productCatalogServices.GetOrderedProductsByPartnerAndOrganizationAsync(partnerId, organizationId, includeTranslations);
        return products.Where(x => x.ProductTypeId == 2);
    }

}