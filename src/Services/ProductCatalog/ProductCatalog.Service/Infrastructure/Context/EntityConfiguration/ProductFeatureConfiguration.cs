using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context.EntityConfiguration
{
    internal class ProductFeatureConfiguration : IEntityTypeConfiguration<ProductFeature>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProductFeature> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasComment("Join table");
        }
    }
}
