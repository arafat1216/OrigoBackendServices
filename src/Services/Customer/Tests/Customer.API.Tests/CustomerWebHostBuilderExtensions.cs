using Customer.API.Controllers;
using CustomerServices.Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Customer.API.IntegrationTests
{
    public static class CustomerWebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureTestDatabase(this IWebHostBuilder builder, Action<CustomerContext> configure)
        {
            return builder.ConfigureTestServices(services =>
            {
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<CustomerContext>();
                var logger = scopedServices.GetRequiredService<ILogger<OrganizationTestDataController>>();

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
