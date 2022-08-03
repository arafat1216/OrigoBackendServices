using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceProviderConfiguration : EntityBaseConfiguration<ServiceProvider>
    {
        public ServiceProviderConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }

        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<ServiceProvider> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'EntityV2' entity
            base.Configure(builder);


            /*
             * Configure relationships
             */

            
        }
    }
}
