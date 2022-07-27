using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class CustomerServiceProviderConfiguration : EntityBaseConfiguration<CustomerServiceProvider>
    {

        public CustomerServiceProviderConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<CustomerServiceProvider> builder)
        {
            // Call the parent that configures the shared properties from the 'EntityV2' entity
            base.Configure(builder);

            builder.ToTable("CustomerServiceProvider");

            builder.HasKey(x => new { x.CustomerId, x.Id, x.ServiceProviderId });

            /*
             * Properties
             */


        }
    }
}
