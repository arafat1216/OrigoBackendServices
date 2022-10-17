using AssetServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetServices.Infrastructure.EntityConfiguration
{
    internal class CustomerSettingConfiguration : EntityBaseConfiguration<CustomerSettings>
    {
        public CustomerSettingConfiguration(bool isSqLite) : base(isSqLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<CustomerSettings> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'Entity'-class
            base.Configure(builder);


            /*
             * DB table configuration (keys, constraints, indexing, etc.)
             */

            // TODO: Re-add this key/index once bug #11063 has been fixed, and the duplicate unique items has been cleared out.
            //builder.HasAlternateKey(entity => entity.CustomerId);


            /*
             * Configure properties
             */

            // Add as needed.


            /*
             * Register and configure owned properties
             */

            builder.OwnsOne(s => s.DisposeSetting)
                   .OwnsMany(x => x.ReturnLocations)
                   .ToTable(nameof(ReturnLocation));
        }
    }
}
