using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Service.Models.Database.Interfaces;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace ProductCatalog.Service.Infrastructure.Repositories.Boilerplate
{
    /// <summary>
    ///     Extends <see cref="IRepository{TEntity}"/> by adding in translation operations.
    /// </summary>
    /// <inheritdoc/>
    /// <seealso cref="IReadRepository{TEntity}"/>
    /// <seealso cref="IRepository{TEntity}"/>
    /// <seealso cref="ITemporalRepository{TEntity}"/>
    internal interface ITranslationRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntityFrameworkEntity
    {
        new Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync(string language, bool asNoTracking, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<string> languages, bool asNoTracking, CancellationToken cancellationToken = default);


        new IAsyncEnumerable<TEntity> GetAllAsStream(bool asNoTracking);
        IAsyncEnumerable<TEntity> GetAllAsStream(string language, bool asNoTracking);
        IAsyncEnumerable<TEntity> GetAllAsStream(IEnumerable<string> languages, bool asNoTracking);


        new Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(string language, Expression<Func<TEntity, bool>> predicate, bool asNoTracking, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(IEnumerable<string> languages, Expression<Func<TEntity, bool>> predicate, bool asNoTracking, CancellationToken cancellationToken = default);


        new IAsyncEnumerable<TEntity> FindAsStream(Expression<Func<TEntity, bool>> predicate, bool asNoTracking);
        IAsyncEnumerable<TEntity> FindAsStream(string language, Expression<Func<TEntity, bool>> predicate, bool asNoTracking);
        IAsyncEnumerable<TEntity> FindAsStream(IEnumerable<string> languages, Expression<Func<TEntity, bool>> predicate, bool asNoTracking);
        
        Task<ITranslationResult?> GetFallbackTranslation<TId>(TId id);

        Task AddTranslationAsync<TTranslation>(TTranslation translation) where TTranslation : class, IEntityFrameworkEntity, ITranslationResult;
        Task AddTranslationRangeAsync<TTranslation>(IEnumerable<TTranslation> translation) where TTranslation : class, IEntityFrameworkEntity, ITranslationResult;

    }
}
