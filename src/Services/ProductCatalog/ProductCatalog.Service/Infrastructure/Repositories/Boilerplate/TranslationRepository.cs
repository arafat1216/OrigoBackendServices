using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Service.Models.Database.Interfaces;
using System.Linq.Expressions;

namespace ProductCatalog.Service.Infrastructure.Repositories.Boilerplate
{
    internal abstract class TranslationRepository<TEntity, TTranslationResult, TDbContext> : Repository<TEntity, TDbContext>, ITranslationRepository<TEntity>
            where TEntity : class, IEntityFrameworkEntity, ITranslatable<TTranslationResult>
            where TTranslationResult : ITranslationResult
            where TDbContext : DbContext
    {
        protected TranslationRepository(TDbContext context) : base(context)
        {
        }

        private static readonly string FALLBACK_LANGUAGE = "en";


        /// <inheritdoc/>
        new public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking, CancellationToken cancellationToken = default)
        {
            if (asNoTracking)
            {
                return await _context.Set<TEntity>()
                                     .Where(predicate)
                                     .Include(e => e.Translations)
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
            }
            else
            {
                return await _context.Set<TEntity>()
                                     .Where(predicate)
                                     .Include(e => e.Translations)
                                     .ToListAsync(cancellationToken);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> FindAsync(string language, Expression<Func<TEntity, bool>> predicate, bool asNoTracking, CancellationToken cancellationToken = default)
        {
            if (asNoTracking)
            {
                return await _context.Set<TEntity>()
                                     .Where(predicate)
                                     // Include the first language result that matches the parameter
                                     .Include(e => e.Translations
                                                                       .Where(t => t.Language == language)
                                                                       .Take(1))
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
            }
            else
            {
                return await _context.Set<TEntity>()
                                     .Where(predicate)
                                     // Include the first language result that matches the parameter
                                     .Include(e => e.Translations
                                                                       .Where(t => t.Language == language)
                                                                       .Take(1))
                                     .ToListAsync(cancellationToken);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> FindAsync(IEnumerable<string> languages, Expression<Func<TEntity, bool>> predicate, bool asNoTracking, CancellationToken cancellationToken = default)
        {
            if (asNoTracking)
            {
                return await _context.Set<TEntity>()
                                     .Where(predicate)
                                     // Include all languages found from the parameter list
                                     .Include(e => e.Translations
                                                                       .Where(t => languages.Contains(t.Language)))
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
            }
            else
            {
                return await _context.Set<TEntity>()
                                     .Where(predicate)
                                     // Include all languages found from the parameter list
                                     .Include(e => e.Translations
                                                                       .Where(t => languages.Contains(t.Language)))
                                     .ToListAsync(cancellationToken);
            }
        }



        /// <inheritdoc/>
        new public IAsyncEnumerable<TEntity> FindAsStream(Expression<Func<TEntity, bool>> predicate, bool asNoTracking)
        {
            if (asNoTracking)
            {
                return _context.Set<TEntity>()
                               .Where(predicate)
                               .Include(e => e.Translations)
                               .AsNoTracking()
                               .AsAsyncEnumerable();
            }
            else
            {
                return _context.Set<TEntity>()
                               .Where(predicate)
                               .Include(e => e.Translations)
                               .AsAsyncEnumerable();
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<TEntity> FindAsStream(string language, Expression<Func<TEntity, bool>> predicate, bool asNoTracking)
        {
            if (asNoTracking)
            {
                return _context.Set<TEntity>()
                               .Where(predicate)
                               // Include the first language result that matches the parameter
                               .Include(e => e.Translations
                                                                 .Where(t => t.Language == language)
                                                                 .Take(1))
                               .AsNoTracking()
                               .AsAsyncEnumerable();
            }
            else
            {
                return _context.Set<TEntity>()
                               .Where(predicate)
                               // Include the first language result that matches the parameter
                               .Include(e => e.Translations
                                                                 .Where(t => t.Language == language)
                                                                 .Take(1))
                               .AsAsyncEnumerable();
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<TEntity> FindAsStream(IEnumerable<string> languages, Expression<Func<TEntity, bool>> predicate, bool asNoTracking)
        {
            if (asNoTracking)
            {
                return _context.Set<TEntity>()
                               .Where(predicate)
                               // Include all languages found from the parameter list
                               .Include(e => e.Translations
                                                                 .Where(t => languages.Contains(t.Language)))
                               .AsNoTracking()
                               .AsAsyncEnumerable();
            }
            else
            {
                return _context.Set<TEntity>()
                               .Where(predicate)
                               // Include all languages found from the parameter list
                               .Include(e => e.Translations
                                                                 .Where(t => languages.Contains(t.Language)))
                               .AsAsyncEnumerable();
            }
        }



        /// <inheritdoc/>
        new public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking, CancellationToken cancellationToken = default)
        {
            if (asNoTracking)
            {
                return await _context.Set<TEntity>()
                                     .Include(e => e.Translations)
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
            }
            else
            {
                return await _context.Set<TEntity>()
                                     .Include(e => e.Translations)
                                     .ToListAsync(cancellationToken);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(string language, bool asNoTracking, CancellationToken cancellationToken = default)
        {
            if (asNoTracking)
            {
                return await _context.Set<TEntity>()
                                     // Include the first language result that matches the parameter
                                     .Include(e => e.Translations
                                                                       .Where(t => t.Language == language)
                                                                       .Take(1))
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
            }
            else
            {
                return await _context.Set<TEntity>()
                                     // Include the first language result that matches the parameter
                                     .Include(e => e.Translations
                                                                       .Where(t => t.Language == language)
                                                                       .Take(1))
                                     .ToListAsync(cancellationToken);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<string> languages, bool asNoTracking, CancellationToken cancellationToken = default)
        {
            if (asNoTracking)
            {
                return await _context.Set<TEntity>()
                                     // Include all languages found from the parameter list
                                     .Include(e => e.Translations
                                                                       .Where(t => languages.Contains(t.Language)))
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
            }
            else
            {
                return await _context.Set<TEntity>()
                                     // Include all languages found from the parameter list
                                     .Include(e => e.Translations
                                                                       .Where(t => languages.Contains(t.Language)))
                                     .ToListAsync(cancellationToken);
            }
        }



        /// <inheritdoc/>
        new public IAsyncEnumerable<TEntity> GetAllAsStream(bool asNoTracking)
        {
            if (asNoTracking)
            {
                return _context.Set<TEntity>()
                               .Include(e => e.Translations)
                               .AsNoTracking()
                               .AsAsyncEnumerable();
            }
            else
            {
                return _context.Set<TEntity>()
                               .Include(e => e.Translations)
                               .AsAsyncEnumerable();
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<TEntity> GetAllAsStream(string language, bool asNoTracking)
        {
            if (asNoTracking)
            {
                return _context.Set<TEntity>()
                               // Include the first language result that matches the parameter
                               .Include(e => e.Translations
                                                                 .Where(t => t.Language == language)
                                                                 .Take(1))
                               .AsNoTracking()
                               .AsAsyncEnumerable();
            }
            else
            {
                return _context.Set<TEntity>()
                               // Include the first language result that matches the parameter
                               .Include(e => e.Translations
                                                                 .Where(t => t.Language == language)
                                                                 .Take(1))
                               .AsAsyncEnumerable();
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<TEntity> GetAllAsStream(IEnumerable<string> languages, bool asNoTracking)
        {
            if (asNoTracking)
            {
                return _context.Set<TEntity>()
                               // Include all languages found from the parameter list
                               .Include(e => e.Translations
                                                                 .Where(t => languages.Contains(t.Language)))
                               .AsNoTracking()
                               .AsAsyncEnumerable();
            }
            else
            {
                return _context.Set<TEntity>()
                               // Include all languages found from the parameter list
                               .Include(e => e.Translations
                                                                 .Where(t => languages.Contains(t.Language)))
                               .AsAsyncEnumerable();
            }
        }

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

        public async Task AddTranslationAsync<TTranslation>(TTranslation translation) where TTranslation : class, IEntityFrameworkEntity, ITranslationResult
        {
            await _context.Set<TTranslation>()
                          .AddAsync(translation);
        }

        public async Task AddTranslationRangeAsync<TTranslation>(IEnumerable<TTranslation> translation) where TTranslation : class, IEntityFrameworkEntity, ITranslationResult
        {
            await _context.Set<TTranslation>()
                          .AddRangeAsync(translation);
        }
    }
}
