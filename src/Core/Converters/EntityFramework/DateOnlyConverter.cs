using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Common.Converters.EntityFramework
{
    // Source: https://github.com/dotnet/efcore/issues/24507#issuecomment-961086782

    /// <summary>
    /// Converts <see cref="DateOnly" /> to <see cref="DateTime"/> and vice versa.
    /// </summary>
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public DateOnlyConverter() : base(
                dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
                dateTime => DateOnly.FromDateTime(dateTime))
        {

        }
    }

    /// <summary>
    /// Compares <see cref="DateOnly" />.
    /// </summary>
    public class DateOnlyComparer : ValueComparer<DateOnly>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public DateOnlyComparer() : base(
            (d1, d2) => d1 == d2 && d1.DayNumber == d2.DayNumber,
            dateOnly => dateOnly.GetHashCode())
        {

        }
    }

    /// <summary>
    /// Converts <see cref="DateOnly?" /> to <see cref="DateTime?"/> and vice versa.
    /// </summary>
    public class NullableDateOnlyConverter : ValueConverter<DateOnly?, DateTime?>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public NullableDateOnlyConverter() : base(
            dateOnly => dateOnly == null
                ? null
                : new DateTime?(dateOnly.Value.ToDateTime(TimeOnly.MinValue)),
            dateTime => dateTime == null
                ? null
                : new DateOnly?(DateOnly.FromDateTime(dateTime.Value)))
        {

        }
    }

    /// <summary>
    /// Compares <see cref="DateOnly?" />.
    /// </summary>
    public class NullableDateOnlyComparer : ValueComparer<DateOnly?>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public NullableDateOnlyComparer() : base(
            (d1, d2) => d1 == d2 && d1.GetValueOrDefault().DayNumber == d2.GetValueOrDefault().DayNumber,
            dateOnly => dateOnly.GetHashCode())
        {

        }
    }
}