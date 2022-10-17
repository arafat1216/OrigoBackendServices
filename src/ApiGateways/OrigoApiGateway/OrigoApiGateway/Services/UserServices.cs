#nullable enable
using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System.Text.Json;

namespace OrigoApiGateway.Services
{
    public class UserServices : IUserServices
    {
        public UserServices(
            ILogger<UserServices> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<UserConfiguration> options,
            IMapper mapper,
            IProductCatalogServices productCatalogServices
        )
        {
            _logger = logger;
            _mapper = mapper;
            _productCatalogServices = productCatalogServices;
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        private readonly ILogger<UserServices> _logger;
        private readonly IMapper _mapper;
        private readonly IProductCatalogServices _productCatalogServices;
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient HttpClient => _httpClientFactory.CreateClient("customerservices");
        private readonly UserConfiguration _options;

        public async Task<CustomerUserCount> GetUsersCountAsync(Guid customerId, FilterOptionsForUser filterOptions)
        {
            try
            {
                string json = JsonSerializer.Serialize(filterOptions);

                return await HttpClient.GetFromJsonAsync<CustomerUserCount>($"{_options.ApiPath}/{customerId}/users/count/?filterOptions={json}");


            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "GetUserAsync failed with JsonException. {0}", exception.Message);
                throw;
            }
            catch (BadHttpRequestException exception)
            {
                _logger.LogError(exception, "GetUserAsync failed with BadHttpRequestException. {0}", exception.Message);
                throw;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetUserAsync failed with HttpRequestException.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetUserAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> GetUserAsync(Guid customerId, Guid userId)
        {
            try
            {
                var user = await HttpClient.GetFromJsonAsync<UserDTO>($"{_options.ApiPath}/{customerId}/users/{userId}");
                return user != null ? _mapper.Map<OrigoUser>(user) : null;
            }
            catch (HttpRequestException exception)
            {
                // Handle this special case by writing id of user instead of users name in auditlog
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                _logger.LogError(exception, "GetUserAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetUserAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetUserAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoMeUser?> GetUserWithPermissionsAsync(Guid? customerId, Guid mainOrganizationId, Guid userId, List<string> permissions,
            List<string> accessList)
        {
            try
            {
                var user = await HttpClient.GetFromJsonAsync<UserDTO>($"{_options.ApiPath}/users/{userId}");

                if (user == null)
                {
                    return null;
                }
                var meUser = _mapper.Map<OrigoMeUser>(user);
                meUser.AccessList.AddRange(accessList);
                meUser.OrganizationId = customerId ?? mainOrganizationId;
                if (customerId != null)
                {
                    var productPermissions = await _productCatalogServices.GetProductPermissionsForOrganizationAsync(customerId.Value);
                    var productPermissionsMainOrganization = await _productCatalogServices.GetProductPermissionsForOrganizationAsync(mainOrganizationId);
                    var permissionsWithoutProductPermissions = permissions.Except(productPermissionsMainOrganization);
                    permissions = permissionsWithoutProductPermissions.Concat(productPermissions).ToList();
                }
                meUser.PermissionNames.AddRange(permissions);

                return meUser;
            }
            catch (HttpRequestException exception)
            {
                // Handle this special case by writing id of user instead of users name in auditlog
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                _logger.LogError(exception, "GetUserAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetUserAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetUserAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> InitiateOffboarding(Guid customerId, Guid userId, string role, List<Guid> departments, OffboardInitiate offboardDate, IList<LifeCycleSetting> lifeCycleSettings, Guid callerId)
        {
            try
            {
                var user = await HttpClient.GetFromJsonAsync<UserDTO>($"{_options.ApiPath}/{customerId}/users/{userId}");
                if (user == null)
                    throw new InvalidUserValueException();
                if (role == PredefinedRole.DepartmentManager.ToString() && !departments.Contains(user.AssignedToDepartment))
                    throw new UnauthorizedAccessException("Manager does not have access to this User!!!");

                var postDate = new OffboardInitiateDTO()
                {
                    LastWorkingDay = offboardDate.LastWorkingDay,
                    CallerId = callerId,
                    BuyoutAllowed = (lifeCycleSettings != null && lifeCycleSettings.Any(x => x.BuyoutAllowed)) ? true : false
                };

                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/users/{userId}/initiate-offboarding", postDate);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    throw new InvalidUserValueException(await response.Content.ReadAsStringAsync());
                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    if (errorDescription.Contains("Okta"))
                        throw new OktaException(errorDescription, _logger);
                    else
                        throw new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                }

                var updatedUser = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
            }
            catch (HttpRequestException exception)
            {
                // Handle this special case by writing id of user instead of users name in auditlog
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                _logger.LogError(exception, "InitiateOffboarding failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "InitiateOffboarding failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "InitiateOffboarding unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> CancelOffboarding(Guid customerId, Guid userId, string role, List<Guid> departments, Guid callerId)
        {
            try
            {
                var user = await HttpClient.GetFromJsonAsync<UserDTO>($"{_options.ApiPath}/{customerId}/users/{userId}");
                if (user == null)
                    throw new InvalidUserValueException();
                if (role == PredefinedRole.DepartmentManager.ToString() && !departments.Contains(user.AssignedToDepartment))
                    throw new UnauthorizedAccessException("Manager does not have access to this User!!!");

                var response = await HttpClient.PostAsync($"{_options.ApiPath}/{customerId}/users/{userId}/{callerId}/cancel-offboarding", null);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    throw new InvalidUserValueException(await response.Content.ReadAsStringAsync());
                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    if (errorDescription.Contains("Okta"))
                        throw new OktaException(errorDescription, _logger);
                    else
                        throw new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                }

                var updatedUser = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
            }
            catch (HttpRequestException exception)
            {
                // Handle this special case by writing id of user instead of users name in auditlog
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                _logger.LogError(exception, "CancelOffboarding failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "CancelOffboarding failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "CancelOffboarding unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> GetUserAsync(Guid userId)
        {
            try
            {
                var user = await HttpClient.GetFromJsonAsync<UserDTO>($"{_options.ApiPath}/users/{userId}");
                return user != null ? _mapper.Map<OrigoUser>(user) : null;
            }
            catch (HttpRequestException exception)
            {
                // Handle this special case by writing id of user instead of users name in auditlog
                if (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                _logger.LogError(exception, "GetUserAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetUserAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetUserAsync unknown error.");
                throw;
            }
        }

        public async Task<PagedModel<OrigoUser>> GetAllUsersAsync(Guid customerId, FilterOptionsForUser filterOptions, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 25)
        {
            try
            {

                var json = JsonSerializer.Serialize(filterOptions);
                var users = await HttpClient.GetFromJsonAsync<PagedModel<UserDTO>>($"{_options.ApiPath}/{customerId}/users?q={search}&page={page}&limit={limit}&filterOptions={json}", cancellationToken);
                if (users == null)
                {
                    return new PagedModel<OrigoUser>();
                }
                return new PagedModel<OrigoUser>
                {
                    Items = _mapper.Map<IList<OrigoUser>>(users.Items),
                    CurrentPage = users.CurrentPage,
                    PageSize = users.PageSize,
                    TotalItems = users.TotalItems,
                    TotalPages = users.TotalPages
                };
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAllUsersAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetAllUsersAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetAllUsersAsync unknown error.");
                throw;
            }
        }

        public async Task<HashSet<UserNamesDTO>> GetAllUsersNamesAsync(Guid customerId, CancellationToken cancellationToken)
        {
            try
            {
                var users = await HttpClient.GetFromJsonAsync<HashSet<UserNamesDTO>>($"{_options.ApiPath}/{customerId}/users/names", cancellationToken);
                return users ?? new HashSet<UserNamesDTO>();
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetAllUsersAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetAllUsersAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetAllUsersAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUser newUser, Guid callerId, bool includeOnboarding)
        {
            try
            {
                var newUserDTO = _mapper.Map<NewUserDTO>(newUser);
                newUserDTO.CallerId = callerId;
                newUserDTO.NeedsOnboarding = includeOnboarding;

                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/users", newUserDTO);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    throw new InvalidUserValueException(await response.Content.ReadAsStringAsync());
                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    if (errorDescription.Contains("Okta"))
                        throw new OktaException(errorDescription, _logger);
                    else
                        throw new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                }


                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
            }
            catch (InvalidUserValueException)
            {
                throw;
            }
            catch (BadHttpRequestException exception)
            {
                _logger.LogError(exception, "AddUserForCustomerAsync - " + exception.Message);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "AddUserForCustomerAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> PutUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser, Guid callerId)
        {
            try
            {
                var updateUserDTO = _mapper.Map<UpdateUserDTO>(updateUser);
                updateUserDTO.CallerId = callerId;
                var response = await HttpClient.PutAsJsonAsync($"{_options.ApiPath}/{customerId}/users/{userId}", updateUserDTO);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    throw new InvalidUserValueException(await response.Content.ReadAsStringAsync());
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save user changes", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
            }
            catch (InvalidUserValueException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "UpdateUserAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> PatchUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser, Guid callerId)
        {
            try
            {
                var updateUserDTO = _mapper.Map<UpdateUserDTO>(updateUser);
                updateUserDTO.CallerId = callerId;
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/users/{userId}", updateUserDTO);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    throw new InvalidUserValueException(await response.Content.ReadAsStringAsync());
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save user changes", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
            }
            catch (InvalidUserValueException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "UpdateUserAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> DeleteUserAsync(Guid customerId, Guid userId, bool softDelete, Guid callerId)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/{customerId}/users/{userId}?softDelete={softDelete}";

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = JsonContent.Create(callerId),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri, UriKind.Relative)
                };

                var response = await HttpClient.SendAsync(request);


                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new BadHttpRequestException("User not found", (int)response.StatusCode);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to delete user", (int)response.StatusCode);
                var dto = await response.Content.ReadFromJsonAsync<UserDTO>();
                return _mapper.Map<OrigoUser>(dto);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "DeleteUserAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> SetUserActiveStatusAsync(Guid customerId, Guid userId, bool isActive, Guid callerId)
        {
            try
            {
                var response = await HttpClient.PostAsync($"{_options.ApiPath}/{customerId}/users/{userId}/activate/{isActive}", JsonContent.Create(callerId));


                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException(await response.Content.ReadAsStringAsync(), (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to change user status.");
                throw;
            }
        }

        public async Task<OrigoUser> AssignUserToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            try
            {

                var requestUri = $"{_options.ApiPath}/{customerId}/users/{userId}/department/{departmentId}";
                var response = await HttpClient.PostAsync(requestUri, JsonContent.Create(callerId));
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to assign department.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to assign department.");
                    throw exception;
                }
                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to assign department.");
                throw;
            }
        }

        public async Task<OrigoUser> UnassignUserFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/{customerId}/users/{userId}/department/{departmentId}";

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = JsonContent.Create(callerId),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri, UriKind.Relative)
                };

                var response = await HttpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to remove assigned department.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to remove assigned department.");
                    throw exception;
                }
                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to remove assigned department.");
                throw;
            }
        }

        public async Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            const string UNABLE_TO_ASSIGN_MESSAGE = "Unable to assign manager to department.";
            try
            {
                var requestUri = $"{_options.ApiPath}/{customerId}/users/{userId}/department/{departmentId}/manager";
                var response = await HttpClient.PostAsync(requestUri, JsonContent.Create(callerId));
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException(UNABLE_TO_ASSIGN_MESSAGE, (int)response.StatusCode);
                    _logger.LogError(exception, UNABLE_TO_ASSIGN_MESSAGE);
                    throw exception;
                }
                return;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, UNABLE_TO_ASSIGN_MESSAGE);
                throw;
            }
        }

        public async Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            const string UNABLE_TO_REMOVE_MANAGER_MESSAGE = "Unable to remove assignment of manager from department.";
            try
            {

                var requestUri = $"{_options.ApiPath}/{customerId}/users/{userId}/department/{departmentId}/manager";
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = JsonContent.Create(callerId),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri, UriKind.Relative)
                };

                var response = await HttpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException(UNABLE_TO_REMOVE_MANAGER_MESSAGE, (int)response.StatusCode);
                    _logger.LogError(exception, UNABLE_TO_REMOVE_MANAGER_MESSAGE);
                    throw exception;
                }
                return;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, UNABLE_TO_REMOVE_MANAGER_MESSAGE);
                throw;
            }
        }
        public async Task<UserInfoDTO> GetUserInfo(string userName, Guid userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userName) && userId == Guid.Empty) return new UserInfoDTO();

                var response = !string.IsNullOrEmpty(userName) ?
                    await HttpClient.GetFromJsonAsync<UserInfoDTO>($"{_options.ApiPath}/{userName}/users-info") :
                    await HttpClient.GetFromJsonAsync<UserInfoDTO>($"{_options.ApiPath}/{userId}/users-info");

                return response;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetUserInfo failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<OrigoExceptionMessages> ResendOrigoInvitationMail(Guid customerId, InviteUsers users, FilterOptionsForUser filterOptions)
        {
            try
            {

                string json = JsonSerializer.Serialize(filterOptions);

                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/users/re-send-invitation/?filterOptions={json}", users);
                var exceptionMessages = await response.Content.ReadFromJsonAsync<OrigoExceptionMessages>();
                return exceptionMessages;

            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "ResendOrigoInvitationMail failed with HttpRequestException.");
                throw;
            }
        }

        public async Task<OrigoUser> CompleteOnboardingAsync(Guid customerId, Guid userId)
        {
            try
            {

                var response = await HttpClient.PostAsync($"{_options.ApiPath}/{customerId}/users/{userId}/onboarding-completed", null);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    var exception = new BadHttpRequestException(error, (int)response.StatusCode);
                    _logger.LogError(exception, error);
                    throw exception;
                }

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return _mapper.Map<OrigoUser>(user);

            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "ActivateOnboardedUser failed with HttpRequestException.");
                throw;
            }
        }


        public async Task<(bool correctOrganization, Guid userId)> UserEmailLinkedToGivenOrganization(Guid organizationId, string userEmail)
        {
            var user = await GetUserInfo(userEmail, Guid.Empty);
            if (string.IsNullOrEmpty(user.UserName))
            {
                return (correctOrganization: true, userId: Guid.Empty);
            }

            return user.OrganizationId == organizationId ? (correctOrganization: true, userId: user.UserId) : (correctOrganization: false, userId: Guid.Empty);
        }
    }
}
