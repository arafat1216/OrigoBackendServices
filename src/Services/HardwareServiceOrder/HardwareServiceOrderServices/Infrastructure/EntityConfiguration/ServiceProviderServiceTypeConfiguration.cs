using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ServiceProviderServiceTypeConfiguration : EntityBaseConfiguration<ServiceProviderServiceType>
    {
        public ServiceProviderServiceTypeConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<ServiceProviderServiceType> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'EntityV2' entity
            base.Configure(builder);


            /*
             * Configure the database-table.
             */

            builder.ToTable(e => e.IsTemporal());
            builder.HasComment("Determines what service-types is available for a given service-provider.");
            builder.HasAlternateKey(e => new { e.ServiceProviderId, e.ServiceTypeId }); // The alternate key is what we will be using.


            /*
             * Configure properties
             */




            /*
             * Configure relationships.
             */

            builder.HasOne(e => e.ServiceProvider)
                   .WithMany(e => e.SupportedServiceTypes);

            builder.HasOne(e => e.ServiceType)
                   .WithMany();
        }
    }
}
