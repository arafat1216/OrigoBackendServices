using Common.Extensions;
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
            _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.DatasyncUserId().ToString());
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task CreateEmployee_WithInvalidUserData_ShouldReturnBadRequest()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            var requestUri = $"api/v1/employeedatasync/{_customerId}/users";
            CreateEmployeeEvent newEmployeeEvent = new();

            // Act
            var response = await httpClient.PostAsJsonAsync(requestUri, newEmployeeEvent);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task CreateEmployee_WithValidUserData_ShouldReturnCreated()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            var requestUri = $"api/v1/employeedatasync/{_customerId}/users";
            CreateEmployeeEvent newEmployeeEvent = new()
            {
                FirstName = "Test",
                LastName = "Mctest",
                Email = "test.mctest@test.techstep.io",
                MobileNumber = "91234567"
            };

            // Act
            var response = await httpClient.PostAsJsonAsync(requestUri, newEmployeeEvent);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(response.StatusCode == HttpStatusCode.Created);
        }
    }
}
