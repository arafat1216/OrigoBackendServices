using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration
{
    /// <summary>
    ///     EntityFramwork configurations for the <see cref="Organization"/> entity.
    /// </summary>
    internal class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("Organization");

            /*
             * Properties
             */

            builder.Property(s => s.LastUpdatedDate)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
