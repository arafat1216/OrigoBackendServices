using Microsoft.EntityFrameworkCore;

namespace OrigoAssetServices.Models
{
    public class AssetsContext : DbContext
    {
        public AssetsContext(DbContextOptions<AssetsContext> options) : base(options)
        {
        }

        public DbSet<Asset> Assets {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>().ToTable("Assets");
        }
    }
}
