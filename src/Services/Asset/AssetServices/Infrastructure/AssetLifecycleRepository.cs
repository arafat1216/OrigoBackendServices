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
using Common.Seedwork;

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
            .GroupBy(a => a.CustomerId)
            .Select(group => new CustomerAssetCount()
            {
                OrganizationId = group.Key,
                Count = group.Count()
            })
            .ToListAsync();

            return assetCountList;
        }

        public async Task<int> GetAssetLifecyclesCountAsync(Guid customerId, Guid? departmentId, AssetLifecycleStatus? assetLifecycleStatus)
        {
            if (departmentId != null && departmentId != Guid.Empty)
            {
                var countStatus = await _assetContext.AssetLifeCycles
                    .Where(a => a.CustomerId == customerId && a.ManagedByDepartmentId == departmentId)
                    .GroupBy(a => a.AssetLifecycleStatus)
                    .Select(c => new { StatusId = c.Key, Count = c.Count() })
                    .ToListAsync();
                return assetLifecycleStatus.HasValue
                    ? countStatus.FirstOrDefault(c => c.StatusId == assetLifecycleStatus)?.Count ?? 0
                    : countStatus.Sum(c => AssetLifecycle.HasActiveState(c.StatusId) ? c.Count : 0);
            }
            else
            {
                var countStatus = await _assetContext.AssetLifeCycles
                    .Where(a => a.CustomerId == customerId)
                    .GroupBy(a => a.AssetLifecycleStatus)
                    .Select(c => new { StatusId = c.Key, Count = c.Count() })
                    .ToListAsync();

                return assetLifecycleStatus.HasValue
                    ? countStatus.FirstOrDefault(c => c.StatusId == assetLifecycleStatus)?.Count ?? 0
                    : countStatus.Sum(c => AssetLifecycle.HasActiveState(c.StatusId) ? c.Count : 0);
            }
        }

        public async Task<decimal> GetCustomerTotalBookValue(Guid customerId)
        {
            var assets = await _assetContext.AssetLifeCycles
                .Where(a => a.CustomerId == customerId && a.AssetLifecycleType == LifecycleType.Transactional &&
                            (a.AssetLifecycleStatus == AssetLifecycleStatus.InUse ||
                             a.AssetLifecycleStatus == AssetLifecycleStatus.InputRequired ||
                             a.AssetLifecycleStatus == AssetLifecycleStatus.Repair ||
                             a.AssetLifecycleStatus == AssetLifecycleStatus.PendingReturn ||
                             a.AssetLifecycleStatus == AssetLifecycleStatus.Available)
                ).ToListAsync();
            return assets.Sum(x => x.BookValue);
        }

        public async Task<PagedModel<AssetLifecycle>> GetAssetLifecyclesAsync(Guid customerId, string? userId, IList<AssetLifecycleStatus>? status, IList<Guid?>? department, int[]? category,
           Guid[]? label, bool? isActiveState, bool? isPersonal, DateTime? endPeriodMonth, string search, int page, int limit, CancellationToken cancellationToken)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();
            query = query.Include(al => al.Asset).ThenInclude(mp => (mp as MobilePhone).Imeis);
            query = query.Include(al => al.Labels);
            query = query.Include(al => al.ContractHolderUser);
            query = query.Where(al => al.CustomerId == customerId);
            if (!string.IsNullOrEmpty(search))
            {
                var imeiFound = false;
                long imei = 0;
                if (search.Length >= 15 && long.TryParse(search, out imei))
                {
                    imeiFound = true;
                }
                query = query.Where(al => al.Asset != null &&
                                          (
                                              al.Asset.Brand.ToLower().Contains(search.ToLower()) ||
                                              al.Asset.ProductName.ToLower().Contains(search.ToLower())) ||
                                              (imeiFound && al.Asset is MobilePhone && (al.Asset as MobilePhone).Imeis.Any(im => im.Imei == imei)) ||
                                              (al.Asset is MobilePhone && (al.Asset as MobilePhone).SerialNumber.ToLower().Contains(search.ToLower()))
                );
            }



            if (status != null)
            {
                query = query.Where(al => status.Contains(al.AssetLifecycleStatus));
            }
            if (department != null)
            {
                query = query.Where(al => department.Contains(al.ManagedByDepartmentId));
            }

            if (category is { Length: 1 })
            {
                if(category[0] == 1)
                    query = query.Where(al => al.Asset is MobilePhone);
                else if(category[0] == 2)
                    query = query.Where(al => al.Asset is Tablet);

                //query = query.Where(al => category.Contains(al.AssetCategoryId));
            }
            else if(category is { Length: 2 })
            {
                query = query.Where(al => al.Asset is MobilePhone || al.Asset is Tablet);
            }

            if (label != null)
            {
                query = query.Where(al => al.Labels.Any(e => label.Contains(e.ExternalId)) );
            }

            if(userId != null)
            {
                query = query.Where(al => al.ContractHolderUser.ExternalId == new Guid(userId));
            }
            if (isPersonal.HasValue)
            {
                query = query.Where(al => al.IsPersonal == isPersonal.Value); 
            }
            if (isActiveState.HasValue)
            {
               if(isActiveState.Value) query = query.Where(al => al.AssetLifecycleStatus == AssetLifecycleStatus.InputRequired ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.InUse ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.PendingReturn ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Repair ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Available ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Active);
               
                else query = query.Where(al => al.AssetLifecycleStatus == AssetLifecycleStatus.Lost ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Stolen ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.BoughtByUser ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Recycled ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Discarded ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Returned);
            
            }
            if (endPeriodMonth.HasValue)
            {
                query = query.Where(al => al.EndPeriod.HasValue && al.EndPeriod.Value.Month == endPeriodMonth.Value.Month && al.EndPeriod.Value.Year == endPeriodMonth.Value.Year);
            }

            query = query.AsSplitQuery().AsNoTracking();
            return await query.PaginateAsync(page, limit, cancellationToken);
        }
        public async Task<ServiceModel.CustomerAssetsCounterDTO> GetAssetLifecycleCountForCustomerAsync(Guid customerId,Guid? userId, IList<AssetLifecycleStatus> statuses)
        {
            ServiceModel.CustomerAssetsCounterDTO customerAssetsCounter = new ServiceModel.CustomerAssetsCounterDTO();
            ServiceModel.AssetCounter personal = new ServiceModel.AssetCounter();
            ServiceModel.AssetCounter nonPersonal = new ServiceModel.AssetCounter();

            var groups = await _assetContext.AssetLifeCycles.Where(a => a.CustomerId == customerId).GroupBy(c => new
            {
                c.AssetLifecycleStatus,
                c.IsPersonal
            }) .Select(group => new
                   {
                       AssetLifecycle = group.Key.AssetLifecycleStatus,
                       value = group.Count(),
                       isPersonal = group.Key.IsPersonal
                   })
                   .ToListAsync();

            foreach (var g in groups) {


                if (statuses == null || (statuses.Contains(g.AssetLifecycle)))
                {
                    switch (g.AssetLifecycle)
                    {
                        case AssetLifecycleStatus.InUse:
                            if (g.isPersonal) personal.InUse = g.value;
                            else nonPersonal.InUse = g.value;
                            break;

                        case AssetLifecycleStatus.InputRequired:
                            if (g.isPersonal) personal.InputRequired = g.value;
                            else nonPersonal.InputRequired = g.value;
                            break;

                        case AssetLifecycleStatus.Active:
                            if (g.isPersonal) personal.Active = g.value;
                            else nonPersonal.Active = g.value;
                            break;

                        case AssetLifecycleStatus.Available:
                            if (g.isPersonal) personal.Available = g.value;
                            else nonPersonal.Available = g.value;
                            break;

                        default:
                            break;

                    }
                }
            }

            customerAssetsCounter.NonPersonal = nonPersonal;
            customerAssetsCounter.Personal = personal;

            customerAssetsCounter.OrganizationId = customerId;

            return customerAssetsCounter;
        }
        public async Task<ServiceModel.CustomerAssetsCounterDTO> GetAssetCountForDepartmentAsync(Guid customerId, Guid? userId, IList<AssetLifecycleStatus> statuses, IList<Guid?> departments)
        {
            ServiceModel.CustomerAssetsCounterDTO baseAssetCounter = new ServiceModel.CustomerAssetsCounterDTO();
            List<ServiceModel.AssetCounterDepartment> assetCounterDepartmentList = new List<ServiceModel.AssetCounterDepartment>();


            foreach (var department in departments)
            {
                var departmentCounter = new ServiceModel.AssetCounterDepartment();

                var groups = await _assetContext.AssetLifeCycles.Where(a => a.CustomerId == customerId && a.ManagedByDepartmentId == department).GroupBy(c => new
                {
                    c.AssetLifecycleStatus,
                    c.IsPersonal
                }).Select(group => new
                {
                    AssetLifecycle = group.Key.AssetLifecycleStatus,
                    value = group.Count(),
                    isPersonal = group.Key.IsPersonal
                })
                  .ToListAsync();

                foreach (var g in groups)
                {
                    if (statuses == null || (statuses.Contains(g.AssetLifecycle)))
                    {
                        switch (g.AssetLifecycle)
                        {
                            case AssetLifecycleStatus.InUse:
                                if (g.isPersonal) departmentCounter.Personal.InUse = g.value;
                                else departmentCounter.NonPersonal.InUse = g.value;
                                break;

                            case AssetLifecycleStatus.InputRequired:
                                if (g.isPersonal) departmentCounter.Personal.InputRequired = g.value;
                                else departmentCounter.NonPersonal.InputRequired = g.value;
                                break;

                            case AssetLifecycleStatus.Active:
                                if (g.isPersonal) departmentCounter.Personal.Active = g.value;
                                else departmentCounter.NonPersonal.Active = g.value;
                                break;

                            case AssetLifecycleStatus.Available:
                                if (g.isPersonal) departmentCounter.Personal.Available = g.value;
                                else departmentCounter.NonPersonal.Available = g.value;
                                break;

                            default:
                                break;

                        }
                    }
                }
                departmentCounter.DepartmentId = department;
                assetCounterDepartmentList.Add(departmentCounter);

            }

            baseAssetCounter.Departments = assetCounterDepartmentList;


            return baseAssetCounter;
        }
        public async Task<int> GetAssetLifecycleCountForUserAsync(Guid customerId, Guid? userId)
        {
            return await _assetContext.AssetLifeCycles.Where(a => a.CustomerId == customerId && a.ContractHolderUser.ExternalId == userId).CountAsync();
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
                if (assetLifecycle.ManagedByDepartmentId == null)
                    assetLifecycle.AssignAssetLifecycleHolder(null, departmentId, callerId);

                _assetContext.Entry(assetLifecycle).State = EntityState.Modified;
            }

            await SaveEntitiesAsync();
        }

        public async Task<AssetLifecycle?> MakeAssetAvailableAsync(Guid customerId, Guid callerId, Guid assetLifeCycleId)
        {
            var assetLifecycles = await _assetContext.AssetLifeCycles
                .Include(al => al.Asset)
                .ThenInclude(hw => (hw as MobilePhone).Imeis)
                //.ThenInclude(hw => (hw as Tablet).Imeis)
                .Include(al => al.ContractHolderUser)
                .Include(a => a.Labels)
                .Where(a => a.CustomerId == customerId && a.ExternalId == assetLifeCycleId)
                .FirstOrDefaultAsync();

            if (assetLifecycles != null)
            {
                assetLifecycles.MakeAssetAvailable(callerId);
                await SaveEntitiesAsync();
            }
            return assetLifecycles;
        }


        public async Task<AssetLifecycle?> GetAssetLifecycleAsync(Guid customerId, Guid assetLifecycleId)
        {
            var assetLifecycle = await _assetContext.AssetLifeCycles
                .Include(al => al.Asset)
                .ThenInclude(hw => (hw as MobilePhone).Imeis)
                //.ThenInclude(hw => (hw as Tablet).Imeis)
                .Include(al => al.ContractHolderUser)
                .Include(a => a.Labels)
                .Where(a => a.CustomerId == customerId && a.ExternalId == assetLifecycleId)
                .FirstOrDefaultAsync();
            return assetLifecycle;
        }

        public async Task<User?> GetUser(Guid userId)
        {
            return await _assetContext.Users.FirstOrDefaultAsync(u => u.ExternalId == userId);
        }
        public async Task<IList<AssetLifecycle>> GetAssetForUser(Guid userId)
        {
            return await _assetContext.AssetLifeCycles.Include(a => a.Asset).Where(u => u.ContractHolderUser.ExternalId == userId).ToListAsync();
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
                    if (!entity.Entity.GetType().IsSubclassOf(typeof(ValueObject)))
                    {
                        entity.Property("LastUpdatedDate").CurrentValue = DateTime.UtcNow;
                    }
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
        public async Task<AssetLifecycle?> GetAssetLifecycleAsync(Guid assetLifeCycleId)
        {
            var assetLifecycle = await _assetContext.AssetLifeCycles
                .Include(al => al.Asset)
                .ThenInclude(hw => (hw as MobilePhone).Imeis)
                .Where(a => a.ExternalId == assetLifeCycleId)
                .FirstOrDefaultAsync();

            return assetLifecycle;
        }

        #region LifeCycleSetting
        public async Task<CustomerSettings> AddCustomerSettingAsync(CustomerSettings customerSettings, Guid customerId)
        {
            var setting = await _assetContext.CustomerSettings.FirstOrDefaultAsync(x=>x.CustomerId == customerId);
            if(setting != null) throw new InvalidOperationException();
            await _assetContext.CustomerSettings.AddAsync(customerSettings);
            await SaveEntitiesAsync();
            return customerSettings;
        }

        public async Task<CustomerSettings?> GetCustomerSettingsAsync(Guid customerId)
        {
            return await _assetContext.CustomerSettings.Include(x => x.LifeCycleSettings).FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }
        public async Task<LifeCycleSetting?> GetCustomerLifeCycleSettingAssetCategory(Guid customerId, int assetCategoryId)
        {
            var setting = await _assetContext.CustomerSettings.Include(x => x.LifeCycleSettings).FirstOrDefaultAsync(x => x.CustomerId == customerId);
            return setting?.LifeCycleSettings.FirstOrDefault(x => x.AssetCategoryId == assetCategoryId);
        }


        #endregion

    }
}