using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Mappings;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Services;
using Xunit;

namespace OrigoApiGateway.Tests
{
    public class AssetServicesTests
    {
        private static IMapper _mapper;

        public AssetServicesTests()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddMaps(Assembly.GetAssembly(typeof(LabelProfile)));
                });
                _mapper = mappingConfig.CreateMapper();
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetBaseMinBuyoutPrice()
        {
            // Arrange
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
                        [
                        {
                          ""Country"": ""NO"",
                          ""AssetCategoryId"": ""1"",
                          ""Amount"": ""500""
                        },
                        {
                          ""Country"": ""NO"",
                          ""AssetCategoryId"": ""2"",
                          ""Amount"": ""500""
                        }
                      ]
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);
            var userPermissionOptionsMock = new Mock<IOptions<UserPermissionsConfigurations>>();
            var userPermissionService = new UserPermissionService(Mock.Of<ILogger<UserPermissionService>>(), httpClient, userPermissionOptionsMock.Object, _mapper);

            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, userPermissionService, _mapper, departmentService);

            // Act
            var minBuyoutPrices = await assetService.GetBaseMinBuyoutPrice();

            // Assert
            Assert.Equal(2, minBuyoutPrices.Count);
        }
        
        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAssetsForUser_ForUserOne_CheckCount()
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
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);
            var userPermissionOptionsMock = new Mock<IOptions<UserPermissionsConfigurations>>();
            var userPermissionService = new UserPermissionService(Mock.Of<ILogger<UserPermissionService>>(), httpClient, userPermissionOptionsMock.Object, _mapper);

            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, userPermissionService, _mapper, departmentService);
            // Act
            var assetsFromUser = await assetService.GetAssetsForUserAsync(new Guid(CUSTOMER_ID), Guid.NewGuid());

            // Assert
            Assert.Equal(2, assetsFromUser.Count);
            Assert.Equal("iPhone", (assetsFromUser[0] as OrigoMobilePhone)!.Brand);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAvailableAssetForCustomer()
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
                        @"{""items"":[{""id"":""6c38b551-a5c2-4f53-8df8-221bf8485c61"",""organizationId"":""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",""alias"":""alias_1"",""note"":"""",""description"":"""",""assetTag"":"""",""assetCategoryId"":1,""assetCategoryName"":""Mobile phone"",""brand"":""Apple"",""productName"":""Apple iPhone 8"",""lifecycleType"":0,""lifecycleName"":""NoLifecycle"",""paidByCompany"":0,""bookValue"":0,""buyoutPrice"":0.00,""purchaseDate"":""0001-01-01T00:00:00"",""createdDate"":""2022-04-28T14:18:11.9616155"",""managedByDepartmentId"":null,""assetHolderId"":""6d16a4cb-4733-44de-b23b-0eb9e8ae6590"",""assetStatus"":3,""assetStatusName"":""Available"",""labels"":[],""serialNumber"":""123456789012364"",""imei"":[546366434558702],""macAddress"":""487027C99FA1"",""orderNumber"":"""",""productId"":"""",""invoiceNumber"":"""",""transactionId"":"""",""isPersonal"":false},{""id"":""bdb4c26c-33fd-40d7-a237-e74728609c1c"",""organizationId"":""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",""alias"":""alias_3"",""note"":"""",""description"":"""",""assetTag"":"""",""assetCategoryId"":1,""assetCategoryName"":""Mobile phone"",""brand"":""Apple"",""productName"":""iPhone 11 Pro"",""lifecycleType"":0,""lifecycleName"":""NoLifecycle"",""paidByCompany"":0,""bookValue"":0,""buyoutPrice"":0.00,""purchaseDate"":""0001-01-01T00:00:00"",""createdDate"":""2022-04-28T14:18:11.9628902"",""managedByDepartmentId"":null,""departmentName"":null,""assetHolderId"":""6d16a4cb-4733-44de-b23b-0eb9e8ae6590"",""assetHolderName"":""Jens Andersen"",""assetStatus"":3,""assetStatusName"":""Available"",""labels"":[],""serialNumber"":""123456789012397"",""imei"":[512217111821624],""macAddress"":""840F1D0C06AB"",""orderNumber"":"""",""productId"":"""",""invoiceNumber"":"""",""transactionId"":"""",""isPersonal"":false},{""id"":""4315bba8-698f-4ddd-aee2-82554c91721f"",""organizationId"":""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",""alias"":""alias_4"",""note"":"""",""description"":"""",""assetTag"":"""",""assetCategoryId"":1,""assetCategoryName"":""Mobile phone"",""brand"":""Apple"",""productName"":""iPhone 11 Pro"",""lifecycleType"":0,""lifecycleName"":""NoLifecycle"",""paidByCompany"":0,""bookValue"":0,""buyoutPrice"":0.00,""purchaseDate"":""0001-01-01T00:00:00"",""createdDate"":""2022-04-28T14:18:11.9628964"",""managedByDepartmentId"":""6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72"",""assetHolderId"":""6d16a4cb-4733-44de-b23b-0eb9e8ae6590"",""assetStatus"":3,""assetStatusName"":""Available"",""labels"":[],""serialNumber"":""123456789012397"",""imei"":[512217111821624],""macAddress"":""840F1D0C06AB"",""orderNumber"":"""",""productId"":"""",""invoiceNumber"":"""",""transactionId"":"""",""isPersonal"":false}],""currentPage"":1,""totalItems"":3,""totalPages"":1,""pageSize"":1000}
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var mockUser = new Mock<OrigoUser>();
            var mockUserService = new Mock<IUserServices>();
            mockUserService.Setup(p => p.GetUserAsync(Guid.Empty,Guid.Empty).Result).Returns(mockUser.Object);

            var mockDepartment = new Mock<OrigoDepartment>();
            var mockDepartmentService = new Mock<IDepartmentsServices>();
            mockDepartmentService.Setup(p => p.GetDepartmentAsync(Guid.Empty, Guid.Empty).Result).Returns(mockDepartment.Object);

            var mockUserPermissionService = new Mock<IUserPermissionService>();

            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, mockUserService.Object, mockUserPermissionService.Object, _mapper, mockDepartmentService.Object);


            // Act
            var pagedAsset = await assetService.GetAssetsForCustomerAsync(new Guid(CUSTOMER_ID), new FilterOptionsForAsset());

            // Assert
            Assert.True(pagedAsset.TotalItems == 3);
            Assert.True(pagedAsset.Items.Count == 3);
        }


        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task UpdateStatusOnAssets()
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
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            IList<Guid> assetGuidList = new List<Guid>();
            assetGuidList.Add(new Guid("64add3ea-74ae-48a3-aad7-1a811f25ccdc"));
            assetGuidList.Add(new Guid("ef8710e8-ca5c-4832-ae27-139100d1ae63"));

            var data = new UpdateAssetsStatus
            {
                AssetGuidList = assetGuidList,
                AssetStatus =  1
            };

            // Act
            var updatedAssets = await assetService.UpdateStatusOnAssets(new Guid(CUSTOMER_ID), data, Guid.Empty);

            // Assert
            Assert.Equal(2, updatedAssets.Count);
            Assert.Equal("NoStatus", (updatedAssets[0] as OrigoMobilePhone)!.AssetStatusName);
            Assert.Equal(0, (int)(updatedAssets[1] as OrigoMobilePhone)!.AssetStatus);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task CreateLabelsForCustomer()
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
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            IList<NewLabel> newLabels = new List<NewLabel>();
            newLabels.Add(new NewLabel { Text = "Manager", Color = LabelColor.Red });
            newLabels.Add(new NewLabel { Text = "Department", Color = LabelColor.Orange });
            newLabels.Add(new NewLabel { Text = "Customer service", Color = LabelColor.Gray });

            var data = new AddLabelsData
            {
                NewLabels = newLabels,
                CallerId = Guid.Empty
            };

            // Act
            var createdLabels = await assetService.CreateLabelsForCustomerAsync(new Guid(CUSTOMER_ID), data);

            // Assert
            Assert.Equal(3, createdLabels.Count);
            Assert.Equal("Manager", createdLabels[0].Text);
            Assert.Equal(3,  (int) createdLabels[1].Color);
            Assert.Equal("Gray", createdLabels[2].ColorName);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task DeleteLabelsForCustomer()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
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
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            



            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            IList<Guid> guidList = new List<Guid>();
            guidList.Add(new Guid(LABEL_THREE_ID));
            guidList.Add(new Guid(LABEL_TWO_ID));

            var data = new DeleteCustomerLabelsData
            {
                LabelGuids = guidList,
                CallerId = Guid.Empty
            };

            // Act
            var remainingLabels = await assetService.DeleteCustomerLabelsAsync(new Guid(CUSTOMER_ID), data);

            // Assert
            Assert.Equal(1, remainingLabels.Count);
            Assert.Equal("Manager", remainingLabels[0].Text);
            Assert.Equal(4, (int)remainingLabels[0].Color);
            Assert.Equal("Red", remainingLabels[0].ColorName);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task MakeAssetAvailableAsync()
        {
            // Arrange
            const string CUSTOMER_ID = "cab4bb77-3471-4ab3-ae5e-2d4fce450f36";
            

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains("/make-available") && x.Method == HttpMethod.Post
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""80665d26-90b4-4a3a-a20d-686b64466f32"",
                            ""organizationId"": ""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",
                            ""alias"": ""alias_2"",
                            ""note"": """",
                            ""description"": """",
                            ""assetTag"": """",
                            ""assetCategoryId"": 1,
                            ""assetCategoryName"": ""Mobile phone"",
                            ""brand"": ""Samsung"",
                            ""productName"": ""Samsung Galaxy S21"",
                            ""lifecycleType"": 2,
                            ""lifecycleName"": ""Transactional"",
                            ""paidByCompany"": 0,
                            ""currencyCode"": 0,
                            ""bookValue"": 0,
                            ""buyoutPrice"": 0.00,
                            ""purchaseDate"": ""0001-01-01T00:00:00"",
                            ""createdDate"": ""2022-05-10T08:11:17.9941683Z"",
                            ""managedByDepartmentId"": ""6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72"",
                            ""assetHolderId"": null,
                            ""assetStatus"": 3,
                            ""assetStatusName"": ""Available"",
                            ""labels"": [],
                            ""serialNumber"": ""123456789012399"",
                            ""imei"": [
                                512217111821626
                            ],
                            ""macAddress"": ""840F1D0C06AD"",
                            ""orderNumber"": """",
                            ""productId"": """",
                            ""invoiceNumber"": """",
                            ""transactionId"": """",
                            ""isPersonal"": false
                        }
                    ")
                });
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.Method == HttpMethod.Get
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                          ""id"": ""4e7413da-54c9-4f79-b882-f66ce48e5074"",
                          ""organizationId"": ""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",
                          ""alias"": ""alias_0"",
                          ""note"": """",
                          ""description"": """",
                          ""assetTag"": """",
                          ""assetCategoryId"": 1,
                          ""assetCategoryName"": ""Mobile phone"",
                          ""brand"": ""Samsung"",
                          ""productName"": ""Samsung Galaxy S20"",
                          ""lifecycleType"": 2,
                          ""lifecycleName"": ""Transactional"",
                          ""paidByCompany"": 0,
                          ""bookValue"": 0,
                          ""buyoutPrice"": 0,
                          ""purchaseDate"": ""0001-01-01T00:00:00"",
                          ""createdDate"": ""2022-05-11T21:30:02.1795951Z"",
                          ""managedByDepartmentId"": null,
                          ""assetHolderId"": ""6d16a4cb-4733-44de-b23b-0eb9e8ae6590"",
                          ""assetStatus"": 9,
                          ""assetStatusName"": ""InUse"",
                          ""labels"": [
                            {
                              ""id"": ""c553ae5b-6a3f-49c2-8d3e-8644d8f7e975"",
                              ""text"": ""Label1"",
                              ""color"": 0,
                              ""colorName"": ""Blue""
                            },
                            {
                              ""id"": ""fa0c43b6-1101-4698-bad9-2fb58b2032b3"",
                              ""text"": ""CompanyOne"",
                              ""color"": 2,
                              ""colorName"": ""Lightblue""
                            }
                          ],
                          ""serialNumber"": ""123456789012345"",
                          ""imei"": [
                            500119468586675
                          ],
                          ""macAddress"": ""B26EDC46046B"",
                          ""orderNumber"": """",
                          ""productId"": """",
                          ""invoiceNumber"": """",
                          ""transactionId"": """",
                          ""isPersonal"": true,
                          ""source"": ""Unknown""
                        }
                    ")
                });
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains("/users/") && x.Method == HttpMethod.Get
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                          ""id"": ""6d16a4cb-4733-44de-b23b-0eb9e8ae6590"",
                          ""firstName"": ""Kari"",
                          ""lastName"": ""Normann"",
                          ""email"": ""kari@normann.no"",
                          ""mobileNumber"": ""+4790603360"",
                          ""employeeId"": ""EID:909091"",
                          ""userPreference"": {
                            ""language"": ""no""
                          },
                          ""organizationName"": ""ORGANIZATION ONE"",
                          ""userStatusName"": ""Deactivated"",
                          ""userStatus"": 1,
                          ""assignedToDepartment"": ""00000000-0000-0000-0000-000000000000"",
                          ""departmentName"": null,
                          ""role"": ""EndUser"",
                          ""managerOf"": []
                        }
                    ")
                });


            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            userOptionsMock.Setup(o => o.Value).Returns(new UserConfiguration() { ApiPath = @"/organizations" });

            var userId = Guid.Parse("6d16a4cb-4733-44de-b23b-0eb9e8ae6590");

            var userService = new Mock<IUserServices>();
            var user = new OrigoUser
            {
                Id = userId,
                Email = "kari@normann.no",
                FirstName = "Kari",
                UserPreference = new UserPreference() { Language = "en"},
                
            };
            userService.Setup(o => o.GetUserAsync(userId))
                .ReturnsAsync(user);
  


            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService.Object, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            var postData = new MakeAssetAvailable()
            {
                AssetLifeCycleId = Guid.Parse("80665d26-90b4-4a3a-a20d-686b64466f32")

            };

            // Act
            
            var asset = await assetService.MakeAssetAvailableAsync(new Guid(CUSTOMER_ID), postData, Guid.Empty);

            // Assert
            Assert.Equal(CUSTOMER_ID, asset.OrganizationId.ToString().ToLower());
            Assert.True(asset.AssetHolderId == null || asset.AssetHolderId == Guid.Empty);
            Assert.True(asset.Labels == null || !asset.Labels.Any());
            Assert.True(asset.AssetStatus == AssetLifecycleStatus.Available);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetLifeCycleSettingByCustomer()
        {
            // Arrange
            const string CUSTOMER_ID = "cab4bb77-3471-4ab3-ae5e-2d4fce450f36";

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
                            ""id"": ""978ffca7-73c0-4494-ad4b-6c092733f634"",
                            ""customerId"": ""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",
                            ""buyoutAllowed"": true,
                            ""createdDate"": ""2022-04-29T14:46:42.421138"",
                            ""assetCategoryId"": 1,
                            ""assetCategoryName"": ""Mobile phone"",
                            ""minBuyoutPrice"": 700,
                            ""runtime"": 12
                        }]
                    ")
                });


            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            // Act
            var assetings = await assetService.GetLifeCycleSettingByCustomer(new Guid(CUSTOMER_ID), CurrencyCode.NOK.ToString());

            // Assert
            Assert.Equal(CUSTOMER_ID, assetings.FirstOrDefault()!.CustomerId.ToString().ToLower());
            Assert.Equal(1, assetings.Count);
            Assert.Equal(CurrencyCode.NOK.ToString(), assetings.FirstOrDefault()!.Currency);
            Assert.Equal(12, assetings.FirstOrDefault()!.Runtime);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task SetLifeCycleSetting_CustomerExist()
        {
            // Arrange
            const string CUSTOMER_ID = "cab4bb77-3471-4ab3-ae5e-2d4fce450f36";

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x=>
                    x.RequestUri != null && x.RequestUri.ToString().Contains("/lifecycle-setting") && x.Method == HttpMethod.Get
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        [{
                            ""id"": ""978ffca7-73c0-4494-ad4b-6c092733f634"",
                            ""customerId"": ""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",
                            ""buyoutAllowed"": true,
                            ""createdDate"": ""2022-04-29T14:46:42.421138"",
                            ""assetCategoryId"": 1,
                            ""assetCategoryName"": ""Mobile phone"",
                            ""minBuyoutPrice"": 700
                        }]
                    ")
                });

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x=>
                    x.RequestUri != null && x.RequestUri.ToString().Contains("/lifecycle-setting") && x.Method == HttpMethod.Put
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""978ffca7-73c0-4494-ad4b-6c092733f634"",
                            ""customerId"": ""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",
                            ""buyoutAllowed"": false,
                            ""createdDate"": ""2022-04-29T14:46:42.421138"",
                            ""assetCategoryId"": 1,
                            ""assetCategoryName"": ""Mobile phone"",
                            ""minBuyoutPrice"": 700                        }
                    ")
                });


            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            var newSettings = new NewLifeCycleSetting()
            {
                AssetCategoryId = 1,
                BuyoutAllowed = false
            };

            // Act
            var lifeCycleSetting = await assetService.SetLifeCycleSettingForCustomerAsync(new Guid(CUSTOMER_ID), newSettings, CurrencyCode.NOK.ToString(), Guid.Empty);

            // Assert
            Assert.Equal(CUSTOMER_ID, lifeCycleSetting.CustomerId.ToString().ToLower());
            Assert.Equal(CurrencyCode.NOK.ToString(), lifeCycleSetting.Currency);
            Assert.True(!lifeCycleSetting.BuyoutAllowed);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task SetLifeCycleSetting_CustomerDoesNotExist()
        {
            // Arrange
            const string CUSTOMER_ID = "cab4bb77-3471-4ab3-ae5e-2d4fce450f36";

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains("/lifecycle-setting") && x.Method == HttpMethod.Get
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""00000000-0000-0000-0000-000000000000"",
                            ""customerId"": ""00000000-0000-0000-0000-000000000000"",
                            ""buyoutAllowed"": true,
                            ""createdDate"": ""2022-04-29T14:46:42.421138"",
                            ""assetCategoryId"": 1,
                            ""assetCategoryName"": ""Mobile phone"",
                            ""minBuyoutPrice"": 700
                        }
                    ")
                });

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains("/lifecycle-setting") && x.Method == HttpMethod.Post
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""978ffca7-73c0-4494-ad4b-6c092733f634"",
                            ""customerId"": ""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",
                            ""buyoutAllowed"": false,
                            ""createdDate"": ""2022-04-29T14:46:42.421138"",
                            ""assetCategoryId"": 1,
                            ""assetCategoryName"": ""Mobile phone"",
                            ""minBuyoutPrice"": 700
                        }
                    ")
                });


            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            var newSettings = new NewLifeCycleSetting()
            {
                BuyoutAllowed = false
            };

            // Act
            var assetings = await assetService.SetLifeCycleSettingForCustomerAsync(new Guid(CUSTOMER_ID), newSettings, CurrencyCode.NOK.ToString(), Guid.Empty);

            // Assert
            Assert.Equal(CUSTOMER_ID, assetings.CustomerId.ToString().ToLower());
            Assert.Equal(CurrencyCode.NOK.ToString(), assetings.Currency);
            Assert.True(!assetings.BuyoutAllowed);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task UpdateLabelsForCustomer()
        {
            // Arrange
            const string CUSTOMER_ID = "20ef7dbd-a0d1-44c3-b855-19799cceb347";
            const string LABEL_ONE_ID = "94D22A10-FC47-4F23-A524-B30D022DD8AF";
            const string LABEL_TWO_ID = "53D6241B-7297-44E4-857B-9EEA69BEB174";

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
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            IList<Label> labels = new List<Label>();
            labels.Add(new Label { Id = new Guid(LABEL_ONE_ID), Text = "Administrator", Color = LabelColor.Blue });
            labels.Add(new Label { Id = new Guid(LABEL_TWO_ID), Text = "Assistant", Color = LabelColor.Lightblue });

            var data = new UpdateCustomerLabelsData
            {
                Labels = labels,
                CallerId = Guid.Empty
            };

            // Act
            var labelsResult = await assetService.UpdateLabelsForCustomerAsync(new Guid(CUSTOMER_ID), data);

            // Assert
            Assert.Equal(3, labelsResult.Count);
            Assert.Equal("Administrator", labelsResult[0].Text);
            Assert.Equal(2, (int)labelsResult[1].Color);
            Assert.Equal("Green", labelsResult[2].ColorName);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetDisposeSettingByCustomer()
        {
            // Arrange
            const string CUSTOMER_ID = "cab4bb77-3471-4ab3-ae5e-2d4fce450f36";

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
                            ""id"": ""978ffca7-73c0-4494-ad4b-6c092733f634"",
                            ""customerId"": ""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",
                            ""payrollContactEmail"": ""example@techstep.no"",
                            ""createdDate"": ""2022-04-29T14:46:42.421138""
                        }
                    ")
                });


            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            // Act
            var assetings = await assetService.GetDisposeSettingByCustomer(new Guid(CUSTOMER_ID));

            // Assert
            Assert.True(!string.IsNullOrEmpty(assetings.PayrollContactEmail));
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetDisposeSettingByCustomer_NotExist()
        {
            // Arrange
            const string CUSTOMER_ID = "cab4bb77-3471-4ab3-ae5e-2d4fce450f36";

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"null")
                });


            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            // Act
            var assetings = await assetService.GetDisposeSettingByCustomer(new Guid(CUSTOMER_ID));

            // Assert
            Assert.True(string.IsNullOrEmpty(assetings.PayrollContactEmail));
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task SetDisposeSetting_CustomerExist()
        {
            // Arrange
            const string CUSTOMER_ID = "cab4bb77-3471-4ab3-ae5e-2d4fce450f36";

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains("/dispose-setting") && x.Method == HttpMethod.Get
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""978ffca7-73c0-4494-ad4b-6c092733f634"",
                            ""customerId"": ""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",
                            ""payrollContactEmail"": ""example@techstep.no"",
                            ""createdDate"": ""2022-04-29T14:46:42.421138""
                        }
                    ")
                });

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains("/dispose-setting") && x.Method == HttpMethod.Put
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""978ffca7-73c0-4494-ad4b-6c092733f634"",
                            ""customerId"": ""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",
                            ""payrollContactEmail"": ""example2@techstep.no"",
                            ""createdDate"": ""2022-04-29T14:46:42.421138""                   
                        }")
                });


            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            var newSettings = new NewDisposeSetting()
            {
                PayrollContactEmail = "example2@techstep.no"
            };

            // Act
            var disposeSetting = await assetService.SetDisposeSettingForCustomerAsync(new Guid(CUSTOMER_ID), newSettings, Guid.Empty);

            // Assert
            Assert.Equal(newSettings.PayrollContactEmail, disposeSetting.PayrollContactEmail);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task SetDisposeSetting_CustomerDoesNotExist()
        {
            // Arrange
            const string CUSTOMER_ID = "cab4bb77-3471-4ab3-ae5e-2d4fce450f36";

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains("/dispose-setting") && x.Method == HttpMethod.Get
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty)
                });

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains("/dispose-setting") && x.Method == HttpMethod.Post
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"
                        {
                            ""id"": ""978ffca7-73c0-4494-ad4b-6c092733f634"",
                            ""customerId"": ""cab4bb77-3471-4ab3-ae5e-2d4fce450f36"",
                            ""payrollContactEmail"": ""example2@techstep.no"",
                            ""createdDate"": ""2022-04-29T14:46:42.421138""   
                        }
                    ")
                });


            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            var newSettings = new NewDisposeSetting()
            {
                PayrollContactEmail = "example2@techstep.no"
            };

            // Act
            var assetings = await assetService.SetDisposeSettingForCustomerAsync(new Guid(CUSTOMER_ID), newSettings, Guid.Empty);

            // Assert
            Assert.Equal(newSettings.PayrollContactEmail, assetings.PayrollContactEmail);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task ReturnDeviceAsync_AssetDoesNotExist()
        {
            // Arrange
            const string CUSTOMER_ID = "cab4bb77-3471-4ab3-ae5e-2d4fce450f36";
            const string ASSET_ID = "c0c7cdba-3217-4da3-9537-cb34b6b8b765";

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains($"{ASSET_ID}/customers") && x.Method == HttpMethod.Get
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            // Act and assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                assetService.ReturnDeviceAsync(new Guid(CUSTOMER_ID), new Guid(ASSET_ID), PredefinedRole.EndUser.ToString(), new List<Guid?>(), Guid.Empty, Guid.Empty));
        }
         
        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task ReportDeviceAsync_AssetDoesNotExist()
        {
            // Arrange
            const string CUSTOMER_ID = "cab4bb77-3471-4ab3-ae5e-2d4fce450f36";
            const string ASSET_ID = "c0c7cdba-3217-4da3-9537-cb34b6b8b765";

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains($"{ASSET_ID}/customers") && x.Method == HttpMethod.Get
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            var reportData = new ReportDevice()
            {
                AssetId = new Guid(ASSET_ID),
                ReportCategory = ReportCategory.Lost,
                Description = "test",
                TimePeriodFrom = DateTime.UtcNow.AddDays(-2),
                TimePeriodTo = DateTime.UtcNow,
                Country = "NO",
                Address = "test",
                PostalCode = "1245",
                City = "Test"
            };

            // Act and assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                assetService.ReportDeviceAsync(new Guid(CUSTOMER_ID), reportData, PredefinedRole.EndUser.ToString(), new List<Guid?>(), Guid.Empty));
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task BuyoutDeviceAsync_AssetDoesNotExist()
        {
            // Arrange
            const string CUSTOMER_ID = "cab4bb77-3471-4ab3-ae5e-2d4fce450f36";
            const string ASSET_ID = "c0c7cdba-3217-4da3-9537-cb34b6b8b765";

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains($"{ASSET_ID}/customers") && x.Method == HttpMethod.Get
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new AssetConfiguration() { ApiPath = @"/assets" };
            var optionsMock = new Mock<IOptions<AssetConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var userOptionsMock = new Mock<IOptions<UserConfiguration>>();
            var userService = new UserServices(Mock.Of<ILogger<UserServices>>(), httpClient, userOptionsMock.Object, _mapper);
            var departmentOptionsMock = new Mock<IOptions<DepartmentConfiguration>>();
            var departmentService = new DepartmentsServices(Mock.Of<ILogger<DepartmentsServices>>(), httpClient, departmentOptionsMock.Object, _mapper);


            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), httpClient, optionsMock.Object, userService, new Mock<IUserPermissionService>().Object, _mapper, departmentService);

            // Act and assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                assetService.BuyoutDeviceAsync(new Guid(CUSTOMER_ID), new Guid(ASSET_ID), PredefinedRole.EndUser.ToString(), new List<Guid?>(), Guid.Empty));
        }

    }
}