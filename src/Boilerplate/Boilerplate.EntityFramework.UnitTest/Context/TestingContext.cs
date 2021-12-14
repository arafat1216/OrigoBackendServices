using Microsoft.EntityFrameworkCore;

namespace Boilerplate.EntityFramework.UnitTest
{
    internal class TestingContext : DbContext
    {
        public TestingContext(DbContextOptions<TestingContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>(b =>
            {
                b.Property("_id");
                b.HasKey("_id");
                b.Property(e => e.Name);
                b.HasMany(e => e.Tags).WithOne().IsRequired();
            });

            modelBuilder.Entity<Tag>(b =>
            {
                b.Property("_id");
                b.HasKey("_id");
                b.Property(e => e.Label);
            });
        }
    }
}