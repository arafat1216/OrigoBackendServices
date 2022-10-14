using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration
{
    /// <summary>
    ///     EntityFramwork configurations for the <see cref="Location"/> entity.
    /// </summary>
    internal class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        private readonly bool _isSqlLite;

        public LocationConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations");

            /*
             * Properties
             */

            builder.Property(e => e.Country)
                   .HasComment("The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.")
                   .HasMaxLength(2)
                   .IsFixedLength()
                   .IsUnicode(false);
        }
    }
}
