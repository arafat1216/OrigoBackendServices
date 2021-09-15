using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;

namespace OrigoApiGateway.Services
{
    public class DepartmentsServices : IDepartmentsServices
    {
        private readonly ILogger<DepartmentsServices> _logger;

        private readonly DepartmentConfiguration _options;

        private HttpClient HttpClient { get; }

        public DepartmentsServices(ILogger<DepartmentsServices> logger, HttpClient httpClient,
         IOptions<DepartmentConfiguration> options)
        {
            _logger = logger;
            HttpClient = httpClient;
            _options = options.Value;
        }

        public async Task<OrigoDepartment> GetDepartment(Guid customerId, Guid departmentId)
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<DepartmentDTO>($"{_options.ApiPath}/{customerId}/departments/{departmentId}");
                return response == null ? null : new OrigoDepartment(response);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetDepartment failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetDepartment failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetDepartment unknown error.");
                throw;
            }
        }

        public async Task<IList<OrigoDepartment>> GetDepartments(Guid customerId)
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<IList<DepartmentDTO>>($"{_options.ApiPath}/{customerId}/departments");
                return response?.Select(d => new OrigoDepartment(d)).ToList();
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetDepartments failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetDepartments failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetDepartments unknown error.");
                throw;
            }
        }

        public async Task<OrigoDepartment> AddDepartmentAsync(Guid customerId, NewDepartment department)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/departments", department);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save department", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<DepartmentDTO>();
                return user == null ? null : new OrigoDepartment(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "AddDepartmentAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoDepartment> UpdateDepartmentPutAsync(Guid customerId, Guid departmentId, OrigoDepartment department)
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync($"{_options.ApiPath}/{customerId}/departments/{departmentId}", department);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save department", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<DepartmentDTO>();
                return user == null ? null : new OrigoDepartment(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "UpdateDepartmentPutAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoDepartment> UpdateDepartmentPatchAsync(Guid customerId, Guid departmentId, OrigoDepartment department)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/departments/{departmentId}", department);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save department", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<DepartmentDTO>();
                return user == null ? null : new OrigoDepartment(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "UpdateDepartmentPatchAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoDepartment> DeleteDepartmentPatchAsync(Guid customerId, Guid departmentId)
        {
            try
            {
                var response = await HttpClient.DeleteAsync($"{_options.ApiPath}/{customerId}/departments/{departmentId}");
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save department", (int)response.StatusCode);

                var user = await response.Content.ReadFromJsonAsync<DepartmentDTO>();
                return user == null ? null : new OrigoDepartment(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "DeleteDepartmentPatchAsync unknown error.");
                throw;
            }
        }
    }
}
