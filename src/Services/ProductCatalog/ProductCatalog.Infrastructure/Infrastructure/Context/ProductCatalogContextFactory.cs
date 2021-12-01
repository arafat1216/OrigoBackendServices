using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProductCatalog.Infrastructure.Infrastructure.Context
{
    internal class ProductCatalogContextFactory : IDesignTimeDbContextFactory<ProductCatalogContext>
    {
        public ProductCatalogContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                   .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                                                   .AddEnvironmentVariables()
                                                   .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ProductCatalogContext>();
            string connectionString = config.GetConnectionString("ProductCatalogDatabase");

            optionsBuilder.UseSqlServer(connectionString);

            /*
             * Please do not remove any of these loggers if they are commented out.
             * They should be left in place so they can be commented back in for debugging purposes.
             */

            //optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            //              .EnableSensitiveDataLogging();

            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);


            return new ProductCatalogContext(optionsBuilder.Options);
        }
    }
}
