using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using AssetServices.Infrastructure;
using AssetServices.Models;
using Common.Enums;
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
    private readonly Guid CALLER_ID = new("da031680-abb0-11ec-849b-00155d3196a5");
    public readonly Guid ASSETLIFECYCLE_ONE_ID = new("4e7413da-54c9-4f79-b882-f66ce48e5074");
    private readonly Guid ASSETLIFECYCLE_TWO_ID = new("6c38b551-a5c2-4f53-8df8-221bf8485c61");
    private readonly Guid ASSETLIFECYCLE_THREE_ID = new("80665d26-90b4-4a3a-a20d-686b64466f32");
    private readonly Guid ASSETLIFECYCLE_FOUR_ID = new("bdb4c26c-33fd-40d7-a237-e74728609c1c");
    private readonly Guid ASSETLIFECYCLE_FIVE_ID = new("4315bba8-698f-4ddd-aee2-82554c91721f");
    public readonly Guid DEPARTMENT_ID = new("6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72");
    public readonly Guid COMPANY_ID = new("cab4bb77-3471-4ab3-ae5e-2d4fce450f36");
    protected readonly Guid ASSETHOLDER_ONE_ID = new("6d16a4cb-4733-44de-b23b-0eb9e8ae6590");
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
                var assetOne = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012345", "Samsung", "Samsung Galaxy S20", new List<AssetImei>() { new AssetImei(500119468586675) }, "B26EDC46046B");

                var assetTwo = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012364", "Apple", "Apple iPhone 8", new List<AssetImei>() { new AssetImei(546366434558702) }, "487027C99FA1");

                var assetThree = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012399", "Samsung", "Samsung Galaxy S21", new List<AssetImei>() { new AssetImei(512217111821626) }, "840F1D0C06AD");
                
                var assetFour = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012397", "Apple", "iPhone 11 Pro", new List<AssetImei>() { new AssetImei(512217111821624) }, "840F1D0C06AB");

                var userOne = new User { ExternalId = ASSETHOLDER_ONE_ID };
                var assetLifecycleOne = new AssetLifecycle(ASSETLIFECYCLE_ONE_ID) { CustomerId = COMPANY_ID, Alias = "alias_0", AssetLifecycleStatus = AssetLifecycleStatus.InputRequired };
                assetLifecycleOne.AssignAsset(assetOne, CALLER_ID);
                assetLifecycleOne.AssignContractHolder(userOne, CALLER_ID);

                var assetLifecycleTwo = new AssetLifecycle(ASSETLIFECYCLE_TWO_ID) { CustomerId = COMPANY_ID, Alias = "alias_1", AssetLifecycleStatus = AssetLifecycleStatus.Available };
                assetLifecycleTwo.AssignAsset(assetTwo, CALLER_ID);
                assetLifecycleTwo.AssignContractHolder(userOne, CALLER_ID);

                var assetLifecycleThree = new AssetLifecycle(ASSETLIFECYCLE_THREE_ID) { CustomerId = COMPANY_ID, Alias = "alias_2", AssetLifecycleStatus = AssetLifecycleStatus.Active };
                assetLifecycleThree.AssignAsset(assetThree, CALLER_ID);
                assetLifecycleThree.AssignDepartment(DEPARTMENT_ID, CALLER_ID);
                assetLifecycleThree.AssignContractHolder(userOne, CALLER_ID);

                var assetLifecycleFour = new AssetLifecycle(ASSETLIFECYCLE_FOUR_ID) { CustomerId = COMPANY_ID, Alias = "alias_3", AssetLifecycleStatus = AssetLifecycleStatus.Available };
                assetLifecycleFour.AssignAsset(assetFour, CALLER_ID);
                assetLifecycleFour.AssignContractHolder(userOne, CALLER_ID);

                var assetLifecycleFive = new AssetLifecycle(ASSETLIFECYCLE_FIVE_ID) { CustomerId = COMPANY_ID, Alias = "alias_4", AssetLifecycleStatus = AssetLifecycleStatus.Available };
                assetLifecycleFive.AssignAsset(assetFour, CALLER_ID);
                assetLifecycleFive.AssignDepartment(DEPARTMENT_ID, CALLER_ID);
                assetLifecycleFive.AssignContractHolder(userOne, CALLER_ID);


                assetsContext.SaveChanges();

                assetsContext.Users.AddRange(userOne);
                assetsContext.Assets.AddRange(assetOne, assetTwo, assetThree);
                assetsContext.AssetLifeCycles.AddRange(assetLifecycleOne, assetLifecycleTwo, assetLifecycleThree, assetLifecycleFour, assetLifecycleFive);
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