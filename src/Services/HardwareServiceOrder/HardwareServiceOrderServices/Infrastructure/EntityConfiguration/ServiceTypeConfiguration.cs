using HardwareServiceOrderServices.Models;
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
            // Call the parent that configures the shared properties from the 'EntityV2' entity
            base.Configure(builder);

            /*
             * Properties
             */

            // Add as needed...
        }
    }
}
