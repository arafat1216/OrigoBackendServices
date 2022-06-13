using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                ReplaceHardwareServiceOrderDbContext<HardwareServiceOrderContext>(services);
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                using var hardwareServiceOrderContext = scope.ServiceProvider.GetRequiredService<HardwareServiceOrderContext>();
                hardwareServiceOrderContext.Database.EnsureCreated();

                //var serviceType = hardwareServiceOrderContext.ServiceTypes.FirstOrDefault(m => m.Id == 3);
                var hwServiceOrder = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                    Guid.NewGuid(),
                    CUSTOMER_ONE_ID,
                    Guid.NewGuid(),
                    "[UserDescription]",
                    new HardwareServiceOrderServices.Models.ContactDetails(Guid.NewGuid(), "FirstName", "Email"),
                    new HardwareServiceOrderServices.Models.DeliveryAddress(HardwareServiceOrderServices.Models.RecipientTypeEnum.Personal, "recipient", "address1", "address2", "postal-code", "NO", "NO"),
                    3, 3, 1, "", "", "", new List<ServiceEvent> { });

               // hwServiceOrder.AddServiceEvent(new ServiceEvent { ServiceStatusId = 3, Timestamp = DateTime.Now });

                hardwareServiceOrderContext.Add(hwServiceOrder);
                hardwareServiceOrderContext.SaveChanges();
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
