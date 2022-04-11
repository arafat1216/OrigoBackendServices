using Common.Logging;
using CustomerServices.Infrastructure.Context;
using CustomerServices.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.API.Tests
{
    public class CustomerWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly DbConnection _dbConnection = new SqliteConnection("Data Source=:memory:");
        private ServiceProvider? _serviceProvider;

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                ReplaceDbContext<CustomerContext>(services);
                ReplaceDbContext<LoggingDbContext>(services);

                _serviceProvider = services.BuildServiceProvider();
                using var scope = _serviceProvider?.CreateScope();
                using var customerContext = scope?.ServiceProvider.GetRequiredService<CustomerContext>();
                customerContext?.Database.EnsureDeletedAsync();
                customerContext?.Database.EnsureCreatedAsync();

                try
                {
                    customerContext?.PopulateData();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
            var host = builder.Build();
            Task.Run(() => host.StartAsync()).GetAwaiter().GetResult();
            return host;
        }

        private void ReplaceDbContext<T>(IServiceCollection services) where T : DbContext
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

    public static class Extention
    {
        public static readonly Guid ORGANIZATION_ID = Guid.Parse("f5635deb-9b38-411c-9577-5423c9290106");
        public static readonly Guid HEAD_DEPARTMENT_ID = Guid.Parse("37d6d1b1-54a5-465d-a313-b6c250d66db4");
        public static readonly Guid SUB_DEPARTMENT_ID = Guid.Parse("5355134f-4852-4c36-99d1-fa9d4a1d7a61");
        public static readonly Guid CALLER_ID = Guid.Parse("a05f97fc-2e3d-4be3-a64c-e2f30ed90b93");
        public static readonly Guid PARENT_ID = Guid.Parse("fa82e042-f4bc-4de1-b68d-dfcb95a64c65");
        public static readonly Guid LOCATION_ID = Guid.Parse("089f6c40-1ae4-4fd0-b2d1-c5181d9fbfde");

        private static object _customerContextLock = new object();
        public static CustomerContext? PopulateData(this CustomerContext customerContext)
        {

            lock (_customerContextLock)
            {
                if (!customerContext.Organizations.Any())
                {
                    var address = new Address("Billingstadsletta 19B", "1396", "Oslo", "NO");
                    var contactPerson = new ContactPerson("Ola", "Normann", "ola@normann.no", "+4745454649");
                    var organizationPreferences = new OrganizationPreferences(ORGANIZATION_ID,
                                                                              CALLER_ID,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                true,
                                                                                "NO",
                                                                                1);
                    customerContext?.OrganizationPreferences.Add(organizationPreferences);

                    var location = new Location(LOCATION_ID,
                                                CALLER_ID,
                                                "Location1",
                                                "Description",
                                                "Billingstadsletta",
                                                "19 B",
                                                "1396",
                                                "Oslo",
                                                "NO");

                    customerContext?.Locations.Add(location);

                    var organization = new Organization(ORGANIZATION_ID,
                                                        CALLER_ID,
                                                        PARENT_ID,
                                                        "ORGANIZATION ONE",
                                                        "ORGNUM12345",
                                                        address,
                                                        contactPerson,
                                                        organizationPreferences,
                                                        location,
                                                        null,
                                                        true
                                                        );
                    customerContext?.Organizations.Add(organization);

                    var headDepartment = new Department("Head department",
                                                    "costCenterId",
                                                    "Description",
                                                    organization,
                                                    HEAD_DEPARTMENT_ID,
                                                    CALLER_ID);

                    customerContext?.Departments.Add(headDepartment);
                    var subDepartment = new Department("Sub department",
                                                    "costCenterId",
                                                    "Description",
                                                    organization,
                                                    SUB_DEPARTMENT_ID,
                                                    CALLER_ID,
                                                    headDepartment);

                    customerContext?.Departments.Add(subDepartment);
                    customerContext?.SaveChanges();
                }
                return customerContext;
            }
        }

        
    }
}
