using AssetServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.Cryptography.Xml;

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

            builder.HasAlternateKey(entity => entity.CustomerId);


            /*
             * Configure properties
             */

            // Add as needed.


            /*
             * Register and configure owned properties
             */

            builder.OwnsOne(s=>s.DisposeSetting)
                   .OwnsMany(x=>x.ReturnLocations)
                   .ToTable(nameof(ReturnLocation));
        }
    }
}
