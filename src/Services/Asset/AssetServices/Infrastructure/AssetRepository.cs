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

        public Task<Asset> AddAsync(Asset asset)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Asset>> GetAssetsAsync(Guid customerId)
        {
            return await _context.Assets.Where(a => a.CustomerId == customerId).ToListAsync();
        }

        public async Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            return await _context.Assets.Where(a => a.CustomerId == customerId && a.AssetHolderId == userId).ToListAsync();
        }

        public Task<Asset> GetAssetAsync(Guid customerId, Guid assetId)
        {
            throw new NotImplementedException();
        }

        public Task<Asset> GetAssetAsync(Guid assetId)
        {
            throw new NotImplementedException();
        }
    }
}