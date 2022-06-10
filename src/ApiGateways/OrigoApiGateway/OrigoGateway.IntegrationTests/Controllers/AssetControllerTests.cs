using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace OrigoGateway.IntegrationTests.Controllers;

public class AssetControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<AssetsController>>
{
    private readonly OrigoGatewayWebApplicationFactory<AssetsController> _factory;
    private readonly ITestOutputHelper _output;

    public AssetControllerTests(OrigoGatewayWebApplicationFactory<AssetsController> factory, ITestOutputHelper output)
    {
        _factory = factory;
        factory.ClientOptions.AllowAutoRedirect = false;
        _output = output;
    }

    public static IEnumerable<object[]> EmailAccess =>
        new List<object[]>
        {
            new object[] { "unknown@test.io", HttpStatusCode.Forbidden },
            new object[] { "admin@test.io", HttpStatusCode.Forbidden },
            new object[] { "systemadmin@test.io", HttpStatusCode.OK }
        };

    [Theory]
    [MemberData(nameof(EmailAccess))]
    public async Task Get_SecurePageAccessibleOnlyByAdminUsers(string email, HttpStatusCode expected)
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                    options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.DefaultScheme, options => { options.Email = email; });
                var assetService = new Mock<IAssetServices>();
                var customerAssetCount =
                    new List<CustomerAssetCount> { new() { OrganizationId = Guid.NewGuid(), Count = 12 } };
                assetService.Setup(_ => _.GetAllCustomerAssetsCountAsync())
                    .Returns(Task.FromResult(customerAssetCount as IList<CustomerAssetCount>));
                services.AddSingleton(assetService.Object);
            });
        }).CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
        var response = await client.GetAsync("/origoapi/v1.0/assets/customers/count");

        Assert.Equal(expected, response.StatusCode);
    }
    [Fact]
    public async Task GetAssetsForCustomerAsync_SecurePageAccessibleToAllSystemAdminsWithRightPermissionNames()
    {
        var permissionsIdentity = new ClaimsIdentity();
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "systemadmin@test.io"));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, "systemadmin@test.io"));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "SystemAdmin"));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
        permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
        permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadAsset"));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var userPermissionServiceMock = new Mock<IUserPermissionService>();
                userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "systemadmin@test.io", CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                services.AddSingleton(userPermissionServiceMock.Object);

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                    options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.DefaultScheme, options => {
                        options.Email = "systemadmin@test.io";
                    });
                var customersAssets = new PagedModel<HardwareSuperType>()
                {
                    CurrentPage = 1,
                    Items = new List<HardwareSuperType>()
                    {
                        new HardwareSuperType
                        {
                            DepartmentName = "",
                            Alias = "",
                            AssetCategoryId = 1,
                            Description = "",
                            AssetStatus = AssetLifecycleStatus.InUse
                        }
                    },
                    PageSize = 1,
                    TotalItems = 1,
                    TotalPages = 1,
                };

                var assetService = new Mock<IAssetServices>();
                assetService.Setup(_ => _.GetAssetsForCustomerAsync(Guid.Parse("6c514552-ea67-48c8-91ec-83c2b16248ee"), It.IsAny<FilterOptionsForAsset>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(customersAssets));
                services.AddSingleton(assetService.Object);
            });
        }).CreateClient();

       
        var response = await client.GetAsync("/origoapi/v1.0/assets/customers/6c514552-ea67-48c8-91ec-83c2b16248ee");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    }

    [Fact]
    public async Task GetLifeCycle_AccessibleToAllSystemAdminsWithRightPermissionNames()
    {
        var permissionsIdentity = new ClaimsIdentity();
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "systemadmin@test.io"));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, "systemadmin@test.io"));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "SystemAdmin"));
        permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
        permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
        permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadAsset"));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var userPermissionServiceMock = new Mock<IUserPermissionService>();
                userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "systemadmin@test.io", CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                services.AddSingleton(userPermissionServiceMock.Object);

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                    options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.DefaultScheme, options => {
                        options.Email = "systemadmin@test.io";
                    });
                var assetService = new Mock<IAssetServices>();
                var customerService = new Mock<ICustomerServices>();

                var customerLifeCycleList =
                   new List<LifeCycleSetting> { new() { 
                       CustomerId = Guid.NewGuid(), 
                       AssetCategoryId = 1,
                       AssetCategoryName = "Catagory",
                       BuyoutAllowed = true,
                       Currency = "no",
                       MinBuyoutPrice = Decimal.Zero
                   } };

                customerService.Setup(_ => _.GetCurrencyByCustomer(Guid.Parse("6c514552-ea67-48c8-91ec-83c2b16248ee"))).ReturnsAsync("NO");
                assetService.Setup(_ => _.GetLifeCycleSettingByCustomer(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(customerLifeCycleList as IList<LifeCycleSetting>);


                services.AddSingleton(customerService.Object);

                services.AddSingleton(assetService.Object);
            });
        }).CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthenticationHandler.DefaultScheme);
        var response = await client.GetAsync("/origoapi/v1.0/assets/customers/6c514552-ea67-48c8-91ec-83c2b16248ee/lifecycle-setting");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    }
}