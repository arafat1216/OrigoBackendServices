using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AssetServices.Infrastructure
{
    // ReSharper disable once UnusedType.Global
    public class AssetContextFactory : IDesignTimeDbContextFactory<AssetsContext>
    {
        public AssetsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AssetsContext>();
            optionsBuilder.UseSqlServer("Server=SAL-9000;Database=Assets;Trusted_Connection=True;");

            return new AssetsContext(optionsBuilder.Options);
        }
    }
}