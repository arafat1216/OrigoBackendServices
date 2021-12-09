namespace Boilerplate.EntityFramework.Generics.Repositories
{
    /// <summary>
    ///     
    /// </summary>
    /// <inheritdoc/>
    public interface ITemporalReadRepository<TEntity> : IReadRepository<TEntity>
                                                        where TEntity : class
    {
        public ITemporalRepositoryRoot<TEntity> Temporal { get; }
    }
}
