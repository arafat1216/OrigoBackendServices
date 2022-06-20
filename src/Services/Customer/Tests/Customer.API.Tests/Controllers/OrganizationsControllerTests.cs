using Xunit;
using Customer.API.Tests;
using System.Net.Http;
using System;
using Xunit.Abstractions;
using System.Net.Http.Json;
using System.Text.Json;
using CustomerServices.ServiceModels;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using Customer.API.WriteModels;
using System.Linq;
using Customer.API.IntegrationTests.Helpers;
using Customer.API.ViewModels;
//Customer.API.IntegrationTests.Controllers
namespace Customer.API.IntegrationTests.Controllers
{
    public class OrganizationsControllerTests : IClassFixture<CustomerWebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly Guid _organizationId;

        private readonly CustomerWebApplicationFactory<Startup> _factory;

        public OrganizationsControllerTests(CustomerWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
            _factory = factory;
            _organizationId = factory.ORGANIZATION_ID;
        }


        [Fact()]
        public async Task CreateOrganizationTest()
        {
            // Setup
            var requestUri = $"/api/v1/organizations";
            _testOutputHelper.WriteLine(requestUri);

            var newOrganization = new NewOrganizationDTO
            {
                Name = "TE BØ",
                OrganizationNumber = "919724617",
                Address = new AddressDTO
                {
                    City = "OSLO",
                    Country = "NO",
                    PostCode = "0554",
                    Street = "Markveien 32F"
                },
                ContactPerson = new ContactPersonDTO
                {
                    Email = "test@test.test",
                    FirstName = "test",
                    LastName = "test",
                    PhoneNumber = "+4790909090"
                },
                Location = new LocationDTO
                {
                    Name = "Default location",
                    Description = null,
                    Address1 = "Markveien 32F",
                    Address2 = null,
                    City = "OSLO",
                    PostalCode = "0554",
                    Country = "NO"
                },
                PrimaryLocation = null,
                ParentId = null,
                CallerId = Guid.Parse("fd93b1f9-3df8-4823-9215-306135992d25"),
                InternalNotes = null,
                IsCustomer = true,
                Preferences = null
            };

            // Do the request
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(newOrganization));
            var response = await _httpClient.PostAsJsonAsync(requestUri, newOrganization);
            var responseMessage = await response.Content.ReadAsStringAsync();

            // Check asserts
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetAllLocationInOrganization()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
            var requestUri = $"/api/v1/organizations/{_organizationId}/location";
            _testOutputHelper.WriteLine(requestUri);
            var locations = await httpClient.GetFromJsonAsync<IList<LocationDTO>>(requestUri);
            Assert.Equal(1, locations!.Count);
        }
        [Fact]
        public async Task DeleteLocationAsync()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
            var requestUri = $"/api/v1/organizations/{_organizationId}/location";
            _testOutputHelper.WriteLine(requestUri);
            var locations = await httpClient.GetFromJsonAsync<IList<LocationDTO>>(requestUri);
            var selectedLocation = locations!.FirstOrDefault();
            

            var delCont = new DeleteContent()
            {
                CallerId = Guid.Empty,
                hardDelete = true
            };

            requestUri = $"/api/v1/organizations/{selectedLocation!.Id}/location";
            _testOutputHelper.WriteLine(requestUri);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = JsonContent.Create(delCont),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };

            var deleteResponse = await httpClient.SendAsync(request);

            requestUri = $"/api/v1/organizations/{_organizationId}/location";
            _testOutputHelper.WriteLine(requestUri);
            var newLocations = await httpClient.GetFromJsonAsync<IList<LocationDTO>>(requestUri);

            // Assert
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            Assert.Equal(1, locations!.Count);
            Assert.Equal(0, newLocations!.Count);
            Assert.DoesNotContain(newLocations, x =>x.Id == selectedLocation.Id);
        }

        [Fact]
        public async Task CreateLocationInOrganization_IsNULL()
        {
            // Arrange
            var newLocation = new NewLocation();

            // Act
            var requestUri = $"/api/v1/organizations/{_organizationId}/location";
            _testOutputHelper.WriteLine(requestUri);
            var createResponse = await _httpClient.PostAsJsonAsync(requestUri, newLocation);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
        }

        [Fact]
        public async Task CreateLocationInOrganization_IsNotPrimary()
        {
            // Arrange
            var newLocation = new NewLocation()
            {
                Name = "Test",
                Description = "test",
                PostalCode = "0000",
                City = "Oslo",
                Country = "NO"
            };
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            // Act
            var requestUri = $"/api/v1/organizations/{_organizationId}/location";
            _testOutputHelper.WriteLine(requestUri);
            var createResponse = await httpClient.PostAsJsonAsync(requestUri, newLocation);
            var returnedLocation = await createResponse.Content.ReadFromJsonAsync<LocationDTO>();
            var getRequestUri = $"/api/v1/organizations/{_organizationId}/false";
            _testOutputHelper.WriteLine(getRequestUri);
            var Org = await httpClient.GetFromJsonAsync<OrganizationDTO>(getRequestUri);
            var locationRequestUri = $"/api/v1/organizations/{_organizationId}/location";
            _testOutputHelper.WriteLine(locationRequestUri);
            var locations = await httpClient.GetFromJsonAsync<IList<LocationDTO>>(locationRequestUri);

            // Assert
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            Assert.True(Org!.Location.Name != newLocation.Name);
            Assert.True(locations!.Count == 2);
            Assert.True(!returnedLocation!.IsPrimary);
            Assert.True(returnedLocation!.Id != Org!.Location.Id);
        }

        [Fact]
        public async Task CreateLocationInOrganization_IsPrimary()
        {
            // Arrange
            var newLocation = new NewLocation()
            {
                Name = "Test",
                Description = "test",
                PostalCode = "0000",
                City = "Oslo",
                Country = "NO",
                IsPrimary = true
            };
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            // Act
            var requestUri = $"/api/v1/organizations/{_organizationId}/location";
            _testOutputHelper.WriteLine(requestUri);
            var createResponse = await httpClient.PostAsJsonAsync(requestUri, newLocation);
            var returnedLocation = await createResponse.Content.ReadFromJsonAsync<LocationDTO>();
            var getRequestUri = $"/api/v1/organizations/{_organizationId}/false";
            _testOutputHelper.WriteLine(getRequestUri);
            var Org = await httpClient.GetFromJsonAsync<OrganizationDTO>(getRequestUri);
            var locationRequestUri = $"/api/v1/organizations/{_organizationId}/location";
            _testOutputHelper.WriteLine(locationRequestUri);
            var locations = await httpClient.GetFromJsonAsync<IList<LocationDTO>>(locationRequestUri);

            // Assert
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            Assert.True(Org!.Location.Name == newLocation.Name);
            Assert.True(locations!.Count == 2);
            Assert.True(returnedLocation!.IsPrimary);
            Assert.True(returnedLocation!.Id == Org!.Location.Id);
        }
    }
}