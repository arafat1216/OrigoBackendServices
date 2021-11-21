using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context.EntityConfiguration
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasIndex(e => e.PartnerId);

            builder.OwnsMany(e => e.Translations);

            builder.HasMany(e => e.Excludes).WithMany(e => e.HasExcludesDependencyFrom);
            builder.HasMany(e => e.RequiresAll).WithMany(e => e.HasRequiresAllDependencyFrom);
            builder.HasMany(e => e.RequiresOne).WithMany(e => e.HasRequiresOneDependencyFrom);
        }
    }
}
