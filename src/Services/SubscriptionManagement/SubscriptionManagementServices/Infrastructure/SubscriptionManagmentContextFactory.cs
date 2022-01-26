using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagmentContextFactory : IDesignTimeDbContextFactory<SubscriptionManagmentContext>
    {
        public SubscriptionManagmentContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                   .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                                                   .AddJsonFile("secrets/appsettings.secrets.json", optional: true)
                                                   .AddEnvironmentVariables()
                                                   .AddUserSecrets<SubscriptionManagmentContextFactory>()
                                                   .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SubscriptionManagmentContext>();
            string connectionString = config.GetConnectionString("SubscriptionManagmentConnectionString");

            optionsBuilder.UseSqlServer(connectionString);

            return new SubscriptionManagmentContext(optionsBuilder.Options);
        }
    }
}
