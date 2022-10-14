using AssetServices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetServices.Infrastructure.EntityConfiguration
{
    internal class CustomerLabelConfiguration : EntityBaseConfiguration<CustomerLabel>
    {
        public CustomerLabelConfiguration(bool isSqLite) : base(isSqLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<CustomerLabel> builder)
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

            // Add as needed.

        }
    }
}
