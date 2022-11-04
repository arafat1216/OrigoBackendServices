using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq.Protected;
using OrigoApiGateway.Mappings.SubscriptionManagement;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrigoApiGateway.Tests
{
    public class SubscriptionManagementServiceTests
    {
        private static IMapper _mapper;
        private Guid EMPTY_CALLER_ID = Guid.Empty;

        public SubscriptionManagementServiceTests()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddMaps(Assembly.GetAssembly(typeof(SubscriptionOrdersProfile)));
                });
                _mapper = mappingConfig.CreateMapper();
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void TransferToPrivateSubscriptionOrderForCustomerAsync_ReturnsOrder()
        {
            // Arrange
            Guid CUSTOMER_ID = Guid.Parse("20ef7dbd-a0d1-44c3-b855-19799cceb347");
            string MOBILE_NUMBER = "98989898";

            var transferPrivateOrder = new TransferToPrivateSubscriptionOrder();

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri != null && x.RequestUri.ToString().Contains("/transfer-to-private") &&
                x.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Created,
                    Content = new StringContent(
                        @"
                        {
                            ""MobileNumber"": ""99999999"",
                            ""OperatorId"": 1,
                            ""OperatorName"": ""Telia"",
                            ""NewSubscriptionName"": ""Private +"",
                            ""OrderExecutionDate"": ""2022-02-09T08:31:42.7933333"",
                             ""PrivateSubscription"":
                            {
                                ""FirstName"":"""",
                                ""LastName"":"""",
                                ""Address"":"""",
                                ""PostalCode"":"""",
                                ""PostalPlace"":"""",
                                ""Country"":""""
                            }
                        }
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var options = new SubscriptionManagementConfiguration() { ApiPath = @"/subscriptionmanagementservices" };
            var optionsMock = new Mock<IOptions<SubscriptionManagementConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(options);

            var subscriptionManagement = new SubscriptionManagementService(Mock.Of<ILogger<SubscriptionManagementService>>(), optionsMock.Object, Mock.Of<IUserServices>(), mockFactory.Object, _mapper);

            // Act
            var order = await subscriptionManagement.TransferToPrivateSubscriptionOrderForCustomerAsync(CUSTOMER_ID, transferPrivateOrder, EMPTY_CALLER_ID);

            // Assert
            Assert.Equal("Private +", order.NewSubscriptionName);
        }
    }
}
