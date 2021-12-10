namespace Boilerplate.EntityFramework.Generics.Repositories
{
    /// <summary>
    ///     
    /// </summary>
    /// <inheritdoc/>
    public interface ITemporalReadWriteRepository<TEntity> : IReadWriteRepository<TEntity>
                                                             where TEntity : class
    {
        public ITemporalRepositoryRoot<TEntity> Temporal { get; }
    }
}
