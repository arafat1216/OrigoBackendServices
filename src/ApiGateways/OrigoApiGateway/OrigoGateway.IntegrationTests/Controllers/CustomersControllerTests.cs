using Common.Interfaces;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq.Protected;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Mappings;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class CustomersControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<CustomersController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<CustomersController> _factory;
        private readonly ITestOutputHelper _output;

        public CustomersControllerTests(OrigoGatewayWebApplicationFactory<CustomersController> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            factory.ClientOptions.AllowAutoRedirect = false;
        }
        public static IEnumerable<object[]> EmailAccess =>
       new List<object[]>
       {
            new object[] { "manager@test.io", "Manager", HttpStatusCode.Forbidden },
            new object[] { "enduser@test.io", "EndUser", HttpStatusCode.Forbidden },
            new object[] { "departmentmanager@test.io", "DepartmentManager", HttpStatusCode.Forbidden },
            new object[] { "partnerAdmin@test.io", "PartnerAdmin", HttpStatusCode.OK },
            new object[] { "admin@test.io", "Admin", HttpStatusCode.OK },
            new object[] { "customerAdmin@test.io", "CustomerAdmin", HttpStatusCode.OK },
            new object[] { "systemadmin@test.io", "SystemAdmin", HttpStatusCode.OK}
       };


        [Theory]
        [MemberData(nameof(EmailAccess))]
        public async Task Get_SecurePageAccessibleOnlyByAdminUsers(string email, string role, HttpStatusCode expected)
        {
            var organizationId = Guid.NewGuid();
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "OnAndOffboarding"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var customerService = new Mock<ICustomerServices>();
                    var organization = new Organization { OrganizationId = organizationId };
                    customerService.Setup(_ => _.InitiateOnbardingAsync(organizationId)).ReturnsAsync(organization);
                    services.AddSingleton(customerService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PostAsync($"/origoapi/v1.0/customers/{organizationId}/initiate-onboarding", null);

            Assert.Equal(expected, response.StatusCode);
        }

        public static IEnumerable<object[]> EmailAccessUserCount => new List<object[]>
        {
            new object[] { "manager@test.io", "Manager", HttpStatusCode.Forbidden },
            new object[] { "enduser@test.io", "EndUser", HttpStatusCode.Forbidden },
            new object[] { "departmentmanager@test.io", "DepartmentManager", HttpStatusCode.Forbidden },
            new object[] { "partnerAdmin@test.io", "PartnerAdmin", HttpStatusCode.OK },
            new object[] { "admin@test.io", "Admin", HttpStatusCode.OK },
            new object[] { "customerAdmin@test.io", "CustomerAdmin", HttpStatusCode.OK },
            new object[] { "systemadmin@test.io", "SystemAdmin", HttpStatusCode.OK}
        };


        [Theory]
        [MemberData(nameof(EmailAccessUserCount))]
        public async Task userCount_SecurePageAccessibleOnlyForGivenRoles(string email, string role, HttpStatusCode expected)
        {
            var organizationId = Guid.NewGuid();
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "OnAndOffboarding"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var customerService = new Mock<ICustomerServices>();

                    var count = new List<CustomerUserCount>
                    {
                        new CustomerUserCount {OrganizationId = organizationId,Count = 1, NotOnboarded = 1}
                    };

                    customerService.Setup(_ => _.GetCustomerUsersAsync(It.IsAny<FilterOptionsForUser>())).ReturnsAsync(count);
                    services.AddSingleton(customerService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/userCount");

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task UpdateOrganization_WithoutPayrollContactEmail()
        {
            var organizationId = Guid.NewGuid();
            var email = "test@techstep.no";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "CustomerAdmin"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var customerService = new Mock<ICustomerServices>();

                    customerService.Setup(_ => _.UpdateOrganizationAsync(It.IsAny<UpdateOrganizationDTO>())).ReturnsAsync(new Organization());
                    services.AddSingleton(customerService.Object);
                });


            }).CreateClient();

            var postData = new UpdateOrganization()
            {
                OrganizationId = organizationId,
                Name = "TestName",
                OrganizationNumber = "919724617",
                ContactPerson = new OrigoContactPerson()
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test@techstep.no",
                    PhoneNumber = "+4790909090",
                },
                Address = new Address
                {
                    City = "OSLO",
                    Country = "NO",
                    Postcode = "0554",
                    Street = "Markveien 32F"
                },
            };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PutAsJsonAsync($"/origoapi/v1.0/customers", postData);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task UpdateOrganization_WithLastDayForReportingSalaryDeduction()
        {
            var organizationId = Guid.NewGuid();
            var email = "test@techstep.no";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "CustomerAdmin"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var postData = @"{
                                ""organizationId"": """ + organizationId + @""",
                                ""name"": ""Anne Petra Østli"",
                                ""organizationNumber"": ""123456789"",
                                ""address"": {
                                    ""street"": ""Test"",
                                    ""postcode"": ""1234"",
                                    ""city"": ""Test"",
                                    ""country"": ""NO""
                                },
                                ""contactPerson"": {
                                    ""firstName"": ""Anne Petra"",
                                    ""lastName"": ""Østli"",
                                    ""email"": ""annepetra@gture.com"",
                                    ""phoneNumber"": ""+4797698931""
                                },
                                ""lastDayForReportingSalaryDeduction"": ""18"",
                                ""payrollContactEmail"": ""annepetra@gture.com""
                            }";

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var mockFactory = new Mock<IHttpClientFactory>();
                    var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                    mockHttpMessageHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                            ItExpr.IsAny<CancellationToken>())
                        .ReturnsAsync(new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(postData)
                        });

                    var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
                    mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
                    var options = new CustomerConfiguration() { ApiPath = @"/customers" };
                    var optionsMock = new Mock<IOptions<CustomerConfiguration>>();
                    optionsMock.Setup(o => o.Value).Returns(options);
                    var mappingConfig = new MapperConfiguration(mc =>
                    {
                        mc.AddMaps(Assembly.GetAssembly(typeof(UpdateOrganizationProfile)));
                    });
                    var _mapper = mappingConfig.CreateMapper();

                    var customerServices = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), mockFactory.Object, optionsMock.Object, _mapper);
                    services.AddSingleton<ICustomerServices>(x => customerServices);

                });


            }).CreateClient();

            var content = new StringContent(postData, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PatchAsync($"/origoapi/v1.0/customers", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task UpdateOrganization_WithoutLastDayForReportingSalaryDeduction()
        {
            var organizationId = Guid.NewGuid();
            var email = "test@techstep.no";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "CustomerAdmin"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var postData = @"{
                                ""organizationId"": """ + organizationId + @""",
                                ""name"": ""Anne Petra Østli"",
                                ""organizationNumber"": ""123456789"",
                                ""address"": {
                                    ""street"": ""Test"",
                                    ""postcode"": ""1234"",
                                    ""city"": ""Test"",
                                    ""country"": ""NO""
                                },
                                ""contactPerson"": {
                                    ""firstName"": ""Anne Petra"",
                                    ""lastName"": ""Østli"",
                                    ""email"": ""annepetra@gture.com"",
                                    ""phoneNumber"": ""+4797698931""
                                },
                                ""payrollContactEmail"": ""annepetra@gture.com""
                            }";

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var mockFactory = new Mock<IHttpClientFactory>();
                    var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                    mockHttpMessageHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                            ItExpr.IsAny<CancellationToken>())
                        .ReturnsAsync(new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(postData)
                        });

                    var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
                    mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
                    var options = new CustomerConfiguration() { ApiPath = @"/customers" };
                    var optionsMock = new Mock<IOptions<CustomerConfiguration>>();
                    optionsMock.Setup(o => o.Value).Returns(options);
                    var mappingConfig = new MapperConfiguration(mc =>
                    {
                        mc.AddMaps(Assembly.GetAssembly(typeof(UpdateOrganizationProfile)));
                    });
                    var _mapper = mappingConfig.CreateMapper();

                    var customerServices = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), mockFactory.Object, optionsMock.Object, _mapper);
                    services.AddSingleton<ICustomerServices>(x => customerServices);

                });


            }).CreateClient();

            var content = new StringContent(postData, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PatchAsync($"/origoapi/v1.0/customers", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public static IEnumerable<object[]> GetOrganizationsRoleAndAccesslist =>
      new List<object[]>
      {
            new object[] { "enduser@test.io", "EndUser", HttpStatusCode.Forbidden },
            new object[] { "manager@test.io", "Manager", HttpStatusCode.Forbidden },
            new object[] { "departmentmanager@test.io", "DepartmentManager", HttpStatusCode.Forbidden },
            new object[] { "admin@test.io", "Admin", HttpStatusCode.Forbidden },
            new object[] { "groupadmin@test.io", "GroupAdmin", HttpStatusCode.Forbidden },
            new object[] { "customeradmin@test.io", "CustomerAdmin", HttpStatusCode.Forbidden },
            new object[] { "systemadmin@test.io", "SystemAdmin", HttpStatusCode.OK},
            new object[] { "partnerAdmin@test.io", "PartnerAdmin", HttpStatusCode.OK },
            new object[] { "partnerAdmin@test.io", "PartnerReadOnlyAdmin", HttpStatusCode.Forbidden }
      };


        [Theory]
        [MemberData(nameof(GetOrganizationsRoleAndAccesslist))]
        public async Task Get_SecurePageAccessibleOnlyForPartnerAdminAndSystemAdmin(string email, string role, HttpStatusCode expected)
        {
            var partnerId = Guid.NewGuid();
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var customerService = new Mock<ICustomerServices>();
                    var organizations = new List<Organization> { new Organization { PartnerId = partnerId } };
                    customerService.Setup(_ => _.GetCustomersAsync(partnerId, true)).ReturnsAsync(organizations);
                    services.AddSingleton(customerService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/?partnerId={partnerId}");

            Assert.Equal(expected, response.StatusCode);
        }


        [Fact]
        public async Task Get_PartnerAdminQueryparameterIsWrong_ShouldBeOverwrittenWithRightPartnerId()
        {
            var partnerId = Guid.NewGuid();
            var notPartnerId = Guid.NewGuid();
            var email = "partnerAdmin@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "PartnerAdmin"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var customerService = new Mock<ICustomerServices>();
                    var organizations = new List<Organization> { new Organization { PartnerId = partnerId } };
                    customerService.Setup(_ => _.GetCustomersAsync(partnerId, true)).ReturnsAsync(organizations);
                    services.AddSingleton(customerService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/?partnerId={notPartnerId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task GetOrganization_PartnerAdminOrganizationIdNotInAccessList()
        {
            var partnerId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();
            var differentOrganizationId = Guid.NewGuid();
            var email = "partnerAdmin@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "PartnerAdmin"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", differentOrganizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{organizationId}");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }


        [Fact]
        public async Task GetOrganization_PartnerAdminOrganizationIdIsInAccessList()
        {
            var partnerId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();
            var differentOrganizationId = Guid.NewGuid();
            var email = "partnerAdmin@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "PartnerAdmin"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", differentOrganizationId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var customerService = new Mock<ICustomerServices>();
                    var organizations = new Organization { OrganizationId = organizationId, PartnerId = partnerId };
                    customerService.Setup(_ => _.GetCustomerAsync(organizationId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(organizations);
                    services.AddSingleton(customerService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{organizationId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Creates a new customer as a partner-admin, where the partner-ID has not been specified.
        // The ID should be auto-assigned based on the user's access-list.
        [Fact]
        public async Task CreateCustomer_PartnerAdminForbidden_NotPartnerIdForCustomer()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();
            var differentOrganizationId = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "partnerAdmin@test.io";
            var created = new NewOrganization { PartnerId = partnerId };

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "PartnerAdmin"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanCreateCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", differentOrganizationId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var userPermissionServiceMock = new Mock<IUserPermissionService>();
            var customerService = new Mock<ICustomerServices>();

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var organizations = new Organization { OrganizationId = organizationId, PartnerId = partnerId };

                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    customerService.Setup(_ => _.CreateCustomerAsync(created, callerId)).ReturnsAsync(organizations);

                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddSingleton(customerService.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);

            // Act
            var response = await client.PostAsJsonAsync($"/origoapi/v1.0/customers", new NewOrganization());

            // Asserts
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            customerService.Verify(_ => _.CreateCustomerAsync(created, callerId), Times.Once());
            customerService.Verify(_ => _.CreateCustomerAsync(It.Is<NewOrganization>(e => e.PartnerId == partnerId), It.IsAny<Guid>()));
        }

        // Create a new customer as a partner-admin, where the partner-ID is not a part of the user's access-list.
        // This is a negative test, and should "fail" as the user should not have access to the requested partner-ID.
        [Fact]
        public async Task CreateCustomer_PartnerAdminAddsWithDiffrentPartnerId()
        {
            var partnerId = Guid.NewGuid();
            var notPartnerId = Guid.NewGuid();
            var email = "partnerAdmin@test.io";
            var created = new NewOrganization { PartnerId = notPartnerId };

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "PartnerAdmin"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanCreateCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PostAsJsonAsync($"/origoapi/v1.0/customers", created);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }


        [Fact]
        public async Task GetPaginatedOrganizationsAsync()
        {
            var partnerId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();
            var differentOrganizationId = Guid.NewGuid();
            var email = "partnerAdmin@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "PartnerAdmin"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", partnerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", differentOrganizationId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var customerService = new Mock<ICustomerServices>();

                    PagedModel<Organization> pagedModel = new()
                    {
                        CurrentPage = 1,
                        PageSize = 25,
                        Items = new List<Organization>
                        {
                            new() { OrganizationId = organizationId, PartnerId = partnerId },
                        },
                        TotalPages = 1,
                        TotalItems = 1
                    };

                    customerService.Setup(_ => _.GetPaginatedCustomersAsync(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<bool>())).ReturnsAsync(pagedModel);
                    services.AddSingleton(customerService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/pagination");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task GetCustomerStandardBusinessSubscriptionProduct()
        {

            var organizationId = Guid.NewGuid();
            var email = "partnerAdmin@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "PartnerAdmin"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "SubscriptionManagement"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var subscriptionManagementService = new Mock<ISubscriptionManagementService>();
                    var businessSubscriptionProducts = new List<OrigoCustomerStandardBusinessSubscriptionProduct> { new OrigoCustomerStandardBusinessSubscriptionProduct
                    {
                        DataPackage = "10GB",
                        OperatorId = 1,
                        OperatorName = "Telenor",
                        SubscriptionName = "Fri flyt"
                    } };

                    subscriptionManagementService.Setup(_ => _.GetCustomerStandardBusinessSubscriptionProductAsync(organizationId)).ReturnsAsync(businessSubscriptionProducts);
                    services.AddSingleton(subscriptionManagementService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{organizationId}/standard-business-subscription-products");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteStandardBusinessSubscriptionProducts()
        {

            var organizationId = Guid.NewGuid();
            var email = "partnerAdmin@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "PartnerAdmin"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "SubscriptionManagement"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var subscriptionManagementService = new Mock<ISubscriptionManagementService>();
                    var businessSubscriptionProducts = new OrigoCustomerStandardBusinessSubscriptionProduct
                    {
                        DataPackage = "10GB",
                        OperatorId = 1,
                        OperatorName = "Telenor",
                        SubscriptionName = "Fri flyt"
                    };

                    subscriptionManagementService.Setup(_ => _.DeleteCustomerStandardBusinessSubscriptionProductAsync(organizationId, 1)).ReturnsAsync(businessSubscriptionProducts);
                    services.AddSingleton(subscriptionManagementService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.DeleteAsync($"/origoapi/v1.0/customers/{organizationId}/standard-business-subscription-products/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddStandardBusinessSubscriptionProducts()
        {

            var organizationId = Guid.NewGuid();
            var email = "partnerAdmin@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "PartnerAdmin"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "SubscriptionManagement"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var newBusinessSubscriptionProducts = new NewStandardBusinessSubscriptionProduct
            {
                DataPackage = "10GB",
                OperatorId = 1,
                SubscriptionName = "Fri flyt"
            };

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), email, CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });

                    var subscriptionManagementService = new Mock<ISubscriptionManagementService>();

                    var businessSubscriptionProducts = new OrigoCustomerStandardBusinessSubscriptionProduct
                    {
                        DataPackage = "10GB",
                        OperatorId = 1,
                        OperatorName = "Telenor",
                        SubscriptionName = "Fri flyt"
                    };

                    subscriptionManagementService.Setup(_ => _.PostCustomerStandardBusinessSubscriptionProductAsync(organizationId, It.IsAny<NewStandardBusinessSubscriptionProduct>())).ReturnsAsync(businessSubscriptionProducts);
                    services.AddSingleton(subscriptionManagementService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PostAsJsonAsync($"/origoapi/v1.0/customers/{organizationId}/standard-business-subscription-products", newBusinessSubscriptionProducts);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
