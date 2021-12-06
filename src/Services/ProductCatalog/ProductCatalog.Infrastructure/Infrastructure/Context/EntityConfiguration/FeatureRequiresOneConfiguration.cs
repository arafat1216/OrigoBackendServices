using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FeatureCatalog.Infrastructure.Models.Database.Joins;

namespace ProductCatalog.Infrastructure.Infrastructure.Context.EntityConfiguration
{
    internal class FeatureRequiresOneConfiguration : IEntityTypeConfiguration<FeatureRequiresOne>
    {
        public void Configure(EntityTypeBuilder<FeatureRequiresOne> builder)
        {
            builder.ToTable(t => t.IsTemporal());

            builder.HasKey(e => new { e.FeatureId, e.RequiresFeatureId });

            builder.HasOne(fe => fe.Feature)
                   .WithMany(f => f.RequiresOne)
                   .HasForeignKey(fe => fe.FeatureId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fe => fe.RequiresFeature)
                   .WithMany(f => f.HasRequiresOneDependenciesFrom)
                   .HasForeignKey(fe => fe.RequiresFeatureId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
