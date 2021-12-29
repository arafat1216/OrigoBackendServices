using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using OrigoApiGateway.Mappings;
using OrigoApiGateway.Services;
using Xunit;

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
        public async void GetUSer_CheckDisplayName()
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

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, optionsMock.Object, _mapper);

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

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, optionsMock.Object, _mapper);
            var updateUser = new Models.BackendDTO.UpdateUserDTO { FirstName = null, LastName = null, Email = null, EmployeeId = null, UserPreference = null,CallerId = EMPTY_CALLER_ID };
            // Act
            var user = await userService.PutUserAsync(new Guid(CUSTOMER_ID), new Guid(USER_ID), updateUser);

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

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, optionsMock.Object, _mapper);
            var updateUser = new Models.BackendDTO.UpdateUserDTO { FirstName = "Ada", LastName = "Lovelace", Email = "jane.doe@example.com", EmployeeId = "E1", UserPreference = null, CallerId = EMPTY_CALLER_ID };
            // Act
            var user = await userService.PutUserAsync(new Guid(CUSTOMER_ID), new Guid(USER_ID), updateUser);

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

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, optionsMock.Object, _mapper);
            var updateUser = new Models.BackendDTO.UpdateUserDTO { FirstName = "Ada", LastName = "Lovelace", Email = null, EmployeeId = null, UserPreference = null, CallerId = EMPTY_CALLER_ID };
            // Act
            var user = await userService.PatchUserAsync(new Guid(CUSTOMER_ID), new Guid(USER_ID), updateUser);

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

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, optionsMock.Object, _mapper);
            var updateUser = new Models.OrigoUpdateUser { FirstName = "Ada", LastName = "Lovelace", Email = null, EmployeeId = null, UserPreference = null };
            // Act
            var userDeleted = await userService.DeleteUserAsync(new Guid(CUSTOMER_ID), new Guid(USER_ID), true,EMPTY_CALLER_ID);

            // Assert
            Assert.True(userDeleted);
        }
    }
}