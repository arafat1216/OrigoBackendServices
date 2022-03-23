using AutoMapper;
using Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

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
                return count;
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

        public async Task<PagedModel<OrigoUser>> GetAllUsersAsync(Guid customerId, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 1000)
        {
            try
            {
                var users = await HttpClient.GetFromJsonAsync<PagedModel<UserDTO>>($"{_options.ApiPath}/{customerId}/users?q={search}&page={page}&limit={limit}");

                //return _mapper.Map<List<OrigoUser>>(users);
                return new PagedModel<OrigoUser>
                {
                    Items = _mapper.Map<IList<OrigoUser>>(users.Items),
                    CurrentPage = users.CurrentPage,
                    PageSize = users.PageSize,
                    TotalItems = users.TotalItems
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

        public async Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUser newUser, Guid callerId)
        {
            try
            {
                var newUserDTO = _mapper.Map<NewUserDTO>(newUser);
                newUserDTO.CallerId = callerId;
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
