using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.Models;
using Common.Extensions;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetServices.Infrastructure
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AssetsContext _context;

        public AssetRepository(AssetsContext context)
        {
            _context = context;
        }

        public async Task<Asset> AddAsync(Asset asset)
        {
            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();
            return await _context.Assets.Include(a => a.AssetCategory)
                .FirstOrDefaultAsync(a => a.AssetId == asset.AssetId);
        }

        public async Task<PagedModel<Asset>> GetAssetsAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await _context.Assets
                    .Include(a => a.AssetCategory)
                    .Where(a => a.CustomerId == customerId)
                    .PaginateAsync(page, limit, cancellationToken);
            }
            else
            {
                return await _context.Assets
                    .Include(a => a.AssetCategory)
                    .Where(a => a.CustomerId == customerId && a.Brand.Contains(search))
                    .PaginateAsync(page, limit, cancellationToken);
            }
        }

        public async Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            return await _context.Assets.Include(a => a.AssetCategory)
                .Where(a => a.CustomerId == customerId && a.AssetHolderId == userId).AsNoTracking().ToListAsync();
        }

        public async Task<Asset> GetAssetAsync(Guid customerId, Guid assetId)
        {
            return await _context.Assets.Include(a => a.AssetCategory)
                .Where(a => a.CustomerId == customerId && a.AssetId == assetId).FirstOrDefaultAsync();
        }

        public async Task<AssetCategory> GetAssetCategoryAsync(Guid assetAssetCategoryId)
        {
            return await _context.AssetCategories.Where(c => c.AssetCategoryId == assetAssetCategoryId)
                .FirstOrDefaultAsync();
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}