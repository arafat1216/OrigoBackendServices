using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementContextFactory : IDesignTimeDbContextFactory<SubscriptionManagementContext>
    {
        public SubscriptionManagementContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                   .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                                                   .AddJsonFile("secrets/appsettings.secrets.json", optional: true)
                                                   .AddEnvironmentVariables()
                                                   .AddUserSecrets<SubscriptionManagementContextFactory>(optional: true)
                                                   .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SubscriptionManagementContext>();
            string connectionString = config.GetConnectionString("SubscriptionManagementConnectionString");

            optionsBuilder.UseSqlServer(connectionString);

            return new SubscriptionManagementContext(optionsBuilder.Options);
        }
    }
}
