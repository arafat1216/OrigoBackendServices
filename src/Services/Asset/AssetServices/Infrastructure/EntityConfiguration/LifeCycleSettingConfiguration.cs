using AssetServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetServices.Infrastructure.EntityConfiguration
{
    internal class LifeCycleSettingConfiguration : EntityBaseConfiguration<LifeCycleSetting>
    {
        public LifeCycleSettingConfiguration(bool isSqLite) : base(isSqLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<LifeCycleSetting> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'Entity'-class
            base.Configure(builder);


            /*
             * DB table configuration (keys, constraints, indexing, etc.)
             */

            builder.HasAlternateKey(entity => entity.ExternalId);

            builder.Property(a => a.Runtime)
                .HasDefaultValue(36);
            

            /*
             * Configure properties
             */

            builder.OwnsOne(a => a.MinBuyoutPrice, b =>
            {
                b.Property(p => p.CurrencyCode).HasConversion(CurrencyConverter);
            });
        }
    }
}
