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
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Development.json"))
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AssetsContext> ();
            var connectionString = configuration.GetConnectionString("AssetConnectionString");

            optionsBuilder.UseSqlServer(connectionString);

            return new AssetsContext(optionsBuilder.Options);
        }
    }
}