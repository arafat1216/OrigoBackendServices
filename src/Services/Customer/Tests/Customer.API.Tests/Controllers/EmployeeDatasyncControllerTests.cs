using Common.Extensions;
using Common.Interfaces;
using Common.Model.EventModels;
using Common.Model.EventModels.DatasyncModels;
using Customer.API.IntegrationTests.Helpers;
using Customer.API.Tests;
using Customer.API.WriteModels;
using Google.Rpc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CustomerServices.ServiceModels;

namespace Customer.API.IntegrationTests.Controllers
{
    public class EmployeeDatasyncControllerTests : IClassFixture<CustomerWebApplicationFactory<Startup>>
    {

        private readonly HttpClient _httpClient;
        private readonly Guid _customerId;

        private readonly CustomerWebApplicationFactory<Startup> _factory;

        public EmployeeDatasyncControllerTests(CustomerWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
        {
            _httpClient = factory.CreateDefaultClient();
            _customerId = factory.ORGANIZATION_ID;
            _factory = factory;
            _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.PubsubUserId().ToString());
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task CreateEmployee_WithInvalidUserData_ShouldReturnBadRequest()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            var customerId = _factory.ORGANIZATION_ID;
            var requestUri = $"api/v1/employee-datasync/organizations/{customerId}/users/create-employee";
            CreateEmployeeEvent newEmployeeEvent = new();

            // Act
            var response = await httpClient.PostAsJsonAsync(requestUri, newEmployeeEvent);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task CreateEmployee_WithValidUserData_FetchNewEmployee()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            var newEmployeeEmail = "test.mctest@test.techstep.io";
            CreateEmployeeEvent newEmployeeEvent = new()
            {
                FirstName = "Test",
                LastName = "Mctest",
                Email = newEmployeeEmail,
                MobileNumber = "91234567",
                CustomerId = _customerId
            };
            
            var customerId = _factory.ORGANIZATION_ID;
            var requestUri = $"api/v1/employee-datasync/organizations/{customerId}/users";

            // Act
            await httpClient.PostAsJsonAsync($"{requestUri}/create-employee", newEmployeeEvent);

            // Assert

            // Fetch newly created employee
            var response = await httpClient.GetAsync($"{requestUri}?q={newEmployeeEmail}");
            var read = await response.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(newEmployeeEmail, read!.Items[0].Email);
        }
    }
}
