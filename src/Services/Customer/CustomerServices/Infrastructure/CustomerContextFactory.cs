using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CustomerServices.Infrastructure
{
    // ReSharper disable once UnusedType.Global
    public class CustomerContextFactory : IDesignTimeDbContextFactory<CustomerContext>
    {
        public CustomerContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<CustomerContextFactory>(optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CustomerContext>();
            var connectionString = configuration.GetConnectionString("CustomerConnectionString");
            optionsBuilder.UseSqlServer(connectionString);

            return new CustomerContext(optionsBuilder.Options);
        }
    }
}