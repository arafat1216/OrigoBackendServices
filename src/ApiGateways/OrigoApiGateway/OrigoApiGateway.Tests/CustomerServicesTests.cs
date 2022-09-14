
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
    public class CustomerServicesTests
    {
        private static IMapper? _mapper;

        public CustomerServicesTests()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddMaps(Assembly.GetAssembly(typeof(TechstepCoreProfile)));
                });
                _mapper = mappingConfig.CreateMapper();
            }
        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetTechstepCustomers_NotIncludeInactiveCustomers()
        {
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
                        {""data"": [{
                            ""techstepCustomerId"": ""1111111111111"",
                            ""orgNumber"": ""12546782"",
                            ""name"": ""Organization 1"",
                            ""isInactive"": false,
                            ""accountOwner"": ""Heidi Dahl"",
                            ""chainCode"": null,
                            ""countryCode"": ""NO"",
                            ""mainCountryCode"": ""NO"",
                            ""customerClassId"": ""1"",
                            ""createdDate"": ""2022-02-09T08:18:54.0194532"",
                            ""updatedDate"": ""2022-02-09T08:31:42.7933333"",
                            ""isBlocked"": false,
                            ""chainCount"": ""2""
                        },
                            {""techstepCustomerId"": ""22222222222"",
                            ""orgNumber"": ""12546782"",
                            ""name"": ""Organization 2"",
                            ""isInactive"": true,
                            ""accountOwner"": null,
                            ""chainCode"": null,
                            ""countryCode"": ""NO"",
                            ""mainCountryCode"": ""NO"",
                            ""customerClassId"": ""1"",
                            ""createdDate"": ""2022-02-09T08:18:54.0194532"",
                            ""updatedDate"": ""2022-02-09T08:31:42.7933333"",
                            ""isBlocked"": false,
                            ""chainCount"": ""2""}]}
                    ")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var customerService = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), mockFactory.Object, Mock.Of<IOptions<CustomerConfiguration>>(),_mapper);

            var searchString = "Organization";
            var techstepCoreData = await customerService.GetTechstepCustomers(searchString);

            Assert.NotNull(techstepCoreData);
            Assert.NotEmpty(techstepCoreData.Data);
            Assert.Equal(1,techstepCoreData.Data?.Count);
            Assert.All(techstepCoreData?.Data, u => Assert.Equal("Organization 1", u.Name));
        }
    }
}
