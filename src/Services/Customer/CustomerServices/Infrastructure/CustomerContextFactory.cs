using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CustomerServices.Infrastructure
{
    // ReSharper disable once UnusedType.Global
    public class CustomerContextFactory : IDesignTimeDbContextFactory<CustomerContext>
    {
        public CustomerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CustomerContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=Customers;Trusted_Connection=True;");

            return new CustomerContext(optionsBuilder.Options);
        }
    }
}