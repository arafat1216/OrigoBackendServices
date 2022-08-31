using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class CustomerServiceProviderServiceOrderAddonConfiguration : EntityBaseConfiguration<CustomerServiceProviderServiceOrderAddon>
    {

        public CustomerServiceProviderServiceOrderAddonConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<CustomerServiceProviderServiceOrderAddon> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'EntityV2' entity
            base.Configure(builder);

            /*
             * DB table configuration (keys, constraints, indexing, etc.)
             */

            builder.HasAlternateKey(x => new { x.CustomerServiceProviderId, x.ServiceOrderAddonId });


            /*
             * Configure relationships
             */

            builder.HasOne(e => e.CustomerServiceProvider)
                   .WithMany()
                   .HasForeignKey(x => x.CustomerServiceProviderId)
                   .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

            builder.HasOne(e => e.ServiceOrderAddon)
                   .WithMany()
                   .HasForeignKey(e => e.ServiceOrderAddonId)
                   .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

        }
    }
}
