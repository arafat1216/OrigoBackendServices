using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace OrigoApiGateway.Services
{
    public class UserServices : IUserServices
    {
        public UserServices(ILogger<UserServices> logger, HttpClient httpClient, IOptions<UserConfiguration> options, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            HttpClient = httpClient;
            _options = options.Value;
        }

        private readonly ILogger<UserServices> _logger;
        private readonly IMapper _mapper;
        private HttpClient HttpClient { get; }
        private readonly UserConfiguration _options;

        public async Task<int> GetUsersCountAsync(Guid customerId)
        {
            try
            {
                var count = await HttpClient.GetFromJsonAsync<int>($"{_options.ApiPath}/{customerId}/users/count");
                return count ;
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
                return user != null ?  _mapper.Map<OrigoUser>(user) : null;
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

        public async Task<IEnumerable<OrigoUser>> GetAllUsersAsync(Guid customerId, IReadOnlyCollection<Guid> filteredDepartmentList = null)
        {
            try
            {
                var users = await HttpClient.GetFromJsonAsync<IList<UserDTO>>($"{_options.ApiPath}/{customerId}/users");
                //if (filteredDepartmentList != null)
                //{

                //}
                //foreach (var item in users)
                //{
                //    item.AssignedToDepartments
                //}
                return _mapper.Map<List<OrigoUser>>(users);
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

        public async Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUserDTO newUser)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/users", newUser);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                if (!response.IsSuccessStatusCode)
                {
                    var errorDescription = await response.Content.ReadAsStringAsync();
                    throw new BadHttpRequestException(errorDescription, (int)response.StatusCode);
                }
                    

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
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

        public async Task<OrigoUser> PutUserAsync(Guid customerId, Guid userId, UpdateUserDTO updateUser)
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync($"{_options.ApiPath}/{customerId}/users/{userId}", updateUser);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save user changes", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "UpdateUserAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> PatchUserAsync(Guid customerId, Guid userId, UpdateUserDTO updateUser)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/users/{userId}", updateUser);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save user changes", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "UpdateUserAsync unknown error.");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid customerId, Guid userId, bool softDelete, Guid callerId)
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
                    return false;
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to delete user", (int)response.StatusCode);

                return true;
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
                    throw new BadHttpRequestException("Unable to change active status on user.", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : _mapper.Map<OrigoUser>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to deactivate user.");
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
    }
}
