using CustomerServices.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration
{
    /// <summary>
    ///     EntityFramwork configurations for the <see cref="Partner"/> entity.
    /// </summary>
    internal class PartnerConfiguration : IEntityTypeConfiguration<Partner>
    {
        private readonly bool _isSqlLite;

        public PartnerConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<Partner> builder)
        {
            builder.ToTable("Partner");

            builder.HasAlternateKey(e => e.ExternalId);

            /*
             * Properties
             */

            builder.Property(e => e.CreatedDate)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "GETUTCDATE()")
                   .ValueGeneratedOnAdd()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Property(e => e.LastUpdatedDate)
                   .HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "GETUTCDATE()")
                   .ValueGeneratedOnAddOrUpdate();

            builder.Property(e => e.ExternalId)
                   .HasDefaultValueSql("NEWID()")
                   .ValueGeneratedOnAdd()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            /*
             * Relationships / Navigation
             */

            builder.HasMany(e => e.Customers)
                   .WithOne(e => e.Partner);

            // Enable eager loading so it's always included
            //builder.Navigation(e => e.Organization)
            //       .AutoInclude();
        }
    }
}
