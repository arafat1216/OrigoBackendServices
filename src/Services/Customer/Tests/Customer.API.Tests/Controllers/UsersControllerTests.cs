using Common.Interfaces;
using Customer.API.Controllers;
using Customer.API.Tests;
using CustomerServices.ServiceModels;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Customer.API.IntegrationTests.Controllers
{
    public class UsersControllerTests : IClassFixture<CustomerWebApplicationFactory<UsersController>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly Guid _customerId;


        public UsersControllerTests(CustomerWebApplicationFactory<UsersController> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
            _customerId = Extention.ORGANIZATION_ID;
        }

        [Fact]
        public async Task GetUsers()
        {

            // Setup
            var search = "";
            var page = 1;
            var limit = 1000;
            var requestUri = $"/api/v1/organizations/{_customerId}/users?q={search}&page={page}&limit={limit}";

            var response = await _httpClient.GetAsync(requestUri);
            var read = response.Content.ReadFromJsonAsync<PagedModel<UserDTO>>();
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Empty(read?.Result?.Items);
        }
    }
}
