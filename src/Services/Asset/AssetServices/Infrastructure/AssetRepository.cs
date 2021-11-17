﻿using AssetServices.Models;
using Common.Extensions;
using Common.Interfaces;
using Common.Logging;
using Common.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AssetServices.Infrastructure
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AssetsContext _assetContext;
        private readonly IFunctionalEventLogService _functionalEventLogService;
        private readonly IMediator _mediator;

        public AssetRepository(AssetsContext assetContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator)
        {
            _assetContext = assetContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
        }

        public async Task<Asset> AddAsync(Asset asset)
        {
            _assetContext.Assets.Add(asset);
            await SaveEntitiesAsync();
            return await _assetContext.Assets.Include(a => a.AssetCategory)
                .FirstOrDefaultAsync(a => a.AssetId == asset.AssetId);
        }

        public async Task<PagedModel<Asset>> GetAssetsAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await _assetContext.Assets
                    .Include(a => a.AssetCategory)
                    .Where(a => a.CustomerId == customerId)
                    .PaginateAsync(page, limit, cancellationToken);
            }
            else
            {
                return await _assetContext.Assets
                    .Include(a => a.AssetCategory)
                    .Where(a => a.CustomerId == customerId && a.Brand.Contains(search))
                    .PaginateAsync(page, limit, cancellationToken);
            }
        }

        public async Task<IList<Asset>> GetAssetsFromListAsync(Guid customerId, IList<Guid> assetGuidList)
        {
            if (assetGuidList.Any())
            {
                return await _assetContext.Assets
                    .Include(a => a.AssetCategory)
                    .Where(a => (a.CustomerId == customerId &&  assetGuidList.Contains(a.AssetId))).ToListAsync();
            }
            return null;
        }

        public async Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            return await _assetContext.Assets.Include(a => a.AssetCategory)
                .Where(a => a.CustomerId == customerId && a.AssetHolderId == userId).AsNoTracking().ToListAsync();
        }

        public async Task<Asset> GetAssetAsync(Guid customerId, Guid assetId)
        {
            return await _assetContext.Assets.Include(a => a.AssetCategory)
                .Where(a => a.CustomerId == customerId && a.AssetId == assetId).FirstOrDefaultAsync();
        }

        public async Task<AssetCategory> GetAssetCategoryAsync(Guid assetAssetCategoryId)
        {
            return await _assetContext.AssetCategories.Where(c => c.AssetCategoryId == assetAssetCategoryId)
                .FirstOrDefaultAsync();
        }

        public async Task<IList<AssetCategory>> GetAssetCategoriesAsync()
        {
            return await _assetContext.AssetCategories.ToListAsync();
        }

        // TODO: Should be removed and all reference replaced with SaveEntitiesAsync.
        public async Task SaveChanges()
        {
            await _assetContext.SaveChangesAsync();
        }

        public async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            int numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_assetContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _assetContext.SaveChangesAsync(cancellationToken);
                foreach (var @event in _assetContext.GetDomainEventsAsync())
                {
                    await _functionalEventLogService.SaveEventAsync(@event, _assetContext.Database.CurrentTransaction);
                }
                numberOfRecordsSaved = await _assetContext.SaveChangesAsync(cancellationToken);
                await _mediator.DispatchDomainEventsAsync(_assetContext);
            });
            return numberOfRecordsSaved;
        }

        public async Task<IList<FunctionalEventLogEntry>> GetAuditLog(Guid assetId)
        {
            return await _functionalEventLogService.RetrieveEventLogsAsync(assetId);
        }
    }
}