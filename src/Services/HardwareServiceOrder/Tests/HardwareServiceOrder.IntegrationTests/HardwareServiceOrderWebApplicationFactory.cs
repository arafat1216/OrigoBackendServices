using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using HardwareServiceOrderServices.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

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
                using var scope = serviceProvider.CreateScope();

                using var hardwareServiceOrderContext = scope.ServiceProvider.GetRequiredService<HardwareServiceOrderContext>();
                hardwareServiceOrderContext.Database.EnsureCreated();

                var hwServiceOrder = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
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
                    3,
                    3,
                    1,
                    "serviceProviderOrderId1",
                    "OrderID2",
                    "OrderExternalLink",
                    new List<ServiceEvent> { new ServiceEvent { ServiceStatusId = 3, Timestamp = DateTime.UtcNow } }
                );

                var cmServiceProvider1 = new CustomerServiceProvider
                {
                    CustomerId = CUSTOMER_ONE_ID,
                    ApiPassword = "52079706",
                    ApiUserName = "d723hjdfhdfnsl23sdf",
                    ServiceProviderId = 1,
                    LastUpdateFetched = DateTime.Today,
                };
                var cmServiceProvider2 = new CustomerServiceProvider
                {
                    CustomerId = CUSTOMER_TWO_ID,
                    ApiPassword = "52079706",
                    ApiUserName = "d723hjdfhdfnsl23sdf",
                    ServiceProviderId = 1,
                    LastUpdateFetched = DateTime.Today.AddDays(-1),
                };

                hardwareServiceOrderContext.Add(hwServiceOrder);
                hardwareServiceOrderContext.AddRange(cmServiceProvider1, cmServiceProvider2);
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
                var emailServiceMock = new Mock<IEmailService>();
                emailServiceMock.Setup(m => m.SendAssetRepairEmailAsync(It.IsAny<DateTime>(), It.IsAny<int>(), "en"));

                services.AddScoped(s => emailServiceMock);
                #endregion

                #region Mock/setup for the ServiceOrderStatusHandlerService implementation
                var statusHandlMock = new Dictionary<ServiceStatusEnum, ServiceOrderStatusHandlerService>();

                var serviceOrderStatusHandlerServiceMock = new Mock<ServiceOrderStatusHandlerService>();
                serviceOrderStatusHandlerServiceMock.Setup(m => m.UpdateServiceOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<ServiceStatusEnum>(), It.IsAny<ISet<string>>(), It.IsAny<string>()));

                statusHandlMock.Add(ServiceStatusEnum.Unknown, serviceOrderStatusHandlerServiceMock.Object);

                services.AddScoped(s => statusHandlMock);
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
