using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Context.EntityConfiguration
{
    internal class FeatureConfiguration : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.ToTable("Feature");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FeatureTypeId).IsRequired();
            builder.Property(e => e.AccessControlPermissionNode).IsRequired();
        }
    }
}
