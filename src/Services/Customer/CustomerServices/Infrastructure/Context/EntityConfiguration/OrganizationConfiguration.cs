using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration
{
    /// <summary>
    ///     EntityFramwork configurations for the <see cref="Organization"/> entity.
    /// </summary>
    internal class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        private readonly bool _isSqlLite;

        public OrganizationConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("Organization");

            builder.HasAlternateKey(e => e.OrganizationId);

            /*
             * Properties
             */
            
            builder.Property(e => e.OrganizationId)
                   .HasDefaultValueSql("NEWID()")
                   .ValueGeneratedOnAdd()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        }
    }
}
