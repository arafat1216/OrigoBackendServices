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
    public class AssetServicesTests
    {
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAssetsForUser_ForUserOne_CheckCount()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
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
                        [{
                            ""assetId"": ""64add3ea-74ae-48a3-aad7-1a811f25ccdc"",
                            ""customerId"": ""20ef7dbd-a0d1-44c3-b855-19799cceb347"",
                            ""serialNumber"": ""387493823798278"",
                            ""assetCategoryName"": ""Mobile phones"",
                            ""brand"": ""iPhone"",
                            ""model"": ""iPhone 12"",
                            ""lifecycleType"": 1,
                            ""purchaseDate"": ""2021-01-01"",
                            ""managedByDepartmentId"": ""2caba3ef-4f33-4a98-a8aa-abaae3cef6cf"",
                            ""assetHolderId"": ""0a9164a5-df1f-4af2-ba3e-b1aa28d36f81"",
                            ""isActive"": true
                        }, {
                            ""assetId"": ""ef8710e8-ca5c-4832-ae27-139100d1ae63"",
                            ""customerId"": ""20ef7dbd-a0d1-44c3-b855-19799cceb347"",
                            ""serialNumber"": ""555493823798278"",
                            ""assetCategoryName"": ""Mobile phones"",
                            ""brand"": ""Samsung"",
                            ""model"": ""Samsung Galaxy S21"",
                            ""lifecycleType"": 1,
                            ""purchaseDate"": ""2021-03-01"",
                            ""managedByDepartmentId"": ""2caba3ef-4f33-4a98-a8aa-abaae3cef6cf"",
                            ""assetHolderId"": ""babed55c-4291-48fe-891e-fb3c18d730e4"",
                            ""isActive"": true
                        }]
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) {BaseAddress = new Uri("http://localhost")};
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiBaseUrl = "https://localhost", ApiPort = "44300", ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object);

            // Act
            var assetsFromUser = await assetService.GetAssetsForUserAsync(new Guid(CUSTOMER_ID), Guid.NewGuid());

            // Assert
            Assert.Equal(2, assetsFromUser.Count);
            Assert.Equal("iPhone",  assetsFromUser[0].Brand);
        }
    }
}