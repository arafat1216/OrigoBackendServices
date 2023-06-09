﻿using Common.Extensions;
using Common.Interfaces;
using Customer.API.IntegrationTests.Helpers;
using Customer.API.Tests;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices.ServiceModels;
using System.Net.Http.Json;
using System.Text.Json;

namespace Customer.API.IntegrationTests.Controllers
{
    public class OrganizationsControllerTests : IClassFixture<CustomerWebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;
        private readonly Guid _organizationId;
        private readonly Guid _organizationTwoId;
        private readonly Guid _organizationIdThree;
        private readonly Guid _departmentId;
        private readonly Guid _techstepPartnerId;
        private readonly Guid _partnerId;
        private readonly Guid _userOneId;


        private readonly CustomerWebApplicationFactory<Startup> _factory;

        public OrganizationsControllerTests(CustomerWebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = factory.CreateDefaultClient();
            _factory = factory;
            _organizationId = factory.ORGANIZATION_ID;
            _organizationTwoId = factory.ORGANIZATION_TWO_ID;
            _organizationIdThree = factory.ORGANIZATION_THREE_ID;
            _departmentId = factory.HEAD_DEPARTMENT_ID;
            _techstepPartnerId = factory.TECHSTEP_PARTNER_ID;
            _partnerId = factory.PARTNER_ID;
            _userOneId = factory.USER_ONE_ID;
            _httpClient.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.SystemUserId().ToString());
        }

        [Fact()]
        public async Task CreateOrganizationTest()
        {
            // Setup
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

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
            var response = await httpClient.PostAsJsonAsync(requestUri, newOrganization);
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

        [Fact]
        public async Task CreateOrganization_WithTechstepAsPartner_SaveTechstepCustomerIdAndAccountOwner()
        {
            // Setup
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            var requestUri = $"/api/v1/organizations";

            string accountOwner = "Julia";
            long techstepCustomerId = 1233232323;

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
                AddUsersToOkta = true,
                PartnerId = _techstepPartnerId,
                AccountOwner = accountOwner,
                TechstepCustomerId = techstepCustomerId
            };

            var response = await httpClient.PostAsJsonAsync(requestUri, newOrganization);
            var organization = await response.Content.ReadFromJsonAsync<OrganizationDTO>();


            // Check asserts
            Assert.Equal(accountOwner, organization?.AccountOwner);
            Assert.Equal(techstepCustomerId, organization?.TechstepCustomerId);
        }

        [Fact]
        public async Task CreateOrganization_PartnerNotTechstep_NotSaveTechstepCustomerId()
        {
            // Setup
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);
            var requestUri = $"/api/v1/organizations";

            string accountOwner = "Julia";
            long techstepCustomerId = 1233232323;

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
                AddUsersToOkta = true,
                PartnerId = _partnerId,
                AccountOwner = accountOwner,
                TechstepCustomerId = techstepCustomerId
            };

            var response = await httpClient.PostAsJsonAsync(requestUri, newOrganization);
            var organization = await response.Content.ReadFromJsonAsync<OrganizationDTO>();

            // Check asserts
            Assert.Equal(accountOwner, organization?.AccountOwner);
            Assert.Null(organization?.TechstepCustomerId);
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
            Assert.DoesNotContain(newLocations, x => x.Id == selectedLocation.Id);
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

            // Act
            var response = await _httpClient.PostAsync(requestUri, null);
            var organization = await response.Content.ReadFromJsonAsync<OrganizationDTO>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(Common.Enums.CustomerStatus.StartedOnboarding, organization?.Status);
            Assert.Equal("StartedOnboarding", organization?.StatusName);
        }

        [Fact]
        public async Task InitiateOnbarding_OrganizationNotFound()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var requestUri = $"/api/v1/organizations/{organizationId}/initiate-onboarding";

            // Act
            var response = await _httpClient.PostAsync(requestUri, null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task InitiateOnbarding_NoUsersForCustomer_ExceptionGetsThrown()
        {
            // add new organization
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

            // Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, newOrganization);
            var organization = await response.Content.ReadFromJsonAsync<OrganizationDTO>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Arrange
            var requestInit = $"/api/v1/organizations/{organization.OrganizationId}/initiate-onboarding";

            // Act
            var responseInit = await _httpClient.PostAsync(requestInit, null);
            var errorMessage = await responseInit.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseInit.StatusCode);
            Assert.Equal("Customers need to have at least one user imported to initiate the onboarding process.", errorMessage);

        }

        [Fact]
        public async Task GetOrganizationUserCountAsync_ForSystemAdmin()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            FilterOptionsForUser filter = new FilterOptionsForUser { Roles = new string[] { "SystemAdmin" } };
            var json = JsonSerializer.Serialize(filter);
            var requestUri = $"/api/v1/organizations/userCount?filterOptions={json}";

            // Act
            var response = await httpClient.GetAsync(requestUri);
            var organizationCount = await response.Content.ReadFromJsonAsync<IList<CustomerServices.Models.OrganizationUserCount>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(3, organizationCount?.Count);
            Assert.Collection(organizationCount,
               item => Assert.Equal(0, item.Count),
               item => Assert.Equal(1, item.Count),
               item => Assert.Equal(1, item.Count)
           );
        }

        [Fact]
        public async Task GetOrganizationUserCountAsync_ForAdmin()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            FilterOptionsForUser filter = new FilterOptionsForUser { AssignedToDepartments = new Guid[] { _organizationId, _departmentId }, Roles = new string[] { "Admin" } };
            var json = JsonSerializer.Serialize(filter);
            var requestUri = $"/api/v1/organizations/userCount?filterOptions={json}";

            // Act
            var response = await httpClient.GetAsync(requestUri);
            var organizationCount = await response.Content.ReadFromJsonAsync<IList<CustomerServices.Models.OrganizationUserCount>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, organizationCount?.Count);
            Assert.Collection(organizationCount,
               item => Assert.Equal(1, item.Count)
           );
        }

        [Fact]
        public async Task GetOrganizationUserCountAsync_ShouldNotCountDeletedUsers()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            // Delete a user
            var url = $"/api/v1/organizations/{_organizationId}/users/{_userOneId}";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Content = JsonContent.Create(Guid.NewGuid());
            var deleteResponse = await httpClient.SendAsync(request);
            var deletedUser = await deleteResponse.Content.ReadFromJsonAsync<User>();
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            Assert.NotNull(deletedUser);
            Assert.Equal(_userOneId, deletedUser!.Id);
            Assert.Equal("Invited", deletedUser.UserStatusName);

            // Arrange - Filter based on guids
            FilterOptionsForUser filterBasedOnGuids = new FilterOptionsForUser { AssignedToDepartments = new Guid[] { _organizationId } };
            var jsonBasedOnGuids = JsonSerializer.Serialize(filterBasedOnGuids);
            var requestUri = $"/api/v1/organizations/userCount?filterOptions={jsonBasedOnGuids}";

            // Act
            var response = await httpClient.GetAsync(requestUri);
            var organizationCount = await response.Content.ReadFromJsonAsync<IList<CustomerServices.Models.OrganizationUserCount>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, organizationCount?.Count);
            Assert.Collection(organizationCount,
               item => Assert.Equal(1, item.Count)
            );
            Assert.Collection(organizationCount,
              item => Assert.Equal(2, item.NotOnboarded)
            );

            // Arrange - all customers
            FilterOptionsForUser filterEmpty = new FilterOptionsForUser { };
            var jsonEmpty = JsonSerializer.Serialize(filterEmpty);
            var requestAllOrganizations = $"/api/v1/organizations/userCount?filterOptions={jsonEmpty}";

            // Act
            var responseAllOrganizations = await httpClient.GetAsync(requestAllOrganizations);
            var allOrganizationCount = await responseAllOrganizations.Content.ReadFromJsonAsync<IList<CustomerServices.Models.OrganizationUserCount>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseAllOrganizations.StatusCode);
            Assert.Equal(3, allOrganizationCount?.Count);
            Assert.Collection(allOrganizationCount,
               item => Assert.Equal(0, item.Count),
               item => Assert.Equal(1, item.Count),
               item => Assert.Equal(1, item.Count)
           );
            Assert.Collection(allOrganizationCount,
               item => Assert.Equal(1, item.NotOnboarded),
               item => Assert.Equal(1, item.NotOnboarded),
               item => Assert.Equal(2, item.NotOnboarded)
           );
            Assert.Collection(allOrganizationCount,
              item => Assert.Equal(_organizationIdThree, item.OrganizationId),
              item => Assert.Equal(_organizationTwoId, item.OrganizationId),
              item => Assert.Equal(_organizationId, item.OrganizationId)

          );
        }

        [Fact]
        public async Task GetOrganizationUserCountAsync_ForAdmin_OrganizationIdIsNotValid()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            FilterOptionsForUser filter = new FilterOptionsForUser { AssignedToDepartments = new Guid[] { Guid.NewGuid() }, Roles = new string[] { "Admin" } };
            var json = JsonSerializer.Serialize(filter);
            var requestUri = $"/api/v1/organizations/userCount?filterOptions={json}";

            // Act
            var response = await httpClient.GetAsync(requestUri);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetOrganizationUserCountAsync_ForSystemAdmin_CountAll()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            // Arrange
            var body = new NewUser
            {
                Email = "test@mail.com",
                FirstName = "test",
                LastName = "user",
                EmployeeId = "123",
                MobileNumber = "+479898645",
                Role = "EndUser",
                NeedsOnboarding = true
            };
            var request = $"/api/v1/organizations/{_organizationTwoId}/users";

            //Act
            var postUser = await httpClient.PostAsJsonAsync(request, body);
            var user = await postUser.Content.ReadFromJsonAsync<ViewModels.User>();

            // Assert
            Assert.NotNull(user);
            Assert.Equal(2, user?.UserStatus);
            Assert.Equal("Invited", user?.UserStatusName);
            Assert.True(user?.IsActiveState);
            Assert.Equal("no", user?.UserPreference.Language);

            // Arrange
            FilterOptionsForUser filter = new FilterOptionsForUser { Roles = new string[] { "SystemAdmin" } };
            var json = JsonSerializer.Serialize(filter);
            var requestUri = $"/api/v1/organizations/userCount?filterOptions={json}";

            // Act
            var response = await httpClient.GetAsync(requestUri);
            var organizationCount = await response.Content.ReadFromJsonAsync<IList<CustomerServices.Models.OrganizationUserCount>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(3, organizationCount?.Count);
            Assert.Collection(organizationCount,
               item => Assert.Equal(1, item.Count),
               item => Assert.Equal(0, item.Count),
               item => Assert.Equal(1, item.Count)
           );
            Assert.Collection(organizationCount,
               item => Assert.Equal(2, item.NotOnboarded),
               item => Assert.Equal(1, item.NotOnboarded),
               item => Assert.Equal(3, item.NotOnboarded)
           );
            Assert.Collection(organizationCount,
              item => Assert.Equal(_organizationTwoId, item.OrganizationId),
              item => Assert.Equal(_organizationIdThree, item.OrganizationId),
              item => Assert.Equal(_organizationId, item.OrganizationId)

          );

        }

        [Fact]
        public async Task UpdateOrganizationTechstepCore_UpdateToNewChanges()
        {
            // Arrange
            var newName = "ORGANIZATION 3";
            var newCountryCode = "EN";
            var newOrganizationNumber = "91111111222";

            var techstepInfo = new TechstepCoreCustomerUpdate
            {
                Data = new List<TechstepCoreData>
                {
                    new TechstepCoreData
                    {
                        AccountOwner = "Petter Sprett",
                        Name = newName,
                        TechstepCustomerId = 123456789,
                        ChainCode = null,
                        CountryCode = newCountryCode,
                        OrgNumber = newOrganizationNumber
                    }
                }
            };
            var requestUri = $"/api/v1/organizations/techstep-core-update";

            // Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, techstepInfo);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Get the customer and check if values is changed

            var getRequestUri = $"/api/v1/organizations/{_organizationIdThree}/true";
            var organization = await _httpClient.GetFromJsonAsync<OrganizationDTO>(getRequestUri);
            Assert.Equal(newName, organization?.Name);
            Assert.Equal(newCountryCode.ToLower(), organization?.Preferences.PrimaryLanguage);
            Assert.Equal(newOrganizationNumber, organization?.OrganizationNumber);
        }
        [Fact]
        public async Task UpdateOrganizationTechstepCore_DontUpdateSameValues()
        {
            // Arrange
            var newName = "ORGANIZATION THREE";
            var oldCountryCode = "NO";
            var oldOrganizationNumber = "ORGNUM3333";

            var techstepInfo = new TechstepCoreCustomerUpdate
            {
                Data = new List<TechstepCoreData>
                {
                    new TechstepCoreData
                    {
                        AccountOwner = "Petter Sprett",
                        Name = newName,
                        TechstepCustomerId = 123456789,
                        ChainCode = null,
                        CountryCode = oldCountryCode,
                        OrgNumber = oldOrganizationNumber
                    }
                }
            };
            var requestUri = $"/api/v1/organizations/techstep-core-update";

            // Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, techstepInfo);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Get the customer and check if values is changed

            var getRequestUri = $"/api/v1/organizations/{_organizationIdThree}/true";
            var organization = await _httpClient.GetFromJsonAsync<OrganizationDTO>(getRequestUri);
            Assert.Equal(newName, organization?.Name);
            Assert.Equal(oldCountryCode.ToLower(), organization?.Preferences.PrimaryLanguage);
            Assert.Equal(oldOrganizationNumber, organization?.OrganizationNumber);
        }

        [Fact]
        public async Task GetOrganization_CheckAccountOwner()
        {
            var getRequestUri = $"/api/v1/organizations/{_organizationIdThree}/true";
            var organization = await _httpClient.GetFromJsonAsync<OrganizationDTO>(getRequestUri);
            Assert.NotNull(organization);
            Assert.NotEmpty(organization.AccountOwner);
            Assert.Equal("Rolf Sjødal", organization.AccountOwner);
        }

        [Fact]
        public async Task GetOrganizations_PartnerAccess()
        {
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            var getRequestUri = $"/api/v1/organizations/true/?partnerId={_techstepPartnerId}";
            var organization = await httpClient.GetFromJsonAsync<IList<OrganizationDTO>>(getRequestUri);
            Assert.NotNull(organization);
            Assert.Equal(1, organization.Count);
            Assert.All(organization, org => Assert.Equal("ORGANIZATION THREE", org.Name));
            Assert.All(organization, org => Assert.Equal(_techstepPartnerId, org.PartnerId));
        }


        [Fact]
        public async Task GetPaginatedOrganizations_PartnerFilter_Async()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            // Act
            var getRequestUri = $"/api/v1/organizations?partnerId={_techstepPartnerId}";
            var result = await httpClient.GetFromJsonAsync<PagedModel<OrganizationDTO>>(getRequestUri);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalItems);
            Assert.Equal(1, result.Items.Count);

            Assert.All(result.Items, org => Assert.Equal(_techstepPartnerId, org.PartnerId));
        }


        [Fact]
        public async Task GetPaginatedOrganizations_NoIncludes_Async()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            // Act
            var getRequestUri = $"/api/v1/organizations?includePreferences=false";
            var result = await httpClient.GetFromJsonAsync<PagedModel<OrganizationDTO>>(getRequestUri);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.TotalItems); // Change this number as we add more seeding-data.
            Assert.Equal(result.TotalItems, result.Items.Count);

            Assert.All(result.Items, org => Assert.Null(org.Preferences));
        }

        [Fact]
        public async Task GetPaginatedOrganizations_WithIncludes_Async()
        {
            // Arrange
            var httpClient = _factory.CreateClientWithDbSetup(CustomerTestDataSeedingForDatabase.ResetDbForTests);

            // Act
            var getRequestUri = $"/api/v1/organizations?includePreferences=true";
            var result = await httpClient.GetFromJsonAsync<PagedModel<OrganizationDTO>>(getRequestUri);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.TotalItems); // Change this number as we add more seeding-data.
            Assert.Equal(result.TotalItems, result.Items.Count);

            Assert.Contains(result.Items, org => org.Preferences is not null);
        }
    }
}