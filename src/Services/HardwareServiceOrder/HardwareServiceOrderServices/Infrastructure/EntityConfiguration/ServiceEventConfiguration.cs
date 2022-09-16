using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceEventConfiguration : EntityBaseConfiguration<ServiceEvent>
    {
        public ServiceEventConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<ServiceEvent> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'EntityV2' entity
            base.Configure(builder);


            /*
             * DB table configuration (keys, constraints, indexing, etc.)
             */

            builder.ToTable("ServiceEvents", e => e.IsTemporal());


            /*
             * Properties
             */

            builder.Property(e => e.Timestamp)
                   .HasComment("When this event was recorded in the external service-provider's system");
        }
    }
}
