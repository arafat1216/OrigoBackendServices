using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Okta.AspNetCore;
using OrigoApiGateway.Services;

namespace OrigoGateway.IntegrationTests
{
    public class OrigoGatewayWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        public Guid ORGANIZATION_ID = Guid.Parse("00000000-0000-0000-0000-000000000001");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var userPermissionServiceMock = new Mock<IUserPermissionService>();
                userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "systemadmin@test.io", CancellationToken.None)).Returns(Task.FromResult(MockClaimsIdentity("systemadmin@test.io")));
                userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "admin@test.io", CancellationToken.None)).Returns(Task.FromResult(MockClaimsIdentity("admin@test.io")));
                userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "unknown@test.io", CancellationToken.None)).Returns(Task.FromResult(new ClaimsIdentity()));
                services.AddSingleton(userPermissionServiceMock.Object);
                services.AddAuthorization(options =>
                {
                    // One static policy - All users must be authenticated
                    options.DefaultPolicy = new AuthorizationPolicyBuilder("Test")
                        .RequireAuthenticatedUser()
                        .Build();
                });
            });
            base.ConfigureWebHost(builder);

        }

        private ClaimsIdentity MockClaimsIdentity(string userName)
        {
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userName));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, userName));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, GetRoleFromEmail(userName)));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            return permissionsIdentity;
        }

        private static string GetRoleFromEmail(string email) =>
            email switch
            {
                "systemadmin@test.io" => "SystemAdmin",
                "admin@test.io" => "Admin",
                _ => "EndUser"
            };
    }
}
