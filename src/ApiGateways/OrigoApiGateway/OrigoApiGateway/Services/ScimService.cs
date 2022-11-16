using AutoMapper;
using Common.Interfaces;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.SCIM;

namespace OrigoApiGateway.Services;

public class ScimService : IScimService
{
    private readonly ILogger<ScimService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient HttpClient => _httpClientFactory.CreateClient("customerservices");
    private readonly ScimConfiguration _options;
    private readonly IMapper _mapper;

    public ScimService(
        ILogger<ScimService> logger, 
        IHttpClientFactory httpClientFactory, 
        IOptions<ScimConfiguration> options, 
        IMapper mapper)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _mapper = mapper;
    }


    public async Task<OrigoUser> GetUserAsync(Guid userId)
    {
        try
        {
            var user = await HttpClient.GetFromJsonAsync<UserDTO>($"{_options.ApiPath}/{userId}");
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


    public async Task<ListResponse<User>> GetAllUsersAsync(CancellationToken cancellationToken, string userName = "", int startIndex = 0, int limit = 25)
    {
        try
        {
            var users = await HttpClient.GetFromJsonAsync<PagedModel<UserDTO>>($"{_options.ApiPath}?userName={userName}&startIndex={startIndex}&limit={limit}", cancellationToken);

            var resources = _mapper.Map<List<User>>(users.Items);
            return new ListResponse<User>(resources)
            {
                StartIndex = ++startIndex,
                TotalResults = users.TotalItems,
                ItemsPerPage = users.Items.Count,
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
    
    public async Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUser newUser, Guid callerId, bool includeOnboarding)
    {
        try
        {
            var newUserDTO = _mapper.Map<NewUserDTO>(newUser);
            newUserDTO.CallerId = callerId;
            newUserDTO.NeedsOnboarding = includeOnboarding;

            var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/organizations/{customerId}", newUserDTO);
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
            var response = await HttpClient.PutAsJsonAsync($"{_options.ApiPath}/{userId}/organizations/{customerId}", updateUserDTO);
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


    public async Task<OrigoUser> DeleteUserAsync(Guid userId, bool softDelete, Guid callerId)
    {
        try
        {
            var requestUri = $"{_options.ApiPath}/{userId}?softDelete={softDelete}";

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
}