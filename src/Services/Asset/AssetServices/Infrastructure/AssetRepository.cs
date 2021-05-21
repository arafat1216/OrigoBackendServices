using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetServices.Models;
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

        public async Task<IList<Asset>> GetAssetsAsync(Guid customerId)
        {
            return await _context.Assets.Where(a => a.CustomerId == customerId).ToListAsync();
        }

        public async Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            return await _context.Assets.Where(a => a.CustomerId == customerId && a.AssetHolderId == userId).ToListAsync();
        }

        public async Task<Asset> GetAssetAsync(Guid customerId, Guid assetId)
        {
            return await _context.Assets.Where(a => a.CustomerId == customerId && a.AssetId == assetId).FirstOrDefaultAsync();
        }

        public async Task<AssetCategory> GetAssetCategoryAsync(Guid assetAssetCategoryId)
        {
            return await _context.AssetCategories.Where(c => c.AssetCategoryId == assetAssetCategoryId).FirstOrDefaultAsync();
        }
    }
}