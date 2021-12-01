namespace Boilerplate.EntityFramework.Generics.Repositories
{
    /// <summary>
    ///     
    /// </summary>
    /// <inheritdoc/>
    public interface IReadWriteRepository<TEntity> : IReadRepository<TEntity>
                                                     where TEntity : class
    {
        /// <summary>
        ///     Create a new entity. If the entity may already exist, consider using <see cref="AddOrUpdate(TEntity)"/> instead.
        /// </summary>
        /// <param name="entity"> The entity to be added. </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns> </returns>
        public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Create new entities from a list. If some of the entities may already exist, consider using <see cref="AddOrUpdateRange(IEnumerable{TEntity})"/> instead.
        /// </summary>
        /// <param name="entities"> The collection of entities to be added. </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete. </param>
        /// <returns> </returns>
        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"> The entity to be added or updated. </param>
        public void AddOrUpdate(TEntity entity);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"> A collection of entities to be added or updated. </param>
        public void AddOrUpdateRange(IEnumerable<TEntity> entities);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityToUpdate"> The entity to be updated. </param>
        public void Update(TEntity entityToUpdate);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entitiesToUpdate"> A collection of entities to be updated. </param>
        public void UpdateRange(IEnumerable<TEntity> entitiesToUpdate);


        /// <summary>
        ///     Deletes a entity.
        /// </summary>
        /// <param name="entityToDelete"> The entity to be deleted. </param>
        public void Delete(TEntity entityToDelete);

        /// <summary>
        ///     
        /// </summary>
        /// <param name="entitiesToDelete"> A collection of entities to be deleted. </param>
        public void DeleteRange(IEnumerable<TEntity> entitiesToDelete);

        /// <summary>
        ///     Retrieves an entity using it's primary key(s) before marking it for deletion. <para>
        ///     
        ///     If the entity is tracked, consider using <see cref="Delete(TEntity)"/> for better performance as this will eliminate the
        ///     need to get the entity from the database. </para>
        /// </summary>
        /// <typeparam name="T"> The data-type used for the primary key(s). </typeparam>
        /// <param name="id"> The primary key(s) for the entity. </param>
        /// <returns> </returns>
        public Task Delete<T>(T id) where T : notnull;
    }
}
