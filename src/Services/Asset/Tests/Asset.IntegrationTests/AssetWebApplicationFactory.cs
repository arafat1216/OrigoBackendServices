using System;
using System.Data.Common;
using System.Linq;
using AssetServices.Infrastructure;
using Common.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable StringLiteralTypo
// ReSharper disable once ClassNeverInstantiated.Global

namespace Asset.IntegrationTests;

public class AssetWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class

{
    private readonly DbConnection _dbConnection = new SqliteConnection("Data Source=:memory:");

    public readonly Guid ORGANIZATION_ID = Guid.Parse("7adbd9fa-97d1-11ec-8500-00155d64bd3d");


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ReplaceDbContext<AssetsContext>(services);
            ReplaceDbContext<LoggingDbContext>(services);

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            using var assetsContext = scope.ServiceProvider.GetRequiredService<AssetsContext>();
            assetsContext.Database.EnsureCreated();
            try
            {
                //assetsContext.Assets.Add(new AssetServices.Models.MobilePhone{});
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