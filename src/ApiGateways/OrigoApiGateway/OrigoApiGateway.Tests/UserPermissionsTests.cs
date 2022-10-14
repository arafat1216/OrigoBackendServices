using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq.Protected;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Services;

namespace OrigoApiGateway.Tests;

public class UserPermissionsTests
{
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetUserPermissions_CheckProductPermissions()
    {
        // Arrange
        var organizationId = new Guid("88acaa34-48a0-11ed-9e8d-00155d73c50a");
        var mockFactory = new Mock<IHttpClientFactory>();
        var mockUserPermissionsHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockUserPermissionsHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"
                        [
                        {
                          ""role"": ""Admin"",
                          ""permissionNames"": [
                            ""CanReadCustomer""
                          ],
                          ""accessList"": [
                            ""525b28d4-48a0-11ed-b3f6-00155d73c50a""
                          ],
                          ""userId"": ""677f63ec-48a0-11ed-a76c-00155d73c50a"",
                          ""mainOrganizationId"" : ""88acaa34-48a0-11ed-9e8d-00155d73c50a""
                        }
                      ]
                    ")
                });
        var httpClient = new HttpClient(mockUserPermissionsHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var options = new UserPermissionsConfigurations { ApiPath = @"/assets" };
        var optionsMock = new Mock<IOptions<UserPermissionsConfigurations>>();
        optionsMock.Setup(o => o.Value).Returns(options);

        var productCatalogServicesMock = new Mock<IProductCatalogServices>();
        productCatalogServicesMock.Setup(pc => pc.GetProductPermissionsForOrganizationAsync(organizationId))
            .ReturnsAsync(new List<string> { "EmployeeAccess", "ProductPermission1" });

        var cacheServiceMock = new Mock<ICacheService>();

        cacheServiceMock.Setup(m => m.Get<IEnumerable<UserPermissionsDTO>>(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(value: null);
        cacheServiceMock.Setup(m => m.Get<IEnumerable<string>>(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(value: null);
        cacheServiceMock.Setup(m => m.Save(It.IsAny<string>(), It.IsAny<It.IsAnyType>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult(default(object)));

        var userPermissionService = new UserPermissionService(Mock.Of<ILogger<UserPermissionService>>(),
            mockFactory.Object, optionsMock.Object, Mock.Of<IMapper>(),
            productCatalogServicesMock.Object, cacheServiceMock.Object);

        // Act
        var userIdentity =
            await userPermissionService.GetUserPermissionsIdentityAsync(string.Empty, "jane@doe.com", CancellationToken.None);

        // Assert
        Assert.NotEmpty(userIdentity.Claims);
        Assert.Equal(1, userIdentity.Claims.Count(c => c.Type == "Permissions" && c.Value == "EmployeeAccess"));
        Assert.Equal(1, userIdentity.Claims.Count(c => c.Type == "Permissions" && c.Value == "ProductPermission1"));
    }
}