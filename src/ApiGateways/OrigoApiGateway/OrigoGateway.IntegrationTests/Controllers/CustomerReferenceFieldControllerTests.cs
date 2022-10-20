using Common.Enums;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class CustomerReferenceFieldControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<CustomerReferenceFieldController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<CustomerReferenceFieldController> _factory;
        private readonly ITestOutputHelper _output;

        public CustomerReferenceFieldControllerTests(OrigoGatewayWebApplicationFactory<CustomerReferenceFieldController> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            factory.ClientOptions.AllowAutoRedirect = false;
        }

        [Fact]
        public async Task GetAllCustomerReferenceFields_ByEndUser()
        {
            var customerId = Guid.NewGuid();
            var email = "enduser@test.io";
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, PredefinedRole.EndUser.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", Permission.SubscriptionManagement.ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", customerId.ToString()));

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
                    var customerReferencesField = new List<OrigoCustomerReferenceField> 
                    {
                        new OrigoCustomerReferenceField()
                        {
                            Id = 1,
                            Name = "Reference Field",
                            Type = "Reference Type"
                        }
                    };
                    subscriptionService.Setup(_ => _.GetAllCustomerReferenceFieldsAsync(customerId)).ReturnsAsync(customerReferencesField);
                    services.AddSingleton(subscriptionService.Object);
                });


            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{customerId}/customer-reference-field");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }

}
