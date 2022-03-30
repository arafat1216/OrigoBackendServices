using Common.Logging;
using CustomerServices.Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;
using System.Linq;

namespace Customer.API.Tests
{
    public class CustomerWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly DbConnection _dbConnection = new SqliteConnection("Data Source=:memory:");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                ReplaceDbContext<CustomerContext>(services);
                ReplaceDbContext<LoggingDbContext>(services);

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                using var assetsContext = scope.ServiceProvider.GetRequiredService<CustomerContext>();
                assetsContext.Database.EnsureCreated();
                try
                {
                    assetsContext.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
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