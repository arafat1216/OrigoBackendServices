﻿using AutoMapper;
using Common.Enums;
using Common.Exceptions;
using Common.Interfaces;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.TechstepCore;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

namespace OrigoApiGateway.Services;

public class CustomerServices : ICustomerServices
{
    public CustomerServices(ILogger<CustomerServices> logger, IHttpClientFactory httpClientFactory,
        IOptions<CustomerConfiguration> options, IMapper mapper)
    {
        _logger = logger;
        _options = options.Value;
        _mapper = mapper;
        _httpClientFactory = httpClientFactory;
    }

    private readonly ILogger<CustomerServices> _logger;
    private HttpClient HttpClient => _httpClientFactory.CreateClient("customerservices");
    private readonly CustomerConfiguration _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMapper _mapper;

    public async Task<IList<Organization>> GetCustomersAsync(Guid? partnerId = null, bool includePreferences = true)
    {
        try
        {
            bool customersOnly = true;
            var organizations = await HttpClient.GetFromJsonAsync<IList<Organization>>($"{_options.ApiPath}/{customersOnly}/?partnerId={partnerId}&includePreferences={includePreferences}");

            return organizations ?? null;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "GetCustomersAsync failed with HttpRequestException.");
            throw;
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "GetCustomersAsync failed with content type is not valid.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "GetCustomersAsync unknown error.");
            throw;
        }
    }

    public async Task<PagedModel<Organization>> GetPaginatedCustomersAsync(CancellationToken cancellationToken, int page, int limit, Guid? partnerId = null, string? search = null, bool includePreferences = false)
    {
        try
        {
            bool customersOnly = true;
            bool hierarchical = false;

            // Build the conditional URL string w/query parameters
            StringBuilder requestUrl = new($"{_options.ApiPath}?page={page}&limit={limit}&customersOnly={customersOnly}");
            if (hierarchical)
                requestUrl.Append($"&hierarchical={hierarchical}");
            if (includePreferences)
                requestUrl.Append($"&includePreferences={includePreferences}");
            if (partnerId is not null)
                requestUrl.Append($"&partnerId={partnerId}");
            if (search is not null)
                requestUrl.Append($"&q={HttpUtility.UrlEncode(search)}");

            var organizations = await HttpClient.GetFromJsonAsync<PagedModel<Organization>>(requestUrl.ToString(), cancellationToken);

            return organizations ?? null;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "GetCustomersAsync failed with HttpRequestException.");
            throw;
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "GetCustomersAsync failed with content type is not valid.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "GetCustomersAsync unknown error.");
            throw;
        }
    }

    public async Task<Organization> GetCustomerAsync(Guid customerId, bool includeDepartments = true, bool includeOrganizationPreferences = true, bool includeLocation = true, bool includePartner = true)
    {
        try
        {
            bool customerOnly = true;
            var organization = await HttpClient.GetFromJsonAsync<Organization>($"{_options.ApiPath}/{customerId}/{customerOnly}?includeDepartments={includeDepartments}&includeOrganizationPreferences={includeOrganizationPreferences}&includeLocation={includeLocation}&includePartner={includePartner}");

            return organization ?? null;
        }
        catch (HttpRequestException exception)
        {
            if (exception.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogError(exception, "GetCustomerAsync failed with HttpRequestException.");
            throw;
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "GetCustomerAsync failed with content type is not valid.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "GetCustomerAsync unknown error.");
            throw;
        }
    }

    public async Task<IList<CustomerUserCount>> GetCustomerUsersAsync(FilterOptionsForUser filterOptions)
    {
        try
        {
            var json = JsonSerializer.Serialize(filterOptions);
            var customerUserCounts = await HttpClient.GetFromJsonAsync<IList<CustomerUserCount>>($"{_options.ApiPath}/userCount?filterOptions={json}");
            return customerUserCounts == null ? null : _mapper.Map<IList<CustomerUserCount>>(customerUserCounts);
        }
        catch (HttpRequestException exception)
        {
            // Not found
            if (exception.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogError(exception, "GetCustomerUsersAsync failed with HttpRequestException.");
            throw;
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "GetCustomerUsersAsync failed with content type is not valid.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "GetCustomerUsersAsync unknown error.");
            throw;
        }
    }
    public async Task<IList<Location>> GetAllCustomerLocations(Guid customerId)
    {
        try
        {
            var locations = await HttpClient.GetFromJsonAsync<IList<LocationDTO>>($"{_options.ApiPath}/{customerId}/location");
            return locations == null ? null : _mapper.Map<IList<Location>>(locations);
        }
        catch (HttpRequestException exception)
        {
            if (exception.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogError(exception, "GetAllCustomerLocations failed with HttpRequestException.");
            throw;
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "GetAllCustomerLocations failed with content type is not valid.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "GetAllCustomerLocations unknown error.");
            throw;
        }
    }
    public async Task<Location> CreateLocationAsync(OfficeLocation officeLocation, Guid customerId, Guid callerId)
    {
        try
        {
            var newLocation = _mapper.Map<OfficeLocationDTO>(officeLocation);
            newLocation.CallerId = callerId;

            var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/location", newLocation);

            if (!response.IsSuccessStatusCode)
                throw new BadHttpRequestException($"Unable to save Location: {await response.Content.ReadAsStringAsync()}", (int)response.StatusCode);

            var location = await response.Content.ReadFromJsonAsync<LocationDTO>();
            return location == null ? null : _mapper.Map<Location>(location);
        }
        catch (HttpRequestException exception)
        {
            // Not found
            if (exception.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogError(exception, "CreateLocationAsync failed with HttpRequestException.");
            throw;
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "CreateLocationAsync failed with content type is not valid.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "CreateLocationAsync unknown error.");
            throw;
        }
    }
    public async Task<IList<Location>> DeleteLocationAsync(Guid customerId, Guid locationId, Guid callerId)
    {
        try
        {
            var delCont = new DeleteContent()
            {
                CallerId = callerId,
                hardDelete = true
            };

            var requestUri = $"{_options.ApiPath}/{locationId}/location";

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = JsonContent.Create(delCont),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };

            var response = await HttpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode == 404)
                    return null;
                var exception = new BadHttpRequestException("Unable to remove location.", (int)response.StatusCode);
                _logger.LogError(exception, "Unable to remove location.");
                throw exception;
            }

            return await GetAllCustomerLocations(customerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteLocationAsync unknown error.");
            throw;
        }
    }
    public async Task<Organization> InitiateOnbardingAsync(Guid customerId)
    {
        try
        {
            var requestUri = $"{_options.ApiPath}/{customerId}/initiate-onboarding";
            var response = await HttpClient.PostAsync(requestUri, null);

            if ((int)response.StatusCode == 404)
                return null;
            if ((int)response.StatusCode == 400)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new BadHttpRequestException(errorMessage);
            }

            var organization = await response.Content.ReadFromJsonAsync<Organization>();

            return organization;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InitiateOnbardingAsync unknown error.");
            throw;
        }

    }

    public async Task<Organization> CreateCustomerAsync(NewOrganization newCustomer, Guid callerId)
    {
        try
        {
            var newCustomerDTO = _mapper.Map<NewOrganizationDTO>(newCustomer);
            newCustomerDTO.CallerId = callerId;
            newCustomerDTO.IsCustomer = true;

            if (newCustomer.Location is null)
            {
                newCustomerDTO.Location = new NewLocation
                {
                    Name = "Default",
                    Description = null,
                    Address1 = newCustomer.Address.Street,
                    Address2 = null,
                    City = newCustomer.Address.City,
                    PostalCode = newCustomer.Address.Postcode,
                    Country = newCustomer.Address.Country
                };
            }

            var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}", newCustomerDTO);
            if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Conflict)
                throw new InvalidOrganizationNumberException(await response.Content.ReadAsStringAsync());
            else if (!response.IsSuccessStatusCode)
                throw new BadHttpRequestException("Unable to save customer", (int)response.StatusCode);

            var organization = await response.Content.ReadFromJsonAsync<Organization>();
            return organization ?? null;
        }
        catch (InvalidOrganizationNumberException exception)
        {
            _logger.LogError(exception, "CreateCustomerAsync failed with InvalidOrganizationNumberException.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "CreateCustomerAsync unknown error.");
            throw;
        }
    }
    public async Task<Organization> CreatePartnerOrganization(NewOrganization newOrganization, Guid callerId)
    {
        try
        {
            var newCustomerDTO = _mapper.Map<NewOrganizationDTO>(newOrganization);
            newCustomerDTO.CallerId = callerId;
            newCustomerDTO.IsCustomer = false;
            newCustomerDTO.ParentId = null;

            // Temp fix until we end up with one address input (instead of two)
            if (newOrganization.Location is null)
            {
                newCustomerDTO.Location = new NewLocation
                {
                    Name = "Default",
                    Description = null,
                    Address1 = newOrganization.Address.Street,
                    Address2 = null,
                    City = newOrganization.Address.City,
                    PostalCode = newOrganization.Address.Postcode,
                    Country = newOrganization.Address.Country
                };
            }

            var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}", newCustomerDTO);
            if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Conflict)
                throw new InvalidOrganizationNumberException(await response.Content.ReadAsStringAsync());
            if (!response.IsSuccessStatusCode)
                throw new BadHttpRequestException("Unable to save organization", (int)response.StatusCode);

            var organization = await response.Content.ReadFromJsonAsync<Organization>();
            return organization ?? null;
        }
        catch (InvalidOrganizationNumberException exception)
        {
            _logger.LogError(exception, "CreateOrganizationAsync failed with InvalidOrganizationNumberException.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "CreateOrganizationAsync unknown error.");
            throw;
        }
    }

    public async Task<Organization> DeleteOrganizationAsync(Guid organizationId, Guid callerId)
    {
        try
        {
            DeleteOrganization delOrg = new DeleteOrganization
            {
                OrganizationId = organizationId,
                CallerId = callerId,
                hardDelete = false
            };

            var requestUri = $"{_options.ApiPath}/{organizationId}";

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = JsonContent.Create(delOrg),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };

            var response = await HttpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode == 404)
                    return null;
                var exception = new BadHttpRequestException("Unable to remove organization.", (int)response.StatusCode);
                _logger.LogError(exception, "Unable to remove organization.");
                throw exception;
            }

            var organization = await response.Content.ReadFromJsonAsync<Organization>();
            return organization ?? null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateOrganizationAsync unknown error.");
            throw;
        }
    }

    public async Task<Organization> UpdateOrganizationAsync(UpdateOrganizationDTO organizationToChange)
    {
        try
        {
            if (!string.IsNullOrEmpty(organizationToChange.PayrollContactEmail))
            {
                if (!new EmailAddressAttribute().IsValid(organizationToChange.PayrollContactEmail))
                    throw new BadHttpRequestException("Invalid Email for 'PayrollContactEmail'", (int)HttpStatusCode.BadRequest);
            }

            if (organizationToChange.LastDayForReportingSalaryDeduction != null && (organizationToChange.LastDayForReportingSalaryDeduction < 1 || organizationToChange.LastDayForReportingSalaryDeduction > 28))
                throw new BadHttpRequestException("Invalid Input!! Valu must be within (1-28) for 'LastDayForReportingSalaryDeduction'", (int)HttpStatusCode.BadRequest);

            var response = await HttpClient.PutAsJsonAsync($"{_options.ApiPath}/{organizationToChange.OrganizationId}/organization", organizationToChange);
            if (!response.IsSuccessStatusCode)
                throw new BadHttpRequestException("Unable to update organization", (int)response.StatusCode);

            var organization = await response.Content.ReadFromJsonAsync<Organization>();
            return organization ?? null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateOrganizationAsync unknown error.");
            throw;
        }
    }

    public async Task<Organization> PatchOrganizationAsync(UpdateOrganizationDTO organizationToChange)
    {
        try
        {
            if (!string.IsNullOrEmpty(organizationToChange.PayrollContactEmail))
            {
                if (!new EmailAddressAttribute().IsValid(organizationToChange.PayrollContactEmail))
                    throw new BadHttpRequestException("Invalid Email for 'PayrollContactEmail'", (int)HttpStatusCode.BadRequest);
            }
            if (organizationToChange.LastDayForReportingSalaryDeduction != null && (organizationToChange.LastDayForReportingSalaryDeduction < 1 || organizationToChange.LastDayForReportingSalaryDeduction > 28))
                throw new BadHttpRequestException("Invalid Input!! Valu must be within (1-28) for 'LastDayForReportingSalaryDeduction'", (int)HttpStatusCode.BadRequest);

            var response = await HttpClient.PostAsync($"{_options.ApiPath}/{organizationToChange.OrganizationId}/organization", JsonContent.Create(organizationToChange));
            if (!response.IsSuccessStatusCode)
                throw new BadHttpRequestException("Unable to update organization", (int)response.StatusCode);

            var organization = await response.Content.ReadFromJsonAsync<Organization>();
            return organization ?? null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateOrganizationAsync unknown error.");
            throw;
        }
    }

    public async Task<string> CreateOrganizationSeedData()
    {
        try
        {
            var errorMessage = await HttpClient.GetStringAsync($"{_options.ApiPath}/seed");
            return errorMessage;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "CreateOrganizationSeedData failed with HttpRequestException.");
            throw;
        }
        catch (NotSupportedException exception)
        {
            _logger.LogError(exception, "CreateOrganizationSeedData failed with content type is not valid.");
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "CreateOrganizationSeedData unknown error.");
            throw;
        }
    }

    public async Task<string> GetOktaUserProfileByEmail(string email)
    {
        try
        {
            var result = await HttpClient.GetStringAsync($"{_options.ApiPath}/{email}/webshopUser");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOktaUserProfileByEmail error.");
            throw;
        }
    }

    public Task<bool> CheckAndProvisionWebShopUser(string email, string orgnumber)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetCurrencyByCustomer(Guid customerId)
    {
        var customer = await GetCustomerAsync(customerId);
        var country = customer.Location == null ?
            customer.Address == null ? null : customer.Address.Country
            : customer.Location.Country;

        if (string.IsNullOrEmpty(country)) return CurrencyCode.NOK.ToString();

        return country.ToUpper().Trim() switch
        {
            "NO" => CurrencyCode.NOK.ToString(),
            "SE" => CurrencyCode.SEK.ToString(),
            "DK" => CurrencyCode.DKK.ToString(),
            "US" => CurrencyCode.USD.ToString(),
            _ => CurrencyCode.EUR.ToString()
        };
    }

    public async Task UpdateCustomerFromTechstepCore(TechstepCoreCustomersData techstepCoreUpdate)
    {
        try
        {
            var response = await HttpClient.PostAsync($"{_options.ApiPath}/techstep-core-update", JsonContent.Create(techstepCoreUpdate));

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError(error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateCustomerFromTechstepCore error.");
            throw;
        }
    }

    public async Task<TechstepCustomers> GetTechstepCustomersAsync(string searchString, string countryCode)
    {
        if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(countryCode))
        {
            return new TechstepCustomers();
        }

        if (countryCode is not ("NO" or "SE"))
        {
            return new TechstepCustomers();
        }

        TechstepCoreCustomersData techstepCoreCustomersData;
        if (countryCode == "NO")
        {
            using var techstepCoreHttpClient = _httpClientFactory.CreateClient("techstep-core-customers-no");
            techstepCoreCustomersData = await techstepCoreHttpClient.GetFromJsonAsync<TechstepCoreCustomersData>($"?searchString={searchString}&pageSize=50");
        }
        else
        {
            using var techstepCoreHttpClient = _httpClientFactory.CreateClient("techstep-core-customers-se");
            techstepCoreCustomersData = await techstepCoreHttpClient.GetFromJsonAsync<TechstepCoreCustomersData>($"?searchString={searchString}&pageSize=50");
        }

        using var customerHttpClient = _httpClientFactory.CreateClient("customerservices");
        var techstepCustomers = await customerHttpClient.GetFromJsonAsync<IList<Organization>>($"{_options.ApiPath}/{true}/?partnerId={_options.TechstepPartnerId}");
        if (techstepCoreCustomersData == null)
        {
            return new TechstepCustomers();
        }

        var notImportedTechstepCustomers = techstepCustomers != null
            ? techstepCoreCustomersData.Data.Where(core => techstepCustomers.All(org => org.OrganizationNumber != core.OrgNumber)).ToList()
            : techstepCoreCustomersData.Data;

        return _mapper.Map<TechstepCustomers>(notImportedTechstepCustomers);
    }
}