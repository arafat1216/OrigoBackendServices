using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration
{
    /// <summary>
    ///     EntityFramework configurations for the <see cref="UserPreference" /> entity.
    /// </summary>
    internal class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
    {
        private readonly bool _isSqlLite;

        public UserPreferenceConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<UserPreference> builder)
        {

            /*
             * Properties
             */

            builder.Property(e => e.IsAssetTileClosed).HasDefaultValue(true);
            builder.Property(e => e.IsSubscriptionTileClosed).HasDefaultValue(true);

            builder.Property(e => e.Language)
               .HasComment("A Language using ISO-634 format.")
               .HasMaxLength(2)
               .IsUnicode(false);



        }
    }
}
