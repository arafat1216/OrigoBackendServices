using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Infrastructure.Context.EntityConfiguration;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context
{
    internal partial class ProductCatalogContext : DbContext
    {
        public DbSet<Feature> Features { get; set; } = null!;


        public ProductCatalogContext(DbContextOptions<ProductCatalogContext> options) : base(options)
        {
        }

        /*
        public ProductCatalogContext()
        {
        }
        */

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=DeleteMe;Trusted_Connection=True");

            //optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            //              .EnableSensitiveDataLogging();

            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        }
        */



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FeatureConfiguration());
        }
    }
}
