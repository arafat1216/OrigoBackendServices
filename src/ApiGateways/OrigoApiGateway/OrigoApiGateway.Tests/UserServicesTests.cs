using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq.Protected;
using OrigoApiGateway.Mappings;
using OrigoApiGateway.Services;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OrigoApiGateway.Tests
{
    public class UserServicesTests
    {
        private static IMapper _mapper;
        private Guid EMPTY_CALLER_ID = Guid.Empty;

        public UserServicesTests()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddMaps(Assembly.GetAssembly(typeof(OrigoUserProfile)));
                });
                _mapper = mappingConfig.CreateMapper();
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetUser_CheckDisplayName()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string USER_ID = "37993d3e-c529-11eb-a5a9-00155dc5d5a8";
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                            ""firstName"": ""Jane"",
                            ""lastName"": ""Doe"",
                            ""email"": ""jane.doe@example.com"",
                            ""mobileNumber"": ""99999999"",
                            ""employeeId"": ""E1"",
                            ""customerName"": ""ACME""
                        }
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new UserConfiguration() { ApiPath = @"/users" };
            var optionsMock = new Mock<IOptions<UserConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), mockFactory.Object, optionsMock.Object, _mapper, Mock.Of<IProductCatalogServices>());

            // Act
            var user = await userService.GetUserAsync(new Guid(CUSTOMER_ID), new Guid(USER_ID));

            // Assert
            Assert.Equal("Jane Doe", user.DisplayName);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void PutUser_all_null()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string USER_ID = "37993d3e-c529-11eb-a5a9-00155dc5d5a8";
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                            ""firstName"": """",
                            ""lastName"": """",
                            ""email"": """",
                            ""mobileNumber"": """",
                            ""employeeId"": """",
                            ""organizationName"": """"
                        }
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new UserConfiguration() { ApiPath = @"/users" };
            var optionsMock = new Mock<IOptions<UserConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), mockFactory.Object, optionsMock.Object, _mapper, Mock.Of<IProductCatalogServices>());
            var updateUser = new Models.OrigoUpdateUser { FirstName = null, LastName = null, Email = null, EmployeeId = null, MobileNumber = null, UserPreference = null };
            // Act
            var user = await userService.PutUserAsync(new Guid(CUSTOMER_ID), new Guid(USER_ID), updateUser, EMPTY_CALLER_ID);

            // Assert
            Assert.Equal("", user.FirstName);
            Assert.Equal("", user.LastName);
            Assert.Equal("", user.Email);
            Assert.Equal("", user.EmployeeId);
            Assert.Equal("", user.UserPreference.Language);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void PutUser_not_null()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string USER_ID = "37993d3e-c529-11eb-a5a9-00155dc5d5a8";
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                            ""firstName"": ""Ada"",
                            ""lastName"": ""Lovelace"",
                            ""email"": ""jane.doe@example.com"",
                            ""mobileNumber"": ""99999999"",
                            ""employeeId"": ""E1"",
                            ""organizationName"": ""ACME""
                        }
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new UserConfiguration() { ApiPath = @"/users" };
            var optionsMock = new Mock<IOptions<UserConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), mockFactory.Object, optionsMock.Object, _mapper, Mock.Of<IProductCatalogServices>());
            var updateUser = new Models.OrigoUpdateUser { FirstName = "Ada", LastName = "Lovelace", Email = "jane.doe@example.com", EmployeeId = "E1", MobileNumber = "+4795554613", UserPreference = null };
            // Act
            var user = await userService.PutUserAsync(new Guid(CUSTOMER_ID), new Guid(USER_ID), updateUser, EMPTY_CALLER_ID);

            // Assert
            Assert.Equal("Ada Lovelace", user.DisplayName);
            Assert.Equal("Lovelace", user.LastName);
            Assert.Equal("jane.doe@example.com", user.Email);
            Assert.Equal("E1", user.EmployeeId);
            Assert.Equal("ACME", user.OrganizationName);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void Patch_user()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string USER_ID = "37993d3e-c529-11eb-a5a9-00155dc5d5a8";
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                            ""firstName"": ""Ada"",
                            ""lastName"": ""Lovelace"",
                            ""email"": ""jane.doe@example.com"",
                            ""mobileNumber"": ""99999999"",
                            ""employeeId"": ""E1"",
                            ""organizationName"": ""ACME""
                        }
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new UserConfiguration() { ApiPath = @"/users" };
            var optionsMock = new Mock<IOptions<UserConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), mockFactory.Object, optionsMock.Object, _mapper, Mock.Of<IProductCatalogServices>());
            var updateUser = new Models.OrigoUpdateUser { FirstName = "Ada", LastName = "Lovelace", Email = null, EmployeeId = null, MobileNumber = null, UserPreference = null };
            // Act
            var user = await userService.PatchUserAsync(new Guid(CUSTOMER_ID), new Guid(USER_ID), updateUser, EMPTY_CALLER_ID);

            // Assert
            Assert.Equal("Ada Lovelace", user.DisplayName);
            Assert.Equal("Ada", user.FirstName);
            Assert.Equal("Lovelace", user.LastName);
            Assert.Equal("jane.doe@example.com", user.Email);
            Assert.Equal("E1", user.EmployeeId);
            Assert.Equal("ACME", user.OrganizationName);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void Patch_user_UpdateUserPreferences()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string USER_ID = "37993d3e-c529-11eb-a5a9-00155dc5d5a8";
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                            ""firstName"": ""Ada"",
                            ""lastName"": ""Lovelace"",
                            ""email"": ""jane.doe@example.com"",
                            ""mobileNumber"": ""99999999"",
                            ""employeeId"": ""E1"",
                            ""organizationName"": ""ACME"",
                            ""userPreference"":
                            {
                                ""IsAssetTileClosed"":true
                            }
                        }
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new UserConfiguration() { ApiPath = @"/users" };
            var optionsMock = new Mock<IOptions<UserConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), mockFactory.Object, optionsMock.Object, _mapper, Mock.Of<IProductCatalogServices>());
            var updateUser = new Models.OrigoUpdateUser {UserPreference = new Models.UserPreference() { IsAssetTileClosed = true } };
            // Act
            var user = await userService.PatchUserAsync(new Guid(CUSTOMER_ID), new Guid(USER_ID), updateUser, EMPTY_CALLER_ID);

            // Assert
            Assert.Equal(true, user.UserPreference.IsAssetTileClosed);
            Assert.Null(user.UserPreference.IsSubscriptionTileClosed);
        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void Delete_user()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string USER_ID = "37993d3e-c529-11eb-a5a9-00155dc5d5a8";
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                            ""firstName"": ""Jane"",
                            ""lastName"": ""Lovelace"",
                            ""email"": ""jane.doe@example.com"",
                            ""mobileNumber"": ""99999999"",
                            ""employeeId"": ""E1"",
                            ""customerName"": ""ACME""
                        }
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new UserConfiguration() { ApiPath = @"/users" };
            var optionsMock = new Mock<IOptions<UserConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), mockFactory.Object, optionsMock.Object, _mapper, Mock.Of<IProductCatalogServices>());
            var updateUser = new Models.OrigoUpdateUser { FirstName = "Ada", LastName = "Lovelace", Email = null, EmployeeId = null, MobileNumber = null, UserPreference = null };
            // Act
            var userDeleted = await userService.DeleteUserAsync(new Guid(CUSTOMER_ID), new Guid(USER_ID), true, EMPTY_CALLER_ID);

            // Assert
            Assert.NotNull(userDeleted);
        }

        [Fact]
        [Trait("", "")]
        public async Task GetUserWithPermissions_Without_CustomerId()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string USER_ID = "37993d3e-c529-11eb-a5a9-00155dc5d5a8";
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6""
                        }
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new UserConfiguration() { ApiPath = @"/users" };
            var optionsMock = new Mock<IOptions<UserConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), mockFactory.Object, optionsMock.Object, _mapper, Mock.Of<IProductCatalogServices>());
            var permissions = new List<string> { "A", "B", "C" };

            // Act
            var user = await userService.GetUserWithPermissionsAsync(null, new Guid(CUSTOMER_ID), new Guid(USER_ID), permissions, new List<string>());

            // Assert
            Assert.Contains("A", user!.PermissionNames);
            Assert.Contains("B", user!.PermissionNames);
            Assert.Contains("C", user!.PermissionNames);
        }

        [Fact]
        [Trait("", "")]
        public async Task GetUserWithPermissions_With_CustomerId()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string MAIN_ORGANIZATION_ID = "22ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string USER_ID = "37993d3e-c529-11eb-a5a9-00155dc5d5a8";
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6""
                        }
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new UserConfiguration() { ApiPath = @"/users" };
            var optionsMock = new Mock<IOptions<UserConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var productCatalogServicesMock = new Mock<IProductCatalogServices>();
            var customerId = new Guid(CUSTOMER_ID);
            var mainOrganizationId = new Guid(MAIN_ORGANIZATION_ID);
            productCatalogServicesMock.Setup(pc => pc.GetProductPermissionsForOrganizationAsync(mainOrganizationId)).ReturnsAsync(new List<string> { "G", "H" });
            productCatalogServicesMock.Setup(pc => pc.GetProductPermissionsForOrganizationAsync(customerId)).ReturnsAsync(new List<string> { "D", "E" });

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), mockFactory.Object, optionsMock.Object, _mapper, productCatalogServicesMock.Object);
            var permissions = new List<string> { "A", "B", "C", "D", "E" };

            // Act
            var user = await userService.GetUserWithPermissionsAsync(new Guid(MAIN_ORGANIZATION_ID), new Guid(CUSTOMER_ID), new Guid(USER_ID), permissions, new List<string>());

            // Assert
            Assert.Contains("A", user!.PermissionNames);
            Assert.Contains("B", user!.PermissionNames);
            Assert.Contains("C", user!.PermissionNames);
            Assert.Contains("G", user!.PermissionNames);
            Assert.Contains("H", user!.PermissionNames);
            Assert.Equal(5, user!.PermissionNames.Count);
        }
        
        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetUserInfo_Test()
        {
            // Arrange
            const string USER_NAME = "test@example.com";
            Guid USER_ID = Guid.Empty;
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""userId"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                            ""organizationId"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                            ""departmentId"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                            ""userName"": ""jane.doe@example.com""
                        }
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new UserConfiguration() { ApiPath = @"/users" };
            var optionsMock = new Mock<IOptions<UserConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), mockFactory.Object, optionsMock.Object, _mapper, Mock.Of<IProductCatalogServices>());

            // Act
            var user = await userService.GetUserInfo(USER_NAME, USER_ID);

            // Assert
            Assert.NotNull(user.UserName);
            Assert.NotEmpty(user.UserName);
        }
        
        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetUserInfo_Handle_404_User_Not_Found()
        {
            // Arrange
            const string USER_NAME = "test@example.com";
            Guid USER_ID = Guid.Empty;
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new UserConfiguration() { ApiPath = @"/users" };
            var optionsMock = new Mock<IOptions<UserConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), mockFactory.Object, optionsMock.Object, _mapper, Mock.Of<IProductCatalogServices>());

            // Act
            var user = await userService.GetUserInfo(USER_NAME, USER_ID);

            // Assert
            Assert.Null(user.UserName);
            Assert.Equal(Guid.Empty, user.UserId);
            Assert.Equal(Guid.Empty, user.OrganizationId);
            Assert.Equal(Guid.Empty, user.DepartmentId);
        }
    }
}