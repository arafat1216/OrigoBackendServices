using AutoMapper;
using Common.Configuration;
using Common.Utilities;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Mappings;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using HardwareServiceOrderServices.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace HardwareServiceOrder.IntegrationTests
{
    public class HardwareServiceOrderWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
    {
        private readonly DbConnection _dbConnection = new SqliteConnection("Data Source=:memory:");
        public readonly Guid CUSTOMER_ONE_ID = Guid.Parse("7adbd9fa-97d1-11ec-8500-00155d64bd3d");
        public readonly Guid CUSTOMER_TWO_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD63");
        public readonly Guid USER_ID = Guid.Parse("3286ba71-fdde-4496-94fa-36de7aa0b41e");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                ReplaceHardwareServiceOrderDbContext<HardwareServiceOrderContext>(services);
                var serviceProvider = services.BuildServiceProvider();
                var scope = serviceProvider.CreateScope();

                var hardwareServiceOrderContext = scope.ServiceProvider.GetRequiredService<HardwareServiceOrderContext>();
                hardwareServiceOrderContext.Database.EnsureCreated();

                HardwareServiceOrderServices.Models.HardwareServiceOrder hwServiceOrder = new(
                    Guid.NewGuid(),
                    CUSTOMER_ONE_ID,
                    Guid.NewGuid(),
                    1,
                    new(
                        "Brand",
                        "Model",
                        new HashSet<string>() { "IMEI" },
                        "S/N-12345",
                        DateOnly.Parse("2020-01-01"),
                        null
                    ),
                    "[UserDescription]",
                    new ContactDetails(
                        USER_ID,
                        "FirstName",
                        "LastName",
                        "Email",
                        "PhoneNumber"
                    ),
                    new DeliveryAddress(
                        RecipientTypeEnum.Personal,
                        "recipient",
                        "address1",
                        "address2",
                        "postal-code",
                        "city",
                        "NO"
                    ),
                    (int)ServiceTypeEnum.SUR,
                    11,
                    (int)ServiceProviderEnum.ConmodoNo,
                    "serviceProviderOrderId1",
                    "OrderID2",
                    "OrderExternalLink",
                    new List<ServiceEvent> { new ServiceEvent { ServiceStatusId = 3, Timestamp = DateTime.UtcNow } }
                );

                var dataProtector1 = scope.ServiceProvider.GetRequiredService<IDataProtectionProvider>().CreateProtector($"{CUSTOMER_ONE_ID}");
                var dataProtector2 = scope.ServiceProvider.GetRequiredService<IDataProtectionProvider>().CreateProtector($"{CUSTOMER_TWO_ID}");

                var cmServiceProvider1 = new CustomerServiceProvider
                {
                    CustomerId = CUSTOMER_ONE_ID,
                    ApiPassword = dataProtector1.Protect("52079706"),
                    ApiUserName = dataProtector1.Protect("d723hjdfhdfnsl23sdf"),
                    ServiceProviderId = (int)ServiceProviderEnum.ConmodoNo,
                    LastUpdateFetched = DateTime.Today,
                    ApiCredentials = new List<ApiCredential>(),
                    ActiveServiceOrderAddons = new List<ServiceOrderAddon>()
                };
                var cmServiceProvider2 = new CustomerServiceProvider
                {
                    CustomerId = CUSTOMER_TWO_ID,
                    ApiPassword = dataProtector2.Protect("52079706"),
                    ApiUserName = dataProtector2.Protect("d723hjdfhdfnsl23sdf"),
                    ServiceProviderId = (int)ServiceProviderEnum.ConmodoNo,
                    LastUpdateFetched = DateTime.Today.AddDays(-1),
                    ApiCredentials = new List<ApiCredential>(),
                    ActiveServiceOrderAddons = new List<ServiceOrderAddon>()
                };

                hardwareServiceOrderContext.Add(hwServiceOrder);
                hardwareServiceOrderContext.AddRange(cmServiceProvider1, cmServiceProvider2);
                hardwareServiceOrderContext.SaveChanges();

                ApiCredential apiCredential1 = new(cmServiceProvider1.Id, null, dataProtector1.Protect("Username"), dataProtector1.Protect("Password"));
                ApiCredential apiCredential2 = new(cmServiceProvider1.Id, (int)ServiceTypeEnum.SUR, dataProtector1.Protect("Username"), dataProtector1.Protect("Password"));
                ServiceOrderAddon? serviceOrderAddon1 = hardwareServiceOrderContext.ServiceOrderAddons.Find(1);

                cmServiceProvider1.ApiCredentials.Add(apiCredential1);
                cmServiceProvider1.ApiCredentials.Add(apiCredential2);
                cmServiceProvider1.ActiveServiceOrderAddons.Add(serviceOrderAddon1);
                hardwareServiceOrderContext.SaveChanges();


                #region Mock/setup for IRepairProvider
                // Conmodo
                var repairProviderMock = new Mock<IRepairProvider>();

                repairProviderMock.Setup(m => m.CreateRepairOrderAsync(It.IsAny<NewExternalRepairOrderDTO>(), It.IsAny<int>(), It.IsAny<string>()))
                    .ReturnsAsync(new NewExternalRepairOrderResponseDTO(serviceProviderOrderId1: "serviceProviderOrderId1", serviceProviderOrderId2: "serviceProviderOrderId2", externalServiceManagementLink: "externalServiceManagementLink"));

                repairProviderMock.Setup(m => m.GetUpdatedRepairOrdersAsync(It.IsAny<DateTimeOffset>()))
                    .ReturnsAsync(new List<ExternalRepairOrderDTO> {
                        new ExternalRepairOrderDTO
                        {
                            AssetIsReplaced = false,
                            ExternalServiceEvents = new List<ExternalServiceEventDTO>{ new ExternalServiceEventDTO {  ServiceStatusId = 2, Timestamp = DateTimeOffset.UtcNow} },
                            ProvidedAsset = new AssetInfoDTO(),
                            ReturnedAsset = new AssetInfoDTO(),
                            ServiceProviderOrderId1 = "serviceProviderOrderId1",
                            ServiceProviderOrderId2 = "serviceProviderOrderId2"
                        }
                    });
                #endregion

                #region Mock/setup for IProviderFactory
                var providerFactoryMock = new Mock<IProviderFactory>();

                providerFactoryMock.Setup(m => m.GetRepairProviderAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(repairProviderMock.Object);

                services.AddScoped<IProviderFactory>(s => providerFactoryMock.Object);
                #endregion

                #region Mock/setup for IEmailService
                var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));
                var emailOptions = Options.Create(new EmailConfiguration
                {

                });
                var origoOptions = Options.Create(new OrigoConfiguration
                {
                    BaseUrl = "https://origov2dev.mytos.no",
                    OrderPath = "/my-business/{0}/hardware-repair/{1}/view"
                });
                var flatDictionary = new FlatDictionary();

                var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new EmailProfile());
                }).CreateMapper();

                var mockFactory = new Mock<IHttpClientFactory>();
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                        x.RequestUri != null && x.RequestUri.ToString().Contains("/notification") && x.Method == HttpMethod.Post
                        ),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(string.Empty)
                    });
                var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
                mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

                var _emailService = new EmailService(emailOptions, flatDictionary, resourceManger, mapper, origoOptions, hardwareServiceOrderContext, mockFactory.Object);
                services.AddScoped<IEmailService>(s => _emailService);
                #endregion

                #region Mock/setup for the ServiceOrderStatusHandlerService implementation
                var statusHandlerFactoryMock = new Mock<IStatusHandlerFactory>();

                var serviceOrderStatusHandlerServiceMock = new Mock<ServiceOrderStatusHandlerService>();

                statusHandlerFactoryMock.Setup(m => m.GetStatusHandler(It.IsAny<ServiceTypeEnum>()))
                    .Returns(serviceOrderStatusHandlerServiceMock.Object);

                serviceOrderStatusHandlerServiceMock
                    .Setup(m =>
                        m.HandleServiceOrderStatusAsync(It.IsAny<HardwareServiceOrderServices.Models.HardwareServiceOrder>(), It.IsAny<ExternalRepairOrderDTO>()));

                services.AddScoped(s => statusHandlerFactoryMock.Object);
                #endregion

            });
            base.ConfigureWebHost(builder);
        }

        private void ReplaceHardwareServiceOrderDbContext<T>(IServiceCollection services) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            _dbConnection.Open();
            services.AddDbContext<T>(options =>
            {
                options.UseSqlite(_dbConnection);
                options.EnableSensitiveDataLogging();
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _dbConnection.Dispose();
        }
    }
}
