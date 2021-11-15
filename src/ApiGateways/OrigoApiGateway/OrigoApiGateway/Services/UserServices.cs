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

namespace OrigoApiGateway.Services
{
    public class UserServices : IUserServices
    {
        public UserServices(ILogger<UserServices> logger, HttpClient httpClient,
            IOptions<UserConfiguration> options)
        {
            _logger = logger;
            HttpClient = httpClient;
            _options = options.Value;
        }

        private readonly ILogger<UserServices> _logger;
        private HttpClient HttpClient { get; }
        private readonly UserConfiguration _options;

        public async Task<OrigoUser> GetUserAsync(Guid customerId, Guid userId)
        {
            try
            {
                var user = await HttpClient.GetFromJsonAsync<UserDTO>($"{_options.ApiPath}/{customerId}/users/{userId}");
                return user != null ? new OrigoUser(user) : null;
            }
            catch (HttpRequestException exception)
            {
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
                return users?.Select(user => new OrigoUser(user)).ToList();
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

        public async Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUser newUser)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/users", newUser);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save user", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : new OrigoUser(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "AddUserForCustomerAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> PutUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser)
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync($"{_options.ApiPath}/{customerId}/users/{userId}", updateUser);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save user changes", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : new OrigoUser(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "UpdateUserAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoUser> PatchUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/users/{userId}", updateUser);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save user changes", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : new OrigoUser(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "UpdateUserAsync unknown error.");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid customerId, Guid userId, bool softDelete)
        {
            try
            {
                var response = await HttpClient.DeleteAsync($"{_options.ApiPath}/{customerId}/users/{userId}?softDelete={softDelete}");
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

        public async Task<OrigoUser> SetUserActiveStatusAsync(Guid customerId, Guid userId, bool isActive)
        {
            try
            {
                var response = await HttpClient.PostAsync($"{_options.ApiPath}/{customerId}/users/{userId}/activate/{isActive}", null);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to change active status on user.", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : new OrigoUser(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to deactivate user.");
                throw;
            }
        }

        public async Task<OrigoUser> AssignUserToDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            try
            {
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{customerId}/users/{userId}/department/{departmentId}";
                var response = await HttpClient.PostAsync(requestUri, emptyStringBodyContent);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to assign department.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to assign department.");
                    throw exception;
                }
                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : new OrigoUser(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to assign department.");
                throw;
            }
        }

        public async Task<OrigoUser> UnassignUserFromDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/{customerId}/users/{userId}/department/{departmentId}";
                var response = await HttpClient.DeleteAsync(requestUri);
                if (!response.IsSuccessStatusCode)
                {
                    var exception = new BadHttpRequestException("Unable to remove assigned department.", (int)response.StatusCode);
                    _logger.LogError(exception, "Unable to remove assigned department.");
                    throw exception;
                }
                var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                return user == null ? null : new OrigoUser(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to remove assigned department.");
                throw;
            }
        }

        public async Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            const string UNABLE_TO_ASSIGN_MESSAGE = "Unable to assign manager to department.";
            try
            {
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{customerId}/users/{userId}/department/{departmentId}/manager";
                var response = await HttpClient.PostAsync(requestUri, emptyStringBodyContent);
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

        public async Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            const string UNABLE_TO_REMOVE_MANAGER_MESSAGE = "Unable to remove assignment of manager from department.";
            try
            {
                var emptyStringBodyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var requestUri = $"{_options.ApiPath}/{customerId}/users/{userId}/department/{departmentId}/manager";
                var response = await HttpClient.DeleteAsync(requestUri);
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
