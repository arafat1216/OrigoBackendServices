using AssetServices.Models;
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
using System.Collections.Generic;

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
            return await _assetContext.Assets
                .Include(a => a.AssetCategory)
                .ThenInclude(c => c.Translations)
                .FirstOrDefaultAsync(a => a.ExternalId == asset.ExternalId);
        }

        public async Task<PagedModel<Asset>> GetAssetsAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(search))
            {
                var temp = await _assetContext.HardwareAsset
                      .Include(a => a.AssetCategory)
                      .ThenInclude(c => c.Translations)
                      .Include(a => a.Imeis)
                      .Where(a => a.CustomerId == customerId)
                      .PaginateAsync(page, limit, cancellationToken);

                IList<Asset> result = new List<Asset>();
                foreach (var asset in temp.Items)
                {
                    result.Add(asset);
                }
                PagedModel<Asset> assets = new PagedModel<Asset>()
                {
                    CurrentPage = temp.CurrentPage,
                    PageSize = temp.PageSize,
                    TotalItems = temp.TotalItems,
                    TotalPages = temp.TotalPages,
                    Items = result
                };
                return assets;
            }
            else
            {
                var temp = await _assetContext.HardwareAsset
                    .Include(a => a.AssetCategory)
                    .ThenInclude(c => c.Translations)
                    .Include(a => a.Imeis)
                    .Where(a => a.CustomerId == customerId && a.Brand.Contains(search))
                    .PaginateAsync(page, limit, cancellationToken);

                IList<Asset> result = new List<Asset>();
                foreach (var asset in temp.Items)
                {
                    result.Add(asset);
                }
                PagedModel<Asset> assets = new PagedModel<Asset>()
                {
                    CurrentPage = temp.CurrentPage,
                    PageSize = temp.PageSize,
                    TotalItems = temp.TotalItems,
                    TotalPages = temp.TotalPages,
                    Items = result
                };

                return assets;
            }
        }

        public async Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            var temp = await _assetContext.HardwareAsset
                .Include(a => a.AssetCategory)
                .ThenInclude(c => c.Translations)
                .Include(a => a.Imeis)
                .Where(a => a.CustomerId == customerId && a.AssetHolderId == userId)
                .AsNoTracking()
                .ToListAsync();

            IList<Asset> result = new List<Asset>();
            foreach (var asset in temp)
            {
                result.Add(asset);
            }
            return result;
        }

        public async Task<Asset> GetAssetAsync(Guid customerId, Guid assetId)
        {
            return await _assetContext.HardwareAsset
                .Include(a => a.AssetCategory)
                .ThenInclude(c => c.Translations)
                .Include(a => a.Imeis)
                .Where(a => a.CustomerId == customerId && a.ExternalId == assetId)
                .FirstOrDefaultAsync();
        }

        public async Task<AssetCategory> GetAssetCategoryAsync(int assetAssetCategoryId)
        {
            return await _assetContext.AssetCategories.Where(c => c.Id == assetAssetCategoryId)
                .Include(c => c.Translations)
                .FirstOrDefaultAsync();
        }

        public async Task<IList<AssetCategory>> GetAssetCategoriesAsync(string language = "EN")
        {
            return await _assetContext.AssetCategories
                .Include(a => a.Translations.Where(t => t.Language == language))
                .ToListAsync();
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