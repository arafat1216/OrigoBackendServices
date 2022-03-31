using Customer.API.Controllers;
using Xunit;
using Customer.API.Tests;
using System.Net.Http;
using System;
using Xunit.Abstractions;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using Customer.API.WriteModels;
using CustomerServices.ServiceModels;
using System.Threading.Tasks;
using System.Net;
using CustomerServices.Models;

namespace Customer.API.Controllers.Tests
{
    public class OrganizationsControllerTests : IClassFixture<CustomerWebApplicationFactory<OrganizationsController>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;


        public OrganizationsControllerTests(CustomerWebApplicationFactory<OrganizationsController> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
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
    }
}