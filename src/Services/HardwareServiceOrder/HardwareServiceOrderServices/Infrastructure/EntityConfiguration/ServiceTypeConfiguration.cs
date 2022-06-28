using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceTypeConfiguration : AuditableBaseConfiguration<ServiceType>
    {
        public ServiceTypeConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }

        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<ServiceType> builder)
        {
            // Call the parent that configures the shared properties from the 'Auditable' entity
            base.Configure(builder);

            /*
             * Properties
             */

            // Add as needed...
        }
    }
}
