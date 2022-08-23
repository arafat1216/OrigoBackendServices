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
using Common.Extensions;
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
        private readonly Guid _organizationTwoId;
        private readonly Guid _departmentId;


        private readonly CustomerWebApplicationFactory<Startup> _factory;

        public OrganizationsControllerTests(CustomerWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
            _factory = factory;
            _organizationId = factory.ORGANIZATION_ID;
            _organizationTwoId = factory.ORGANIZATION__TWO_ID;
            _departmentId = factory.HEAD_DEPARTMENT_ID;
            _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.SystemUserId().ToString());
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
        public async Task CreateOrganization_OrganizationNumberAlreadyExists_ReturnedErrorMessage()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            // Setup
            var requestUri = $"/api/v1/organizations";
            _testOutputHelper.WriteLine(requestUri);

            var newOrganization = new NewOrganizationDTO
            {
                Name = "Techstep",
                OrganizationNumber = "ORGNUM12345",
                Address = new AddressDTO
                {
                    City = "OSLO",
                    Country = "NO",
                    PostCode = "0319",
                    Street = "Billingstadseletta 19"
                },
                ContactPerson = new ContactPersonDTO
                {
                    Email = "atest@atest.no",
                    FirstName = "Testing",
                    LastName = "Testing",
                    PhoneNumber = "+4790909090"
                },
                Location = new LocationDTO
                {
                    Name = "Location one",
                    Description = null,
                    Address1 = "Billingstadseletta 19",
                    Address2 = null,
                    City = "OSLO",
                    PostalCode = "0319",
                    Country = "NO"
                },
                PrimaryLocation = null,
                ParentId = null,
                InternalNotes = null,
                IsCustomer = true,
                Preferences = null
            };

            // Do the request
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(newOrganization));
            var response = await httpClient.PostAsJsonAsync(requestUri, newOrganization);
            var responseMessage = await response.Content.ReadAsStringAsync();

            // Check asserts
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Equal("An organization with the provided organization-number already exists.", responseMessage);
        }

        [Fact()]
        public async Task CreateOrganization_CheckAddUsersToOkta_ReturnedCorrectly()
        {
            // Setup
            var requestUri = $"/api/v1/organizations";
            _testOutputHelper.WriteLine(requestUri);

            var newOrganization = new NewOrganizationDTO
            {
                Name = "TE BØ",
                OrganizationNumber = "912343222",
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
                InternalNotes = null,
                IsCustomer = true,
                Preferences = null,
                AddUsersToOkta = true
            };

            // Do the request
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(newOrganization));
            var response = await _httpClient.PostAsJsonAsync(requestUri, newOrganization);
            var addedOrganization = await response.Content.ReadFromJsonAsync<OrganizationDTO>();
            var getRequestUri = $"{requestUri}/{addedOrganization!.OrganizationId}/true";
            var content = await _httpClient.GetStringAsync(getRequestUri);
            _testOutputHelper.WriteLine(content);
            var readOrganization = await _httpClient.GetFromJsonAsync<OrganizationDTO>(getRequestUri);

            // Check asserts
            Assert.True(readOrganization!.AddUsersToOkta);
        }

        [Fact()]
        public async Task UpdateOrganizationTest_WithLastDayOfReport()
        {
            // Setup
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
            var requestUri = $"/api/v1/organizations/{_organizationId}/true";
            _testOutputHelper.WriteLine(requestUri);
            var org = await httpClient.GetFromJsonAsync<OrganizationDTO>(requestUri);

            // Do the request
            var orgData = new UpdateOrganization()
            {
                OrganizationId = _organizationId,
                LastDayForReportingSalaryDeduction = 20,
                Name = org.Name
            };
            requestUri = $"/api/v1/organizations/{_organizationId}/organization";
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(org));
            var response = await _httpClient.PutAsJsonAsync(requestUri, orgData);
            var responseasd = await response.Content.ReadAsStringAsync();
            var updatedOrg = await response.Content.ReadFromJsonAsync<OrganizationDTO>();

            // Check asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(orgData.LastDayForReportingSalaryDeduction, updatedOrg!.LastDayForReportingSalaryDeduction);
        }

        [Fact()]
        public async Task UpdateOrganizationTest_WithoutLastDayOfReport()
        {
            // Setup
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
            var requestUri = $"/api/v1/organizations/{_organizationId}/true";
            _testOutputHelper.WriteLine(requestUri);
            var org = await httpClient.GetFromJsonAsync<OrganizationDTO>(requestUri);

            // Do the request
            var orgData = new UpdateOrganization()
            {
                OrganizationId = _organizationId,
                Name = "UpdateTest"
            };
            requestUri = $"/api/v1/organizations/{_organizationId}/organization";
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(org));
            var response = await _httpClient.PutAsJsonAsync(requestUri, orgData);
            var responseasd = await response.Content.ReadAsStringAsync();
            var updatedOrg = await response.Content.ReadFromJsonAsync<OrganizationDTO>();

            // Check asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(orgData.Name, updatedOrg!.Name);
            Assert.Equal(org!.LastDayForReportingSalaryDeduction, updatedOrg!.LastDayForReportingSalaryDeduction);
        }

        [Fact()]
        public async Task UpdateOrganizationTest_WithPayrollEmail()
        {
            // Setup
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
            var requestUri = $"/api/v1/organizations/{_organizationId}/true";
            _testOutputHelper.WriteLine(requestUri);
            var org = await httpClient.GetFromJsonAsync<OrganizationDTO>(requestUri);

            // Do the request
            var orgData = new UpdateOrganization()
            {
                OrganizationId = _organizationId,
                PayrollContactEmail = "example@techstep.no",
                Name = org.Name
            };
            requestUri = $"/api/v1/organizations/{_organizationId}/organization";
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(org));
            var response = await _httpClient.PutAsJsonAsync(requestUri, orgData);
            var responseasd = await response.Content.ReadAsStringAsync();
            var updatedOrg = await response.Content.ReadFromJsonAsync<OrganizationDTO>();

            // Check asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(orgData.PayrollContactEmail, updatedOrg!.PayrollContactEmail);
        }

        [Fact()]
        public async Task UpdateOrganizationTest_WithoutPayrollEmail()
        {
            // Setup
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
            var requestUri = $"/api/v1/organizations/{_organizationId}/true";
            _testOutputHelper.WriteLine(requestUri);
            var org = await httpClient.GetFromJsonAsync<OrganizationDTO>(requestUri);

            // Do the request
            var orgData = new UpdateOrganization()
            {
                OrganizationId = _organizationId,
                Name = "UpdateTest"
            };
            requestUri = $"/api/v1/organizations/{_organizationId}/organization";
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(org));
            var response = await _httpClient.PutAsJsonAsync(requestUri, orgData);
            var responseasd = await response.Content.ReadAsStringAsync();
            var updatedOrg = await response.Content.ReadFromJsonAsync<OrganizationDTO>();

            // Check asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(orgData.Name, updatedOrg!.Name);
            Assert.Equal(org!.PayrollContactEmail, updatedOrg!.PayrollContactEmail);
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
        [Fact]
        public async Task InitiateOnbarding_ChangeOnboardingStatus()
        {
            
            // Arrange
            var requestUri = $"/api/v1/organizations/{_organizationId}/initiate-onboarding";

            //Act
            var response = await _httpClient.PostAsync(requestUri, null);
            var organization = await response.Content.ReadFromJsonAsync<OrganizationDTO>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(Common.Enums.CustomerStatus.StartedOnboarding,organization?.Status);
            Assert.Equal("StartedOnboarding", organization?.StatusName);
        }
        [Fact]
        public async Task InitiateOnbarding_OrganizationNotFound()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var requestUri = $"/api/v1/organizations/{organizationId}/initiate-onboarding";

            //Act
            var response = await _httpClient.PostAsync(requestUri, null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task InitiateOnbarding_NoUsersForCustomer()
        {
            //add new organization
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
                InternalNotes = null,
                IsCustomer = true,
                Preferences = null
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, newOrganization);
            var organization = await response.Content.ReadFromJsonAsync<OrganizationDTO>();

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Arrange
            var requestInit = $"/api/v1/organizations/{organization.OrganizationId}/initiate-onboarding";

            //Act
            var responseInit = await _httpClient.PostAsync(requestInit, null);
            var organizationInit = await responseInit.Content.ReadFromJsonAsync<OrganizationDTO>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseInit.StatusCode);
            Assert.Equal(organization.OrganizationId, organizationInit?.OrganizationId);

        }
        [Fact]
        public async Task GetOrganizationUserCountAsync_ForSystemAdmin()
        {
            // Arrange
            FilterOptionsForUser filter = new FilterOptionsForUser { Roles = new string[] {"SystemAdmin"} };
            var json = JsonSerializer.Serialize(filter);
            var requestUri = $"/api/v1/organizations/userCount?filterOptions={json}";

            //Act
            var response = await _httpClient.GetAsync(requestUri);
            var organizationCount = await response.Content.ReadFromJsonAsync<IList<CustomerServices.Models.OrganizationUserCount>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, organizationCount?.Count);
            Assert.Collection(organizationCount,
               item => Assert.Equal(1,item.Count),
               item => Assert.Equal(1,item.Count)
           );
        }
        [Fact]
        public async Task GetOrganizationUserCountAsync_ForAdmin()
        {
            // Arrange
            FilterOptionsForUser filter = new FilterOptionsForUser {AssignedToDepartments= new Guid[] { _organizationId, _departmentId }, Roles = new string[] { "Admin" } };
            var json = JsonSerializer.Serialize(filter);
            var requestUri = $"/api/v1/organizations/userCount?filterOptions={json}";

            //Act
            var response = await _httpClient.GetAsync(requestUri);
            var organizationCount = await response.Content.ReadFromJsonAsync<IList<CustomerServices.Models.OrganizationUserCount>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, organizationCount?.Count);
            Assert.Collection(organizationCount,
               item => Assert.Equal(1, item.Count)
           );
        }
        [Fact]
        public async Task GetOrganizationUserCountAsync_ForAdmin_OrganizationIdIsNotValid()
        {
            // Arrange
            FilterOptionsForUser filter = new FilterOptionsForUser { AssignedToDepartments = new Guid[] { Guid.NewGuid() }, Roles = new string[] { "Admin" } };
            var json = JsonSerializer.Serialize(filter);
            var requestUri = $"/api/v1/organizations/userCount?filterOptions={json}";

            //Act
            var response = await _httpClient.GetAsync(requestUri);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task GetOrganizationUserCountAsync_ForSystemAdmin_CountAll()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            //Arrange
            var body = new NewUser
            {
                Email = "test@mail.com",
                FirstName = "test",
                LastName = "user",
                EmployeeId = "123",
                MobileNumber = "+479898645",
                Role = "EndUser"
            };
            var request = $"/api/v1/organizations/{_organizationTwoId}/users";

            //Act
            var postUser = await httpClient.PostAsJsonAsync(request, body);
            var user = await postUser.Content.ReadFromJsonAsync<ViewModels.User>();

            //Assert
            Assert.NotNull(user);
            Assert.Equal(2, user?.UserStatus);
            Assert.Equal("Invited", user?.UserStatusName);
            Assert.True(user?.IsActiveState);
            Assert.Equal("no", user?.UserPreference.Language);

            // Arrange
            FilterOptionsForUser filter = new FilterOptionsForUser { Roles = new string[] { "SystemAdmin" } };
            var json = JsonSerializer.Serialize(filter);
            var requestUri = $"/api/v1/organizations/userCount?filterOptions={json}";

            //Act
            var response = await httpClient.GetAsync(requestUri);
            var organizationCount = await response.Content.ReadFromJsonAsync<IList<CustomerServices.Models.OrganizationUserCount>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, organizationCount?.Count);
            Assert.Collection(organizationCount,
               item => Assert.Equal(1, item.Count),
               item => Assert.Equal(1, item.Count)
           );
            Assert.Collection(organizationCount,
               item => Assert.Equal(1, item.NotOnboarded),
               item => Assert.Equal(0, item.NotOnboarded)
           );
            Assert.Collection(organizationCount,
              item => Assert.Equal(_organizationTwoId, item.OrganizationId),
              item => Assert.Equal(_organizationId, item.OrganizationId)
          );

        }
       
    }
}