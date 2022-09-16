using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceTypeConfiguration : EntityBaseConfiguration<ServiceType>
    {
        public ServiceTypeConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }

        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<ServiceType> builder)
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
