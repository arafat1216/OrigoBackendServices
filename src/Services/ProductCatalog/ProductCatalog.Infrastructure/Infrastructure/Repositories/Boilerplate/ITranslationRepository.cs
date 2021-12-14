using Boilerplate.EntityFramework.Generics.Repositories;
using ProductCatalog.Common.Interfaces;
using System.Linq.Expressions;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories.Boilerplate
{
    /// <summary>
    ///     Extends <see cref="IRepository{TEntity}"/> by adding in translation operations.
    /// </summary>
    /// <inheritdoc/>
    /// <seealso cref="IReadRepository{TEntity}"/>
    /// <seealso cref="IRepository{TEntity}"/>
    /// <seealso cref="ITemporalRepository{TEntity}"/>
    internal interface ITranslationRepository<TEntity> : ITemporalReadWriteRepository<TEntity> where TEntity : class
    {
        /// <inheritdoc/>
        new public Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
                                                       Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                                       string includeProperties = "",
                                                       int? pageIndex = null,
                                                       int pageSize = 50,
                                                       bool asNoTracking = false,
                                                       CancellationToken cancellationToken = default
        );


        /// <inheritdoc cref="GetAsync(Expression{Func{TEntity, bool}}?, Func{IQueryable{TEntity}, IOrderedQueryable{TEntity}}?, string, int?, int, bool, CancellationToken)"/>
        /// <remarks>
        ///     Only the selected language will be included in <see cref="ITranslatable{TEntity}.Translations"/>.
        /// </remarks>
        /// <param name="language"> The language to be included. </param>
        public Task<IEnumerable<TEntity>> GetAsync(string language,
                                                   Expression<Func<TEntity, bool>>? filter = null,
                                                   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                                   string includeProperties = "",
                                                   int? pageIndex = null,
                                                   int pageSize = 50,
                                                   bool asNoTracking = false,
                                                   CancellationToken cancellationToken = default
        );


        /// <inheritdoc cref="GetAsync(Expression{Func{TEntity, bool}}?, Func{IQueryable{TEntity}, IOrderedQueryable{TEntity}}?, string, int?, int, bool, CancellationToken)"/>
        /// <remarks>
        ///     Only the selected languages will be included in <see cref="ITranslatable{TEntity}.Translations"/>.
        /// </remarks>
        /// <param name="languages"> The languages to be included. </param>
        public Task<IEnumerable<TEntity>> GetAsync(IEnumerable<string> languages,
                                                   Expression<Func<TEntity, bool>>? filter = null,
                                                   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                                   string includeProperties = "",
                                                   int? pageIndex = null,
                                                   int pageSize = 50,
                                                   bool asNoTracking = false,
                                                   CancellationToken cancellationToken = default
        );

    }
}
