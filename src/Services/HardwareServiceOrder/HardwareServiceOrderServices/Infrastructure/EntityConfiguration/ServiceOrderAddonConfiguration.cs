using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceOrderAddonConfiguration : EntityBaseConfiguration<ServiceOrderAddon>
    {
        public ServiceOrderAddonConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<ServiceOrderAddon> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'EntityV2' entity
            base.Configure(builder);


            /*
             * Configure the database-table.
             */

            builder.HasComment("The service-order addons that is offered by a given service-provider.");


            /*
             * Configure properties
             */

            builder.Property(e => e.ThirdPartyId)
                   .HasComment("The ID that is used in the external service provider's systems.");


            /*
             * Configure relationships
             */

            builder.HasOne(e => e.ServiceProvider)
                   .WithMany(e => e.OfferedServiceOrderAddons)
                   .HasForeignKey(e => e.ServiceProviderId);
        }
    }
}
