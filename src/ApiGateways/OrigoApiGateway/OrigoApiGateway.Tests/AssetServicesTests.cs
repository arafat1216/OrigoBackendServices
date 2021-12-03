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
using OrigoApiGateway.Models;
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

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
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
            Assert.Equal("iPhone", (assetsFromUser[0] as OrigoMobilePhone).Brand);
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
            Assert.Equal("NoStatus", (updatedAssets[0] as OrigoMobilePhone).AssetStatusName);
            Assert.Equal(0, (int)(updatedAssets[1] as OrigoMobilePhone).AssetStatus);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void CreateLabelsForCustomer()
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
                            ""Id"": ""94D22A10-FC47-4F23-A524-B30D022DD8AF"",
                            ""text"":  ""Manager"",
                            ""color"":  4,
                            ""colorName"": ""Red""
                         },
                         {
                            ""Id"": ""53D6241B-7297-44E4-857B-9EEA69BEB174"",
                            ""text"":  ""Department"",
                            ""color"": 3,
                            ""colorName"": ""Orange""
                         },
                         {
                            ""Id"": ""484289DA-FA13-4629-8AAA-CF988F905681"",
                            ""text"":  ""Customer service"",
                            ""color"":  5,
                            ""colorName"": ""Gray""
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

            IList<NewLabel> newLabels = new List<NewLabel>();
            newLabels.Add(new NewLabel { Text = "Manager", Color = Common.Enums.LabelColor.Red });
            newLabels.Add(new NewLabel { Text = "Department", Color = Common.Enums.LabelColor.Orange });
            newLabels.Add(new NewLabel { Text = "Customer service", Color = Common.Enums.LabelColor.Gray });

            // Act
            IList<Label> createdLabels = await assetService.CreateLabelsForCustomerAsync(new Guid(CUSTOMER_ID), newLabels);

            // Assert
            Assert.Equal(3, createdLabels.Count);
            Assert.Equal("Manager", createdLabels[0].Text);
            Assert.Equal(3,  (int) createdLabels[1].Color);
            Assert.Equal("Gray", createdLabels[2].ColorName);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void DeleteLabelsForCustomer()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string LABEL_ONE_ID = "94D22A10-FC47-4F23-A524-B30D022DD8AF";
            const string LABEL_TWO_ID = "53D6241B-7297-44E4-857B-9EEA69BEB174";
            const string LABEL_THREE_ID = "484289DA-FA13-4629-8AAA-CF988F905681";

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
                           ""Id"" : ""94D22A10-FC47-4F23-A524-B30D022DD8AF"",
                            ""text"":  ""Manager"",
                            ""color"":  4,
                            ""colorName"": ""Red""
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

            IList<Guid> guidList = new List<Guid>();
            guidList.Add(new Guid(LABEL_THREE_ID));
            guidList.Add(new Guid(LABEL_TWO_ID));


            // Act
            IList<Label> remainingLabels = await assetService.DeleteCustomerLabelsAsync(new Guid(CUSTOMER_ID), guidList);

            // Assert
            Assert.Equal(1, remainingLabels.Count);
            Assert.Equal("Manager", remainingLabels[0].Text);
            Assert.Equal(4, (int)remainingLabels[0].Color);
            Assert.Equal("Red", remainingLabels[0].ColorName);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void UpdateLabelsForCustomer()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string LABEL_ONE_ID = "94D22A10-FC47-4F23-A524-B30D022DD8AF";
            const string LABEL_TWO_ID = "53D6241B-7297-44E4-857B-9EEA69BEB174";
            const string LABEL_THREE_ID = "484289DA-FA13-4629-8AAA-CF988F905681";

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
                            ""Id"": ""94D22A10-FC47-4F23-A524-B30D022DD8AF"",
                            ""text"":  ""Administrator"",
                            ""color"":  0,
                            ""colorName"": ""Blue""
                         },
                         {
                            ""Id"": ""53D6241B-7297-44E4-857B-9EEA69BEB174"",
                            ""text"":  ""Assistant"",
                            ""color"": 2,
                            ""colorName"": ""Light_blue""
                         },
                         {
                            ""Id"": ""484289DA-FA13-4629-8AAA-CF988F905681"",
                            ""text"":  ""Customer service"",
                            ""color"":  1,
                            ""colorName"": ""Green""
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

            IList<Label> labels = new List<Label>();
            labels.Add(new Label { Id = new Guid(LABEL_ONE_ID), Text = "Administrator", Color = Common.Enums.LabelColor.Blue });
            labels.Add(new Label { Id = new Guid(LABEL_TWO_ID), Text = "Assistant", Color = Common.Enums.LabelColor.Light_blue });

            

            // Act
            IList<Label> labelsResult = await assetService.UpdateLabelsForCustomerAsync(new Guid(CUSTOMER_ID), labels);

            // Assert
            Assert.Equal(3, labelsResult.Count);
            Assert.Equal("Administrator", labelsResult[0].Text);
            Assert.Equal(2, (int)labelsResult[1].Color);
            Assert.Equal("Green", labelsResult[2].ColorName);
        }

    }
}