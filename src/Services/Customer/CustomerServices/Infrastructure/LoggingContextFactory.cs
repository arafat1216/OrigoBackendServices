using System.IO;
using Common.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CustomerServices.Infrastructure
{
    // ReSharper disable once UnusedType.Global
    public class LoggingContextFactory : IDesignTimeDbContextFactory<LoggingDbContext>
    {
        public LoggingDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true).AddJsonFile("appsettings.Development.json", true, true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<LoggingDbContext>();
            var connectionString = configuration.GetConnectionString("CustomerConnectionString");
            optionsBuilder.UseSqlServer(connectionString, builder =>
            {
                builder.MigrationsAssembly("CustomerServices");
            });

            return new LoggingDbContext(optionsBuilder.Options);
        }
    }
}