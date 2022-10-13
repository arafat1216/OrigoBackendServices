using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class UsersControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<UsersController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<UsersController> _factory;
        private readonly ITestOutputHelper _output;

        public UsersControllerTests(OrigoGatewayWebApplicationFactory<UsersController> factory, ITestOutputHelper output)
        {
            _factory = factory;
            factory.ClientOptions.AllowAutoRedirect = false;
            _output = output;
        }
        [Fact]
        public async Task GetUsersCount_Manager_WithReadRightsForOrganization()
        {
            var organizationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            var email = "manager@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Manager"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", departmentId.ToString()));



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
                    var userService = new Mock<IUserServices>();

                    var count = new CustomerUserCount { OrganizationId = organizationId, Count = 1, NotOnboarded = 1 };

                    userService.Setup(_ => _.GetUsersCountAsync(organizationId, It.IsAny<FilterOptionsForUser>()))
                        .ReturnsAsync(count);
                    services.AddSingleton(userService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{organizationId}/users/count");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetUsersCount_PartnerAdmin_WithReadRightsForOrganization()
        {
            var organizationId1 = Guid.NewGuid();
            var organizationId2 = Guid.NewGuid();

            var email = "manager@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, nameof(PredefinedRole.PartnerAdmin)));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId1.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId2.ToString()));



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
                    var userService = new Mock<IUserServices>();

                    var count = new CustomerUserCount { OrganizationId = organizationId1, Count = 1, NotOnboarded = 1 };

                    userService.Setup(_ => _.GetUsersCountAsync(organizationId1, It.IsAny<FilterOptionsForUser>()))
                        .ReturnsAsync(count);
                    services.AddSingleton(userService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{organizationId1}/users/count");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetUsersCount_Manager_NotReadRightsToOrganization()
        {
            var organization_NOTreadRights = Guid.NewGuid();
            var organization_readRights = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "manager@test.io";
            var role = "Manager";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organization_readRights.ToString()));


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
                    var userService = new Mock<IUserServices>();

                    FilterOptionsForUser filter = new FilterOptionsForUser { Roles = new[] { role } };
                    userService.Setup(_ => _.GetUsersCountAsync(organization_NOTreadRights, filter))
                        .Returns(Task.FromResult(new CustomerUserCount { Count = 2, NotOnboarded = 3, OrganizationId = organization_NOTreadRights }));
                    services.AddSingleton(userService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{organization_NOTreadRights}/users/count");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        [Fact]
        public async Task GetCustomerStandardPrivateSubscriptionProduct_()
        {
            var organizationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "manager@test.io";
            var role = "Manager";

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", departmentId.ToString()));


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
                    var subscriptionService = new Mock<ISubscriptionManagementService>();

                    var subProd = new List<OrigoCustomerStandardPrivateSubscriptionProduct>();

                    subscriptionService.Setup(_ => _.GetCustomerStandardPrivateSubscriptionProductAsync(organizationId))
                        .ReturnsAsync(subProd as IList<OrigoCustomerStandardPrivateSubscriptionProduct>);
                    services.AddSingleton(subscriptionService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{organizationId}/standard-private-subscription-products");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetCustomerStandardPrivateSubscriptionProduct_NotAllowed()
        {
            var organizationId = Guid.NewGuid();
            var organizationId_forUser = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "manager@test.io";
            var role = "Manager";

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId_forUser.ToString()));


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
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{organizationId}/standard-private-subscription-products");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        [Fact]
        public async Task GetAllUsers_ManagerAccsess()
        {
            var organizationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var callerId = Guid.NewGuid();
            var email = "manager@test.io";
            var role = "Manager";

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", departmentId.ToString()));

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

                    var usersService = new Mock<IUserServices>();

                    var users = new PagedModel<OrigoUser>()
                    {
                        CurrentPage = 1,
                        Items = new List<OrigoUser> {
                            new OrigoUser { FirstName = "Name"} },
                        TotalItems = 1,
                        TotalPages = 1,
                        PageSize = 1,
                    };

                    usersService.Setup(_ => _.GetAllUsersAsync(organizationId, It.IsAny<FilterOptionsForUser>(), It.IsAny<CancellationToken>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(users);

                    services.AddSingleton(usersService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{organizationId}/users");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task CompleteOnboardingAsync_CallerIdIsUserId()
        {
            var organizationId = Guid.NewGuid();
            var callerId = Guid.NewGuid();

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "mail@mail.com", CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = "mail@mail.com"; });

                    var usersService = new Mock<IUserServices>();

                    var user = new OrigoUser
                    {
                        Id = callerId
                    };

                    usersService.Setup(_ => _.CompleteOnboardingAsync(organizationId, callerId))
                        .ReturnsAsync(user);

                    services.AddSingleton(usersService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.PostAsync($"/origoapi/v1.0/customers/{organizationId}/users/onboarding-completed", null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task CreateUserForCustomer_IncludeOnboardingEqualFalse()
        {
            var organizationId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var callerId = Guid.NewGuid();

            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, callerId.ToString()));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "SystemAdmin"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));
            permissionsIdentity.AddClaim(new Claim("AccessList", organizationId.ToString()));

            var newUser = new NewUser { Email = "tesst@mail.com", MobileNumber = "+4745545457" };

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "mail@mail.com", CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options => { options.Email = "mail@mail.com"; });

                    var customerServiceMock = new Mock<ICustomerServices>();
                    var customer = new Organization
                    {
                        OrganizationId = organizationId,
                        PartnerId = partnerId
                    };

                    customerServiceMock.Setup(_ => _.GetCustomerAsync(organizationId))
                        .ReturnsAsync(customer);
                    services.AddSingleton(customerServiceMock.Object);

                    var order = new List<OrigoApiGateway.Models.ProductCatalog.ProductGet>
                    {
                        new OrigoApiGateway.Models.ProductCatalog.ProductGet
                            {
                                Id = 2,
                                PartnerId = partnerId,
                                ProductTypeId = 2,
                                Translations = new List<OrigoApiGateway.Models.ProductCatalog.Translation>
                                {
                                    new OrigoApiGateway.Models.ProductCatalog.Translation
                                    {
                                         Language = "en",
                                         Description = "Simple Asset Management for units purchased transactionally in Techstep's own WebShop.",
                                         Name = "Implement"
                                    }
                                }
                            }
                    };


                    var productOrderMock = new Mock<IProductCatalogServices>();
                    productOrderMock.Setup(_ => _.GetOrderedProductsByPartnerAndOrganizationAsync(partnerId, organizationId))
                      .ReturnsAsync(order);
                    services.AddSingleton(productOrderMock.Object);


                    var usersService = new Mock<IUserServices>();

                    var user = new OrigoUser
                    {
                        Id = Guid.NewGuid(),
                        Email = newUser.Email,
                        MobileNumber = newUser.MobileNumber
                    };

                    usersService.Setup(_ => _.AddUserForCustomerAsync(organizationId, newUser, callerId, It.IsAny<bool>()))
                        .ReturnsAsync(user);

                    services.AddSingleton(usersService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);

            var response = await client.PostAsJsonAsync($"/origoapi/v1.0/customers/{organizationId}/users", newUser);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
