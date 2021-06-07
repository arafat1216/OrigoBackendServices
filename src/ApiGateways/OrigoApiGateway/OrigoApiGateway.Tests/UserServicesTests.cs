using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using OrigoApiGateway.Services;
using Xunit;

namespace OrigoApiGateway.Tests
{
    public class UserServicesTests
    {
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

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) {BaseAddress = new Uri("http://localhost")};
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new UserConfiguration() { ApiPath = @"/users" };
            var optionsMock = new Mock<IOptions<UserConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, optionsMock.Object);

            // Act
            var user = await userService.GetUserAsync(new Guid(CUSTOMER_ID), new Guid(USER_ID));

            // Assert
            Assert.Equal("Jane Doe",  user.DisplayName);
        }
    }
}