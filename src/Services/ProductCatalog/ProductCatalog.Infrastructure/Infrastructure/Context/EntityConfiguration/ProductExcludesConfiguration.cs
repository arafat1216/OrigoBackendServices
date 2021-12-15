using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Infrastructure.Models.Database.Joins;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
{
    internal class ProductExcludesConfiguration : IEntityTypeConfiguration<ProductExcludes>
    {
        public void Configure(EntityTypeBuilder<ProductExcludes> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasKey(e => new { e.ProductId, e.ExcludesProductId });

            /*
             * Properties
             */

            builder.Property(e => e.ProductId)
                   .HasColumnOrder(0)
                   .HasComment("The product that has exclusions");

            builder.Property(e => e.ExcludesProductId)
                   .HasColumnOrder(1)
                   .HasComment($"The 'ProductId' cant be combined or used with this product. This is a one-way requirement");

            /*
             * Relationships / Navigation
             */

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
