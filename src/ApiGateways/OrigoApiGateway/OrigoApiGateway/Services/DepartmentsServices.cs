using AutoMapper;
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

namespace OrigoApiGateway.Services
{
    public class DepartmentsServices : IDepartmentsServices
    {
        private readonly ILogger<DepartmentsServices> _logger;
        private readonly IMapper _mapper;
        private readonly DepartmentConfiguration _options;

        private HttpClient HttpClient { get; }

        public DepartmentsServices(ILogger<DepartmentsServices> logger, HttpClient httpClient,
         IOptions<DepartmentConfiguration> options, IMapper mapper)
        {
            _logger = logger;
            HttpClient = httpClient;
            _options = options.Value;
            _mapper = mapper;
        }

        public async Task<OrigoDepartment> GetDepartment(Guid customerId, Guid departmentId)
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<DepartmentDTO>($"{_options.ApiPath}/{customerId}/departments/{departmentId}");
                return response == null ? null : _mapper.Map<OrigoDepartment>(response);
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
                return _mapper.Map<List<OrigoDepartment>>(response);
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

        public async Task<OrigoDepartment> AddDepartmentAsync(Guid customerId, NewDepartmentDTO department)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/departments", department);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save department", (int)response.StatusCode);

                var newDepartment = await response.Content.ReadFromJsonAsync<DepartmentDTO>();
                return newDepartment == null ? null : _mapper.Map<OrigoDepartment>(newDepartment);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "AddDepartmentAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoDepartment> UpdateDepartmentPutAsync(Guid customerId, Guid departmentId, UpdateDepartmentDTO department)
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync($"{_options.ApiPath}/{customerId}/departments/{departmentId}", department);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save department", (int)response.StatusCode);

                var updatedDepartment = await response.Content.ReadFromJsonAsync<DepartmentDTO>();
                return updatedDepartment == null ? null : _mapper.Map<OrigoDepartment>(updatedDepartment);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "UpdateDepartmentPutAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoDepartment> UpdateDepartmentPatchAsync(Guid customerId, Guid departmentId, UpdateDepartmentDTO department)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}/{customerId}/departments/{departmentId}", department);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save department", (int)response.StatusCode);

                var updatedDepartment = await response.Content.ReadFromJsonAsync<DepartmentDTO>();
                return updatedDepartment == null ? null : _mapper.Map<OrigoDepartment>(updatedDepartment);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "UpdateDepartmentPatchAsync unknown error.");
                throw;
            }
        }

        public async Task<OrigoDepartment> DeleteDepartmentPatchAsync(Guid customerId, Guid departmentId,Guid callerId)
        {
            try
            {
                var requestUri = $"{_options.ApiPath}/{customerId}/departments/{departmentId}";

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = JsonContent.Create(callerId),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri, UriKind.Relative)
                };

                var response = await HttpClient.SendAsync(request);

                //var response = await HttpClient.DeleteAsync($"{_options.ApiPath}/{customerId}/departments/{departmentId}");
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save department", (int)response.StatusCode);

                var deletedDepartment = await response.Content.ReadFromJsonAsync<DepartmentDTO>();
                return deletedDepartment == null ? null : _mapper.Map<OrigoDepartment>(deletedDepartment);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "DeleteDepartmentPatchAsync unknown error.");
                throw;
            }
        }
    }
}
