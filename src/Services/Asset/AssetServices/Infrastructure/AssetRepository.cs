using AssetServices.Models;
using Common.Enums;
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
            return await _assetContext.Assets
                .Include(a => a.AssetCategory)
                .ThenInclude(c => c.Translations)
                .Include(a => a.AssetLabels.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.Label)
                .FirstOrDefaultAsync(a => a.ExternalId == asset.ExternalId);
        }

        public async Task<IList<CustomerAssetCount>> GetAssetsCountsAsync()
        {
            var assetCountList = await _assetContext.Assets
            .Where(a => a.Status == AssetStatus.Active)
            .GroupBy(a => a.CustomerId)
            .Select(group => new CustomerAssetCount(){
                OrganizationId = group.Key,
                Count = group.Count()
            })
            .ToListAsync();

            return assetCountList;
        }

        public async Task<int> GetAssetsCountAsync(Guid customerId)
        {
            var assets = await _assetContext.Assets
            .Where(a => a.CustomerId == customerId && a.Status == AssetStatus.Active).CountAsync();

            return assets;
        }

        public async Task<PagedModel<Asset>> GetAssetsAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(search))
            {
                var temp = await _assetContext.HardwareAsset
                      .Include(a => a.AssetCategory)
                      .ThenInclude(c => c.Translations)
                      .Include(a => a.Imeis)
                      .Include(a => a.AssetLabels.Where(a => !a.IsDeleted))
                      .ThenInclude(a => a.Label)
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
                    .Include(a => a.AssetLabels.Where(a => !a.IsDeleted))
                    .ThenInclude(a => a.Label)
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

        public async Task<IList<Asset>> GetAssetsFromListAsync(Guid customerId, IList<Guid> assetGuidList)
        {
            if (assetGuidList.Any())
            {
                var temp = await _assetContext.HardwareAsset
                    .Include(a => a.AssetCategory)
                    .ThenInclude(c => c.Translations)
                    .Include(b => b.AssetLabels.Where(c => (!c.IsDeleted)))
                    .ThenInclude(b => b.Label)
                    .Where(a => (a.CustomerId == customerId && assetGuidList.Contains(a.ExternalId))).ToListAsync();

                IList<Asset> result = new List<Asset>();
                foreach (var asset in temp)
                {
                    // should not be necessary...
                    foreach (AssetLabel al in asset.AssetLabels)
                    {
                        if (al.IsDeleted || al.Label.IsDeleted)
                        {
                            asset.AssetLabels.Remove(al);
                        }
                    }
                    result.Add(asset);
                }
                return result;
            }
            return null;
        }

        public async Task<IList<CustomerLabel>> AddCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels)
        {
            _assetContext.CustomerLabels.AddRange(labels);
            await SaveEntitiesAsync();
            return await _assetContext.CustomerLabels
                         .Where(c => c.CustomerId == customerId && !c.IsDeleted).ToListAsync();
        }

        public async Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId)
        {
            return await _assetContext.CustomerLabels
                         .Where(a => a.CustomerId == customerId && !a.IsDeleted).ToListAsync();
        }

        public async Task<IList<CustomerLabel>> GetCustomerLabelsFromListAsync(IList<Guid> labelsGuid)
        {
            return await _assetContext.CustomerLabels
                         .Where(a => labelsGuid.Contains<Guid>(a.ExternalId)).ToListAsync();
        }

        public async Task<IList<AssetLabel>> GetAssetLabelsFromListAsync(IList<int> labelInts)
        {
            return await _assetContext.AssetLabels
                         .Where(a => labelInts.Contains<int>(a.LabelId)).ToListAsync();
        }

        public async Task<CustomerLabel> GetCustomerLabelAsync(Guid labelGuid)
        {
            return await _assetContext.CustomerLabels
                         .Where(a => labelGuid == a.ExternalId).FirstOrDefaultAsync();
        }

        public async Task<IList<CustomerLabel>> DeleteCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels)
        {

            _assetContext.CustomerLabels.RemoveRange(labels);
            await SaveEntitiesAsync();
            return await GetCustomerLabelsForCustomerAsync(customerId);
        }

        public async Task<IList<CustomerLabel>> UpdateCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels)
        {
            foreach (CustomerLabel updateLabel in labels)
            {
                CustomerLabel original = await GetCustomerLabelAsync(updateLabel.ExternalId);
                if (original != null)
                {
                    original.PatchLabel(updateLabel.UpdatedBy, updateLabel.Label);
                }
            }

            await SaveEntitiesAsync();
            return await GetCustomerLabelsForCustomerAsync(customerId);
        }

        public async Task<AssetLabel> GetAssetLabelForAssetAsync(int assetId, int labelId)
        {
            return await _assetContext.AssetLabels.Where(a => a.AssetId == assetId && a.LabelId == labelId).FirstOrDefaultAsync();
        }

        public async Task AddAssetLabelsForAsset(IList<AssetLabel> labels)
        {
            await _assetContext.AssetLabels.AddRangeAsync(labels);
            await SaveEntitiesAsync();
        }

        public async Task DeleteLabelsFromAssetLabels(IList<int> labelIds)
        {
            var labels = await _assetContext.AssetLabels.Where(a => labelIds.Contains(a.LabelId)).ToListAsync();
            _assetContext.AssetLabels.RemoveRange(labels);
            await SaveEntitiesAsync();
        }

        public async Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            var temp = await _assetContext.HardwareAsset
                .Include(a => a.AssetCategory)
                .ThenInclude(c => c.Translations)
                .Include(a => a.Imeis)
                .Include(a => a.AssetLabels.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.Label)
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
            var temp = await _assetContext.HardwareAsset
                .Include(a => a.AssetCategory)
                .ThenInclude(c => c.Translations)
                .Include(a => a.Imeis)
                .Include(a => a.AssetLabels.Where(b => !b.IsDeleted))
                .ThenInclude(a => a.Label)
                .Where(a => a.CustomerId == customerId && a.ExternalId == assetId)
                .FirstOrDefaultAsync();
            return temp;
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
                var EditedEntities = _assetContext.ChangeTracker.Entries()
                                        .Where(E => E.State == EntityState.Modified)
                                        .ToList();

                EditedEntities.ForEach(E =>
                {
                    E.Property("LastUpdatedDate").CurrentValue = DateTime.UtcNow;
                });
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