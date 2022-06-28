using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceProviderConfiguration : AuditableBaseConfiguration<ServiceProvider>
    {
        public ServiceProviderConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }

        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<ServiceProvider> builder)
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
