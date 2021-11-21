using Microsoft.EntityFrameworkCore;
using ProductCatalog.Service.Models.Database;

namespace ProductCatalog.Service.Infrastructure.Repositories
{
    internal class FeatureRepository<TDbContext> : Repository<Feature, TDbContext>, IFeatureRepository where TDbContext : DbContext
    {
        public FeatureRepository(TDbContext context) : base(context)
        {
        }


    }
}
