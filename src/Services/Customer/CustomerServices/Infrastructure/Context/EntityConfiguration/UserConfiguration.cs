using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration;



internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    private readonly bool _isSqlLite;

    public UserConfiguration(bool isSqlLite)
    {
        _isSqlLite = isSqlLite;
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        // A value comparer for keys. Used to force EF Core into case-insensitive string comparisons like in the database.
        // Source: Use case-insensitive string keys (https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=fluent-api#use-case-insensitive-string-keys)
        var comparer = new ValueComparer<string>(
            (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
            v => v.ToLowerInvariant().GetHashCode(),
            v => v
        );

        builder.ToTable("User");

        builder.HasIndex(e => e.Email).IsUnique();

        /*
         * Properties
         */

        builder.Property(e => e.UserId).HasDefaultValueSql("NEWID()").ValueGeneratedOnAdd().Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Throw);

        builder.Property(e => e.Email)
                   .HasMaxLength(320)
                   .Metadata.SetValueComparer(comparer);

        builder.Property(e => e.MobileNumber)
                   .HasComment("A phone-number using E.164 format.")
                   .HasMaxLength(15)
                   .IsUnicode(false);

        /*
         * Relationships / Navigation
         */

        builder.HasMany(u => u.ManagesDepartments).WithMany(d => d.Managers)
            .UsingEntity(join => join.ToTable("DepartmentManager"));

        builder.HasOne(e => e.Department).WithMany(d => d.Users);
    }
}