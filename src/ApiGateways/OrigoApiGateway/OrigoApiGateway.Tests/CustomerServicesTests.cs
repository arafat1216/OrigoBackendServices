using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Moq.Protected;
using OrigoApiGateway.Mappings;
using OrigoApiGateway.Services;

namespace OrigoApiGateway.Tests;

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
        // Arrange
        var mockFactory = new Mock<IHttpClientFactory>();
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"
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
        mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
            ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri != null && x.RequestUri.ToString().Contains("/?partnerId=") && x.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(@"
                        []
                    ")
        });

        using var customerHttpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
        using var techstepCoreNoHttpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
        using var techstepCoreSeHttpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };

        mockFactory.Setup(_ => _.CreateClient("customerservices")).Returns(customerHttpClient);
        mockFactory.Setup(_ => _.CreateClient("techstep-core-customers-no")).Returns(techstepCoreNoHttpClient);
        mockFactory.Setup(_ => _.CreateClient("techstep-core-customers-se")).Returns(techstepCoreSeHttpClient);

        var options =
            new CustomerConfiguration { TechstepPartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C") };
        var optionsMock = new Mock<IOptions<CustomerConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(options);
        var customerService = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), mockFactory.Object,
            optionsMock.Object, _mapper);
        const string SEARCH_STRING = "Organization";

        // Act
        var techstepCoreData = await customerService.GetTechstepCustomersAsync(SEARCH_STRING, "NO");

        // Assert
        Assert.NotNull(techstepCoreData);
        Assert.NotEmpty(techstepCoreData.Data);
        Assert.Equal(1, techstepCoreData.Data?.Count);
        Assert.All(techstepCoreData.Data!, u => Assert.Equal("Organization 1", u.Name));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetTechstepCustomers_DoNotShowCustomersAlreadyAddedInList()
    {
        // Arrange
        var mockFactory = new Mock<IHttpClientFactory>();
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
            ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri != null && x.RequestUri.ToString().Contains("?searchString=") &&
                x.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(@"
                        {""data"": [{
                            ""techstepCustomerId"": ""1111111111111"",
                            ""orgNumber"": ""000000001"",
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
                            ""orgNumber"": ""000000002"",
                            ""name"": ""Organization 2"",
                            ""isInactive"": false,
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
        mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
            ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri != null && x.RequestUri.ToString().Contains("/?partnerId=") && x.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(@"
                        [{
                            ""organizationId"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                            ""organizationNumber"": ""000000001"",
                            ""name"": ""Organization 1"",
                            ""lastDayForReportingSalaryDeduction"": 0,
                            ""payrollContactEmail"": ""user@example.com"",
                            ""address"": null,
                            ""contactPerson"": null,
                            ""preferences"": null,
                            ""location"": null,
                            ""partnerId"": ""5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C"",
                            ""addUsersToOkta"": false,
                            ""status"": 1,
                            ""statusName"": ""Onboarded"",
                            ""accountOwner"": ""Heidi Dahl"",
                            ""techstepCustomerId"": 121212122
                        }]
                    ")
        });
        using var customerHttpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
        using var techstepCoreNoHttpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
        using var techstepCoreSeHttpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
        mockFactory.Setup(_ => _.CreateClient("customerservices")).Returns(customerHttpClient);
        mockFactory.Setup(_ => _.CreateClient("techstep-core-customers-no")).Returns(techstepCoreNoHttpClient);
        mockFactory.Setup(_ => _.CreateClient("techstep-core-customers-se")).Returns(techstepCoreSeHttpClient);
        var options =
            new CustomerConfiguration { TechstepPartnerId = Guid.Parse("5741B4A1-4EEF-4FC2-B1B8-0BA7F41ED93C") };
        var optionsMock = new Mock<IOptions<CustomerConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(options);
        var customerService = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), mockFactory.Object,
            optionsMock.Object, _mapper);
        const string SEARCH_STRING = "Organization";

        // Act
        var techstepCoreData = await customerService.GetTechstepCustomersAsync(SEARCH_STRING, "NO");

        // Assert
        Assert.NotNull(techstepCoreData);
        Assert.NotEmpty(techstepCoreData.Data);
        Assert.Equal(1, techstepCoreData.Data!.Count);
        Assert.All(techstepCoreData.Data, u => Assert.Equal("Organization 2", u.Name));
    }

    [Fact]
    [Trait("Category", "UnitTest")]

    public async Task GetTechstepCustomers_WithMissingCountryCode_ReturnsEmpty()
    {
        // Arrange
        var mockFactory = new Mock<IHttpClientFactory>();
        var optionsMock = new Mock<IOptions<CustomerConfiguration>>();
        var customerService = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), mockFactory.Object,
            optionsMock.Object, _mapper);

        // Act
        var techstepCustomersFound = await customerService.GetTechstepCustomersAsync("ABC", "");

        // Assert
        Assert.Null(techstepCustomersFound.Data);
    }
}