using Customer.API.Controllers;
using Customer.API.Tests;
using Customer.API.WriteModels;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
//[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
//[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Customer.API.IntegrationTests.Controllers
{
    public class DepartmentsControllerTests : IClassFixture<CustomerWebApplicationFactory<DepartmentsController>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly Guid _organizationId;
        private readonly Guid _departmentId;


        private readonly CustomerWebApplicationFactory<DepartmentsController> _factory;

        public DepartmentsControllerTests(CustomerWebApplicationFactory<DepartmentsController> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateClient();
            //new WebApplicationFactoryClientOptions
            //{
            //    BaseAddress = new Uri($"/api/v1/organizations/")
            //});
            _organizationId = Extention.ORGANIZATION_ID;
            _departmentId = Extention.HEAD_DEPARTMENT_ID;
            _factory = factory;
        }

        [Fact]
        public async Task GetDepartments()
        {

            var response = await _httpClient.GetAsync(
                $"/api/v1/organizations/{_organizationId}/departments");

            var read = await response.Content.ReadFromJsonAsync<List<ViewModels.Department>>();

            _testOutputHelper.WriteLine(read?[0].Name);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task CreateDepartment()
        {
            var newDepartment = new NewDepartment
            {
                Name = "Department",
                CostCenterId = "CostCenter123",
                Description = "Description",
                CallerId = Guid.NewGuid()

            };
            var response = await _httpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_organizationId}/departments", newDepartment);
            var createdDepartment = await response.Content.ReadFromJsonAsync<ViewModels.Department>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(newDepartment.Name, createdDepartment?.Name);
            Assert.Equal(newDepartment.CostCenterId, createdDepartment?.CostCenterId);
            Assert.Equal(newDepartment.Description, createdDepartment?.Description);
            Assert.Null(createdDepartment?.ParentDepartmentId);
            Assert.NotNull(createdDepartment?.DepartmentId);
        }

        [Fact]
        public async Task UpdateDepartmentPut()
        {
            var department = new ViewModels.Department
            {
                DepartmentId = _departmentId,
                Name = "Department",
                CostCenterId = "CostCenter123",
                Description = "Description",
                CallerId = Guid.NewGuid()
            };

            var response = await _httpClient.PutAsJsonAsync(
                $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}", department);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task UpdateDepartmentPatch()
        {

            var department = new ViewModels.Department
            {
                DepartmentId = _departmentId,
                Name = "Department",
                CostCenterId = "CostCenter123",
                Description = "Description",
                CallerId = Guid.NewGuid()
                
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}", department);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task DeleteDepartment()
        {

            var requestUri = $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}";

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = JsonContent.Create(Guid.NewGuid()),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };

            var response = await _httpClient.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseGet = await _httpClient.GetAsync(
               $"/api/v1/organizations/{_organizationId}/departments/{_departmentId}");

            Assert.Equal(HttpStatusCode.NotFound, responseGet.StatusCode);
        }
        [Fact]
        public async Task GetDepartment()
        {
            var newDepartment = new NewDepartment
            {
                Name = "Department",
                CostCenterId = "CostCenter123",
                Description = "Description",
                CallerId = Guid.NewGuid(),
                ParentDepartmentId = _organizationId
            };

            var response1 = await _httpClient.PostAsJsonAsync(
                        $"/api/v1/organizations/{_organizationId}/departments", newDepartment);
            var createdDepartment = await response1.Content.ReadFromJsonAsync<ViewModels.Department>();

            var response = await _httpClient.GetAsync(
                $"/api/v1/organizations/{_organizationId}/departments/{createdDepartment?.DepartmentId.ToString()}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


    }
}