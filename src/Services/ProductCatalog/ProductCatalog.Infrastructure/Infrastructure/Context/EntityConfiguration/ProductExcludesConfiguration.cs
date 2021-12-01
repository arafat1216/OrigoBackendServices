using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Infrastructure.Models.Database.Joins;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
{
    internal class ProductExcludesConfiguration : IEntityTypeConfiguration<ProductExcludes>
    {
        public void Configure(EntityTypeBuilder<ProductExcludes> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasKey(e => new { e.ProductId, e.ExcludesProductId });

            builder.HasOne(pe => pe.Product)
                   .WithMany(p => p.Excludes)
                   .HasForeignKey(pe => pe.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pe => pe.RequiresProduct)
                   .WithMany(p => p.HasExcludesDependenciesFrom)
                   .HasForeignKey(pe => pe.ExcludesProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
