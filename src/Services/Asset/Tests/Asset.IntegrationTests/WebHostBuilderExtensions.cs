using System;
using Asset.API.Controllers;
using AssetServices.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Asset.IntegrationTests
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureTestDatabase(this IWebHostBuilder builder, Action<AssetsContext> configure)
        {
            return builder.ConfigureTestServices(services =>
            {
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AssetsContext>();
                var logger = scopedServices.GetRequiredService<ILogger<AssetTestDataController>>();

                try
                {
                    configure(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred setting up the " +
                                        "database for the test. Error: {Message}", ex.Message);
                }
            });
        }
    }
}
