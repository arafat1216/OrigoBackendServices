using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Infrastructure.Models.Database.Joins;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
{
    internal class ProductFeatureConfiguration : IEntityTypeConfiguration<ProductFeature>
    {
        public void Configure(EntityTypeBuilder<ProductFeature> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            /*
             * Properties
             */

            builder.Property(e => e.ProductId)
                   .HasColumnOrder(0);

            builder.Property(e => e.FeatureId)
                   .HasColumnOrder(1);

            /*
             * Relationships / Navigation
             */

            builder.HasOne(e => e.Product)
                   .WithMany(e => e.ProductFeatures)
                   .HasForeignKey(e => e.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Feature)
                   .WithMany(e => e.ProductFeatures)
                   .HasForeignKey(e => e.FeatureId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
