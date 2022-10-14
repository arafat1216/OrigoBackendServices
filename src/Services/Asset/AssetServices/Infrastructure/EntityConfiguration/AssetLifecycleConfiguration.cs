using AssetServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetServices.Infrastructure.EntityConfiguration;

internal class AssetLifecycleConfiguration : EntityBaseConfiguration<AssetLifecycle>
{
    public AssetLifecycleConfiguration(bool isSqLite) : base(isSqLite)
    {
    }


    /// <inheritdoc/>
    public override void Configure(EntityTypeBuilder<AssetLifecycle> builder)
    {
        // Call the parent that configures the shared properties from the inherited 'Entity'-class
        base.Configure(builder);


        /*
         * DB table configuration (keys, constraints, indexing, etc.)
         */

        builder.HasAlternateKey(entity => entity.ExternalId);

        builder.HasIndex(entity => entity.CustomerId);


        /*
         * Configure properties
         */

        builder.Property(a => a.PaidByCompany)
               .HasColumnType("decimal(18,2)");

        builder.Property(a => a.OffboardBuyoutPrice)
               .HasColumnType("decimal(18,2)")
               .HasDefaultValue(0m);

    }
}