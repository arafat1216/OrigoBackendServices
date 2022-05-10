using Common.Logging;
using Customer.API.IntegrationTests.Helpers;
using CustomerServices.Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Customer.API.Tests;

    public class CustomerWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        //private readonly DbConnection _dbConnection = new SqliteConnection("Data Source=:memory:");
       
        private readonly InMemoryDatabaseRoot _dbRoot = new InMemoryDatabaseRoot();
        public Guid ORGANIZATION_ID => CustomerTestDataSeedingForDatabase.ORGANIZATION_ID;
        public Guid HEAD_DEPARTMENT_ID => CustomerTestDataSeedingForDatabase.HEAD_DEPARTMENT_ID;
        public Guid SUB_DEPARTMENT_ID => CustomerTestDataSeedingForDatabase.SUB_DEPARTMENT_ID;
        public Guid USER_ONE_ID => CustomerTestDataSeedingForDatabase.USER_ONE_ID;
        public Guid USER_TWO_ID => CustomerTestDataSeedingForDatabase.USER_TWO_ID;
        public Guid USER_THREE_ID => CustomerTestDataSeedingForDatabase.USER_THREE_ID;


    protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {

                ReplaceDbContext<CustomerContext>(services);
                ReplaceDbContext<LoggingDbContext>(services);

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                using var customerContext = scope.ServiceProvider.GetRequiredService<CustomerContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomerWebApplicationFactory<TProgram>>>();

                try
                {
                    CustomerTestDataSeedingForDatabase.PopulateData(customerContext);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception,
                        "An error occurred seeding the " + "database with test data. Error: {Message}", exception.Message);
                }
            });
            base.ConfigureWebHost(builder);
        }
        private void ReplaceDbContext<T>(IServiceCollection services) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<T>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting", _dbRoot);
                options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                options.EnableSensitiveDataLogging();
            });
        }
    }

