using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CustomerServices.Models;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration
{
    /// <summary>
    ///     EntityFramework configurations for the <see cref="OrganizationPreferences" /> entity.
    /// </summary>
    internal class OrganizationPreferencesConfiguration : IEntityTypeConfiguration<OrganizationPreferences>
    {
        private readonly bool _isSqlLite;

        public OrganizationPreferencesConfiguration(bool isSqlLite)
        { 
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<OrganizationPreferences> builder)
        {

            /*
             * Properties
             */

            builder.Property(e => e.PrimaryLanguage)
               .HasComment("A Language using ISO-634 format.")
               .HasMaxLength(2)
               .IsUnicode(false);
        }
    }
}
