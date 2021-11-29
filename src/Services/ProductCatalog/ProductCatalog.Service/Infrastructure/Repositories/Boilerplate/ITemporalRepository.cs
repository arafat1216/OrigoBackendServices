using ProductCatalog.Service.Models.Database.Interfaces;

namespace ProductCatalog.Service.Infrastructure.Repositories.Boilerplate
{
    /// <summary>
    ///     Defines temporal operations that is shared across all repositories.
    /// </summary>
    /// <inheritdoc/>
    /// <seealso cref="IReadRepository{TEntity}{TEntity}"/>
    /// <seealso cref="IRepository{TEntity}"/>
    /// <seealso cref="ITranslationRepository{TEntity}"/>
    internal interface ITemporalRepository<TEntity> : IReadRepository<TEntity> where TEntity : class, IEntityFrameworkEntity
    {

    }
}
