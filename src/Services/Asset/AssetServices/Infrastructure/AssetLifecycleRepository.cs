﻿using AssetServices.Models;
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
    public class AssetLifecycleRepository : IAssetLifecycleRepository
    {
        private readonly AssetsContext _assetContext;
        private readonly IFunctionalEventLogService _functionalEventLogService;
        private readonly IMediator _mediator;

        public AssetLifecycleRepository(AssetsContext assetContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator)
        {
            _assetContext = assetContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
        }

        public async Task<AssetLifecycle> AddAsync(AssetLifecycle assetLifecycle)
        {
            if (assetLifecycle.Asset == null)
            {
                throw new Exception();
            }
            _assetContext.Assets.Add(assetLifecycle.Asset);
            _assetContext.AssetLifeCycles.Add(assetLifecycle);
            await SaveEntitiesAsync();
            var savedAssetLifecycle = await _assetContext.AssetLifeCycles
                .FirstOrDefaultAsync(a => a.ExternalId == assetLifecycle.ExternalId);
            if (savedAssetLifecycle == null)
            {
                throw new Exception();
            }
            return savedAssetLifecycle;
        }

        public async Task<IList<CustomerAssetCount>> GetAssetLifecyclesCountsAsync()
        {
            var assetCountList = await _assetContext.AssetLifeCycles
            .Where(a => a.AssetLifecycleStatus == AssetLifecycleStatus.Active)
            .GroupBy(a => a.CustomerId)
            .Select(group => new CustomerAssetCount()
            {
                OrganizationId = group.Key,
                Count = group.Count()
            })
            .ToListAsync();

            return assetCountList;
        }

        public async Task<int> GetAssetLifecyclesCountAsync(Guid customerId)
        {
            return await _assetContext.AssetLifeCycles
                .Where(a => a.CustomerId == customerId && a.AssetLifecycleStatus == AssetLifecycleStatus.Active).CountAsync();
        }

        public async Task<decimal> GetCustomerTotalBookValue(Guid customerId)
        {
            var assets = await _assetContext.AssetLifeCycles
                .Where(a => a.CustomerId == customerId && a.AssetLifecycleStatus == AssetLifecycleStatus.Active).ToListAsync();
            return assets.Sum(x=>x.BookValue);
        }

        public async Task<PagedModel<AssetLifecycle>> GetAssetLifecyclesAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken)
        {
            var assets = await _assetContext.AssetLifeCycles.Include(al => al.Asset)
                    .ThenInclude(hw => (hw as MobilePhone).Imeis)
                    //.ThenInclude(hw => (hw as Tablet).Imeis)
                    .Include(al => al.ContractHolderUser)
                    .Where(al => al.CustomerId == customerId).ToListAsync();

            if (long.TryParse(search, out long imei))
            {
                assets = assets.Where(a => a.Asset!.Brand.ToLower().Contains(search.ToLower())
                || a.Asset.ProductName.ToLower().Contains(search.ToLower())
                || a.ContractHolderUser!.Name.ToLower().Contains(search.ToLower())
                || (a.Asset as HardwareAsset)!.SerialNumber.ToLower().Contains(search.ToLower())
                || (a.Asset as MobilePhone is not null && (a.Asset as MobilePhone).Imeis.Any(im => im.Imei == imei))).ToList();

                return assets.AsEnumerable().PaginateAsync(page, limit);
            }

            if (!string.IsNullOrEmpty(search))
                assets = assets.Where(a => a.Asset!.Brand.ToLower().Contains(search.ToLower())
                || a.Asset.ProductName.ToLower().Contains(search.ToLower())
                || a.ContractHolderUser!.Name.ToLower().Contains(search.ToLower())
                || (a.Asset as HardwareAsset)!.SerialNumber.ToLower().Contains(search.ToLower())).ToList();

            return assets.AsEnumerable().PaginateAsync(page, limit);
        }

        public async Task<IList<AssetLifecycle>> GetAssetLifecyclesFromListAsync(Guid customerId, IList<Guid> assetGuidList)
        {
            return await _assetContext.AssetLifeCycles.Include(al => al.Asset)
                .Include(al => al.Labels)
                .Where(al => assetGuidList.Contains(al.ExternalId)).ToListAsync();
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

        public async Task<IList<AssetLifecycle>> GetAssetLifecyclesForUserAsync(Guid customerId, Guid userId)
        {
            return await _assetContext.AssetLifeCycles
                .Include(a => a.ContractHolderUser)
                .Where(a => a.CustomerId == customerId && a.ContractHolderUser!.ExternalId == userId)
                .AsTracking()
                .ToListAsync();
        }

        public async Task UnAssignAssetLifecyclesForUserAsync(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            var assetLifecyclesForUser = _assetContext.AssetLifeCycles
                .Include(a => a.ContractHolderUser)
                .Where(a => a.CustomerId == customerId && a.ContractHolderUser!.ExternalId == userId);

            if (!assetLifecyclesForUser.Any()) return;

            foreach (var assetLifecycle in assetLifecyclesForUser)
            {
                assetLifecycle.UnAssignContractHolder(callerId);

                if (assetLifecycle.ManagedByDepartmentId == null)
                    assetLifecycle.AssignDepartment(departmentId, callerId);

                _assetContext.Entry(assetLifecycle).State = EntityState.Modified;
            }

            await SaveEntitiesAsync();
        }

        public async Task<AssetLifecycle?> GetAssetLifecycleAsync(Guid customerId, Guid assetLifecycleId)
        {
            var assetLifecycle = await _assetContext.AssetLifeCycles
                .Include(al => al.Asset)
                .ThenInclude(hw => (hw as MobilePhone).Imeis)
                //.ThenInclude(hw => (hw as Tablet).Imeis)
                .Include(al => al.ContractHolderUser)
                .Where(a => a.CustomerId == customerId && a.ExternalId == assetLifecycleId)
                .FirstOrDefaultAsync();
            return assetLifecycle;
        }

        public async Task<User?> GetUser(Guid userId)
        {
            return await _assetContext.Users.FirstOrDefaultAsync(u => u.ExternalId == userId);
        }

        public async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            int numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_assetContext).ExecuteAsync(async () =>
            {
                var editedEntities = _assetContext.ChangeTracker.Entries()
                                        .Where(E => E.State == EntityState.Modified)
                                        .ToList();

                editedEntities.ForEach(entity =>
                {
                    entity.Property("LastUpdatedDate").CurrentValue = DateTime.UtcNow;
                });
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _assetContext.SaveChangesAsync(cancellationToken);
                if (!_assetContext.IsSQLite)
                {
                    foreach (var @event in _assetContext.GetDomainEventsAsync())
                    {
                        await _functionalEventLogService.SaveEventAsync(@event,
                            _assetContext.Database.CurrentTransaction!);
                    }
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