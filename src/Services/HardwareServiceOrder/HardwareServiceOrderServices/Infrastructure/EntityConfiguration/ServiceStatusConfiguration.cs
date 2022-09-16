using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceStatusConfiguration : EntityBaseConfiguration<ServiceStatus>
    {
        public ServiceStatusConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }

        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<ServiceStatus> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'EntityV2' entity
            base.Configure(builder);


            /*
             * Configure the database-table.
             */

            builder.ToTable(e => e.IsTemporal());


            /*
             * Properties
             */

            // Add as needed...
        }
    }
}
