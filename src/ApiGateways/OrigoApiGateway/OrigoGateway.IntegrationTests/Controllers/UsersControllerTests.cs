﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

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

                    
                    userService.Setup(_ => _.GetUsersCountAsync(organizationId, null, departmentId))
                        .Returns(Task.FromResult(2));
                    services.AddSingleton(userService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{organizationId}/users/count");

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
                    userService.Setup(_ => _.GetUsersCountAsync(organization_NOTreadRights, filter, organization_readRights))
                        .Returns(Task.FromResult(2));
                    services.AddSingleton(userService.Object);
                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{organization_NOTreadRights}/users/count");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
