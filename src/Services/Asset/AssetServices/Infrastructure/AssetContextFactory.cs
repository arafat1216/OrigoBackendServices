using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AssetServices.Infrastructure
{
    // ReSharper disable once UnusedType.Global
    public class AssetContextFactory : IDesignTimeDbContextFactory<AssetsContext>
    {
        public AssetsContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<AssetContextFactory>()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AssetsContext> ();
            var connectionString = configuration.GetConnectionString("AssetConnectionString");

            optionsBuilder.UseSqlServer(connectionString);

            return new AssetsContext(optionsBuilder.Options);
        }
    }
}