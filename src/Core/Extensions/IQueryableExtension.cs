using System.Linq.Expressions;

namespace Common.Extensions
{
    /// <summary>
    ///     A collection of <see cref="IQueryable{T}"/> extensions.
    /// </summary>
    public static class IQueryableExtension
    {
        // Sources: 
        // https://nwb.one/blog/linq-extensions-pagination-order-by-property-name

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="T"> The type of the data in the data source. </typeparam>
        /// <param name="this"> The extended item. </param>
        /// <param name="propertyName"> The name of the key or property to sort by. </param>
        /// <returns> An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to a key. </returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> @this, string propertyName)
        {
            return @this.OrderBy(ToLambda<T>(propertyName));
        }


        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="T"> The type of the data in the data source. </typeparam>
        /// <param name="this"> The extended item. </param>
        /// <param name="propertyName"> The name of the key or property to sort by. </param>
        /// <returns> An <see cref="IOrderedQueryable{T}"/> whose elements are sorted in descending order to a key. </returns>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> @this, string propertyName)
        {
            return @this.OrderByDescending(ToLambda<T>(propertyName));
        }


        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }
    }
}
