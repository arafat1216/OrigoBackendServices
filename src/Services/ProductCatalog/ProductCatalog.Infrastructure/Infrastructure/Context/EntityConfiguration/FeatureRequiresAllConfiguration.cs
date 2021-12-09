using FeatureCatalog.Infrastructure.Models.Database.Joins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
{
    internal class FeatureRequiresAllConfiguration : IEntityTypeConfiguration<FeatureRequiresAll>
    {
        public void Configure(EntityTypeBuilder<FeatureRequiresAll> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasKey(e => new { e.FeatureId, e.RequiresFeatureId });

            builder.HasOne(fe => fe.Feature)
                   .WithMany(f => f.RequiresAll)
                   .HasForeignKey(fe => fe.FeatureId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fe => fe.RequiresFeature)
                   .WithMany(f => f.HasRequiresAllDependenciesFrom)
                   .HasForeignKey(fe => fe.RequiresFeatureId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
