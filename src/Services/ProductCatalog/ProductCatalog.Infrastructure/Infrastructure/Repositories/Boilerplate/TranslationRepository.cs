using Boilerplate.EntityFramework.Generics.Repositories;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.Models.Database.Interfaces;
using System.Linq.Expressions;

namespace ProductCatalog.Infrastructure.Infrastructure.Repositories.Boilerplate
{
    internal abstract class TranslationRepository<TDbContext, TEntity, TTranslationResult> : TemporalReadWriteRepository<TDbContext, TEntity>, ITranslationRepository<TEntity>
                                                                                             where TDbContext : DbContext
                                                                                             where TEntity : class, IEntityFrameworkEntity, ITranslatable<TTranslationResult>
                                                                                             where TTranslationResult : ITranslationResult
    {
        protected TranslationRepository(ref TDbContext context) : base(ref context)
        {
        }

        private static readonly string FALLBACK_LANGUAGE = "en";


        public async Task<ITranslationResult?> GetFallbackTranslation<TId>(TId id)
        {
            var entity = await _context.Set<TEntity>()
                                       .FindAsync(id);

            if (entity is null)
            {
                return null;
            }
            else
            {
                var result = entity.Translations.FirstOrDefault(t => t.Language == FALLBACK_LANGUAGE);
                return result;
            }
        }


        public void AddTranslation<TTranslation>(TTranslation translation) where TTranslation : class, ITranslationResult
        {
            _context.Set<TTranslation>()
                    .Update(translation);
        }

        public void AddTranslationRange<TTranslation>(IEnumerable<TTranslation> translation) where TTranslation : class, ITranslationResult
        {
            _context.Set<TTranslation>()
                    .UpdateRange(translation);
        }


        new public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "", int? pageIndex = null, int pageSize = 50, bool asNoTracking = false, CancellationToken cancellationToken = default)
        {
            var query = QueryBuilder(filter, orderBy, includeProperties, pageIndex, pageSize, asNoTracking);
            query = query.Include(e => e.Translations);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAsync(string language, Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "", int? pageIndex = null, int pageSize = 50, bool asNoTracking = false, CancellationToken cancellationToken = default)
        {
            var query = QueryBuilder(filter, orderBy, includeProperties, pageIndex, pageSize, asNoTracking);
            query = query.Include(e => e.Translations.Where(t => t.Language == language));

            return await query.ToListAsync(cancellationToken);
        }


        public async Task<IEnumerable<TEntity>> GetAsync(IEnumerable<string> languages, Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "", int? pageIndex = null, int pageSize = 50, bool asNoTracking = false, CancellationToken cancellationToken = default)
        {
            var query = QueryBuilder(filter, orderBy, includeProperties, pageIndex, pageSize, asNoTracking);
            query = query.Include(e => e.Translations.Where(t => languages.Contains(t.Language)));

            return await query.ToListAsync(cancellationToken);
        }

    }
}
