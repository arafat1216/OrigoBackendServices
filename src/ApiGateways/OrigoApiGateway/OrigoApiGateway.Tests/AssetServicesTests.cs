using System;
using System.Collections.Generic;
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
                            ""organizationId"": ""20ef7dbd-a0d1-44c3-b855-19799cceb347"",
                            ""alias"": ""Row row row your boat"",
                            ""note"": ""Order screen protective."",
                            ""description"": ""This device will be used to order new vares."",
                            ""assetTag"": ""Company owned"",
                            ""serialNumber"": ""387493823798278"",
                            ""assetCategoryId"": ""1"",
                            ""assetCategoryName"": ""Mobile phones"",
                            ""brand"": ""iPhone"",
                            ""productName"": ""iPhone 12"",
                            ""lifecycleType"": 1,
                            ""purchaseDate"": ""2021-01-01"",
                            ""managedByDepartmentId"": ""2caba3ef-4f33-4a98-a8aa-abaae3cef6cf"",
                            ""assetHolderId"": ""0a9164a5-df1f-4af2-ba3e-b1aa28d36f81"",
                            ""imei"": [980451084262467,359884912624420],
                            ""macAddress"": ""AA-96-A8-9F-06-82"",
                            ""assetStatus"": 0,
                            ""assetStatusName"": ""NoStatus""
                        }, {
                            ""assetId"": ""ef8710e8-ca5c-4832-ae27-139100d1ae63"",
                            ""organizationId"": ""20ef7dbd-a0d1-44c3-b855-19799cceb347"",
                            ""alias"": ""Ro ro ro din båt"",
                            ""note"": ""Order screen protective."",
                            ""description"": ""This device will be used to order new vares."",
                            ""assetTag"": ""Company owned"",
                            ""serialNumber"": ""555493823798278"",
                            ""assetCategoryId"": ""1"",
                            ""assetCategoryName"": ""Mobile phones"",
                            ""brand"": ""Samsung"",
                            ""productName"": ""Samsung Galaxy S21"",
                            ""lifecycleType"": 1,
                            ""purchaseDate"": ""2021-03-01"",
                            ""managedByDepartmentId"": ""2caba3ef-4f33-4a98-a8aa-abaae3cef6cf"",
                            ""assetHolderId"": ""babed55c-4291-48fe-891e-fb3c18d730e4"",
                            ""imei"": [357879702624426],
                            ""macAddress"": ""AA-96-A8-9F-06-82"",
                            ""assetStatus"": 0,
                            ""assetStatusName"": ""NoStatus""
                        }]
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) {BaseAddress = new Uri("http://localhost")};
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object);

            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService);

            // Act
            var assetsFromUser = await assetService.GetAssetsForUserAsync(new Guid(CUSTOMER_ID), Guid.NewGuid());

            // Assert
            Assert.Equal(2, assetsFromUser.Count);
            Assert.Equal("iPhone",  assetsFromUser[0].Brand);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void UpdateStatusOnAssets()
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
                            ""organizationId"": ""20ef7dbd-a0d1-44c3-b855-19799cceb347"",
                            ""alias"": ""Row row row your boat"",
                            ""note"": ""Order screen protective."",
                            ""description"": ""This device will be used to order new vares."",
                            ""assetTag"": ""Company owned"",
                            ""serialNumber"": ""387493823798278"",
                            ""assetCategoryId"": ""1"",
                            ""assetCategoryName"": ""Mobile phones"",
                            ""brand"": ""iPhone"",
                            ""productName"": ""iPhone 12"",
                            ""lifecycleType"": 1,
                            ""purchaseDate"": ""2021-01-01"",
                            ""managedByDepartmentId"": ""2caba3ef-4f33-4a98-a8aa-abaae3cef6cf"",
                            ""assetHolderId"": ""0a9164a5-df1f-4af2-ba3e-b1aa28d36f81"",
                            ""imei"": [980451084262467,359884912624420],
                            ""macAddress"": ""AA-96-A8-9F-06-82"",
                            ""assetStatus"": 0,
                            ""assetStatusName"": ""NoStatus""
                        }, {
                            ""assetId"": ""ef8710e8-ca5c-4832-ae27-139100d1ae63"",
                            ""organizationId"": ""20ef7dbd-a0d1-44c3-b855-19799cceb347"",
                            ""alias"": ""Ro ro ro din båt"",
                            ""note"": ""Order screen protective."",
                            ""description"": ""This device will be used to order new vares."",
                            ""assetTag"": ""Company owned"",
                            ""serialNumber"": ""555493823798278"",
                            ""assetCategoryId"": ""1"",
                            ""assetCategoryName"": ""Mobile phones"",
                            ""brand"": ""Samsung"",
                            ""productName"": ""Samsung Galaxy S21"",
                            ""lifecycleType"": 1,
                            ""purchaseDate"": ""2021-03-01"",
                            ""managedByDepartmentId"": ""2caba3ef-4f33-4a98-a8aa-abaae3cef6cf"",
                            ""assetHolderId"": ""babed55c-4291-48fe-891e-fb3c18d730e4"",
                            ""imei"": [357879702624426],
                            ""macAddress"": ""AA-96-A8-9F-06-82"",
                            ""assetStatus"": 0,
                            ""assetStatusName"": ""NoStatus""
                        }]
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object);

            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService);
            IList<Guid> assetGuidList = new List<Guid>();
            assetGuidList.Add(new Guid("64add3ea-74ae-48a3-aad7-1a811f25ccdc"));
            assetGuidList.Add(new Guid("ef8710e8-ca5c-4832-ae27-139100d1ae63"));

            // Act
            var updatedAssets = await assetService.UpdateStatusOnAssets(new Guid(CUSTOMER_ID), assetGuidList, 0);

            // Assert
            Assert.Equal(2, updatedAssets.Count);
            Assert.Equal("NoStatus", updatedAssets[0].AssetStatusName);
            Assert.Equal(0, (int) updatedAssets[1].AssetStatus);
        }
    }
}