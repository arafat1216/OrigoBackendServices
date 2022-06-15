using System;
using System.Linq;
using Asset.IntegrationTests.Helpers;
using AssetServices.Infrastructure;
using Common.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

// ReSharper disable StringLiteralTypo
// ReSharper disable once ClassNeverInstantiated.Global

namespace Asset.IntegrationTests;

public class AssetWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class

{
    //private readonly DbConnection _dbConnection = new SqliteConnection("Data Source=:memory:");
    private readonly InMemoryDatabaseRoot _dbRoot = new InMemoryDatabaseRoot();


    public FeatureManagerStub FeatureManager { get; private set; }

    public Guid ORGANIZATION_ID => AssetTestDataSeedingForDatabase.ORGANIZATION_ID;
    public Guid ASSETLIFECYCLE_ONE_ID => AssetTestDataSeedingForDatabase.ASSETLIFECYCLE_ONE_ID;
    public Guid ASSETLIFECYCLE_TWO_ID => AssetTestDataSeedingForDatabase.ASSETLIFECYCLE_TWO_ID;
    public Guid ASSETLIFECYCLE_THREE_ID => AssetTestDataSeedingForDatabase.ASSETLIFECYCLE_THREE_ID;
    public Guid ASSETLIFECYCLE_FOUR_ID => AssetTestDataSeedingForDatabase.ASSETLIFECYCLE_FOUR_ID;
    public Guid ASSETLIFECYCLE_FIVE_ID => AssetTestDataSeedingForDatabase.ASSETLIFECYCLE_FIVE_ID;
    public Guid ASSETLIFECYCLE_SEVEN_ID => AssetTestDataSeedingForDatabase.ASSETLIFECYCLE_SEVEN_ID;
    public Guid ASSETLIFECYCLE_EIGHT_ID => AssetTestDataSeedingForDatabase.ASSETLIFECYCLE_EIGHT_ID;
    public Guid ASSETLIFECYCLE_NINE_ID => AssetTestDataSeedingForDatabase.ASSETLIFECYCLE_NINE_ID;

    public Guid DEPARTMENT_ID => AssetTestDataSeedingForDatabase.DEPARTMENT_ID;
    public Guid DEPARTMENT_TWO_ID => AssetTestDataSeedingForDatabase.DEPARTMENT_TWO_ID;
    public Guid COMPANY_ID => AssetTestDataSeedingForDatabase.COMPANY_ID;
    public Guid COMPANY_ID_TWO => AssetTestDataSeedingForDatabase.COMPANY_ID_TWO;

    public Guid ASSETHOLDER_ONE_ID => AssetTestDataSeedingForDatabase.ASSETHOLDER_ONE_ID;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ReplaceDbContext<AssetsContext>(services);
            ReplaceDbContext<LoggingDbContext>(services);

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            using var assetsContext = scope.ServiceProvider.GetRequiredService<AssetsContext>();
            FeatureManager = new FeatureManagerStub();
            services.AddScoped<IFeatureManager, FeatureManagerStub>(f => FeatureManager);
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AssetWebApplicationFactory<TProgram>>>();
            try
            {
                AssetTestDataSeedingForDatabase.InitialiseDbForTests(assetsContext);
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