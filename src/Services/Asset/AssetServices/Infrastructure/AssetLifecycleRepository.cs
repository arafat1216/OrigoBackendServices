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
using AssetServices.Exceptions;

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

        public async Task<PagedModel<CustomerAssetCount>> GetAssetLifecyclesCountsAsync(IList<Guid> customerIds, int page, int limit, CancellationToken cancellationToken)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();

            if (customerIds.Any())
            {
                query = query.Where(x => customerIds.Contains(x.CustomerId));
            }
            var assetCountList = query
            .GroupBy(a => a.CustomerId)
            .Select(group => new CustomerAssetCount()
            {
                OrganizationId = group.Key,
                Count = group.Count()
            }).AsNoTracking();

            return await assetCountList.PaginateAsync(page, limit, cancellationToken);
        }
        public async Task<IList<CustomerAssetCount>> GetAssetLifecyclesCountsAsync(IList<Guid> customerIds)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();

            if (customerIds.Any())
            {
                query = query.Where(x => customerIds.Contains(x.CustomerId));
            }
            var assetCountList = await query
            .GroupBy(a => a.CustomerId)
            .Select(group => new CustomerAssetCount()
            {
                OrganizationId = group.Key,
                Count = group.Count()
            }).AsNoTracking()
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
                    .AsNoTracking()
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
                    .AsNoTracking()
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
                ).AsNoTracking().ToListAsync();
            return assets.Sum(x => x.BookValue);
        }

        public async Task<PagedModel<AssetLifecycle>> GetAssetLifecyclesAsync(Guid customerId, string? userId, IList<AssetLifecycleStatus>? status, IList<Guid?>? department, int[]? category,
           Guid[]? label, bool? isActiveState, bool? isPersonal, DateTime? endPeriodMonth, DateTime? purchaseMonth, string? search, int page, int limit, CancellationToken cancellationToken,
           bool includeAsset = false, bool includeImeis = false, bool includeLabels = false, bool includeContractHolderUser = false)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();
            if (includeAsset)
            {
                if (includeImeis)
                    query = query.Include(al => al.Asset).ThenInclude(mp => (mp as MobilePhone).Imeis);
                else
                    query = query.Include(al => al.Asset);
            }
                
            if (includeLabels)   
                query = query.Include(al => al.Labels);

            if (includeContractHolderUser)   
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
                                              (al.Asset is MobilePhone && (al.Asset as MobilePhone).SerialNumber.ToLower().Contains(search.ToLower())) ||
                                              al.ContractHolderUser.Name.ToLower().Contains(search.ToLower())
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
                if (category[0] == 1)
                    query = query.Where(al => al.Asset is MobilePhone);
                else if (category[0] == 2)
                    query = query.Where(al => al.Asset is Tablet);
            }
            else if (category is { Length: 2 })
            {
                query = query.Where(al => al.Asset is MobilePhone || al.Asset is Tablet);
            }

            if (label != null)
            {
                query = query.Where(al => al.Labels.Any(e => label.Contains(e.ExternalId)));
            }

            if (userId != null)
            {
                query = query.Where(al => al.ContractHolderUser.ExternalId == new Guid(userId));
            }
            if (isPersonal.HasValue)
            {
                query = query.Where(al => al.IsPersonal == isPersonal.Value);
            }
            if (isActiveState.HasValue)
            {
                if (isActiveState.Value) query = query.Where(al => al.AssetLifecycleStatus == AssetLifecycleStatus.InputRequired ||
                                      al.AssetLifecycleStatus == AssetLifecycleStatus.InUse ||
                                      al.AssetLifecycleStatus == AssetLifecycleStatus.PendingReturn ||
                                      al.AssetLifecycleStatus == AssetLifecycleStatus.Repair ||
                                      al.AssetLifecycleStatus == AssetLifecycleStatus.Available ||
                                      al.AssetLifecycleStatus == AssetLifecycleStatus.Active ||
                                      al.AssetLifecycleStatus == AssetLifecycleStatus.ExpiresSoon ||
                                      al.AssetLifecycleStatus == AssetLifecycleStatus.PendingRecycle ||
                                      al.AssetLifecycleStatus == AssetLifecycleStatus.Expired);

                else query = query.Where(al => al.AssetLifecycleStatus == AssetLifecycleStatus.Lost ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Stolen ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.BoughtByUser ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Recycled ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Discarded ||
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Returned || 
                                    al.AssetLifecycleStatus == AssetLifecycleStatus.Inactive);

            }
            if (endPeriodMonth.HasValue)
            {
                query = query.Where(al => al.EndPeriod.HasValue && al.EndPeriod.Value.Month == endPeriodMonth.Value.Month && al.EndPeriod.Value.Year == endPeriodMonth.Value.Year);
            }

            if (purchaseMonth.HasValue)
            {
                query = query.Where(al => al.PurchaseDate.Month == purchaseMonth.Value.Month && al.PurchaseDate.Year == purchaseMonth.Value.Year);
            }

            query = query.AsSplitQuery().AsNoTracking();
            return await query.PaginateAsync(page, limit, cancellationToken);
        }
        public async Task<ServiceModel.CustomerAssetsCounterDTO> GetAssetLifecycleCountForCustomerAsync(Guid customerId, Guid? userId, IList<AssetLifecycleStatus> statuses)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();
            query = query.Where(a => a.CustomerId == customerId);

            if (statuses != null)
            {
                query = query.Where(a => statuses.Contains(a.AssetLifecycleStatus));
            }

            var queryCounter = await query.GroupBy(c => new
            {
                c.CustomerId,
            }).Select(group => new ServiceModel.CustomerAssetsCounterDTO
            {
                OrganizationId = group.Key.CustomerId,
                Personal = new ServiceModel.AssetCounter()
                {
                    InUse = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.InUse),
                    InputRequired = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.InputRequired),
                    Active = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Active),
                    Available = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Available),
                    Expired = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Expired),
                    Repair = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Repair),
                    ExpiresSoon = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.ExpiresSoon),
                    PendingReturn = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.PendingReturn),
                    PendingRecycle = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.PendingRecycle),
                },
                NonPersonal = new ServiceModel.AssetCounter()
                {
                    InUse = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.InUse),
                    InputRequired = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.InputRequired),
                    Active = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Active),
                    Available = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Available),
                    Expired = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Expired),
                    Repair = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Repair),
                    ExpiresSoon = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.ExpiresSoon),
                    PendingReturn = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.PendingReturn),
                    PendingRecycle = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.PendingRecycle),
                },

            }).FirstOrDefaultAsync();

            return queryCounter ?? new ServiceModel.CustomerAssetsCounterDTO();
        }
        public async Task<ServiceModel.CustomerAssetsCounterDTO> GetAssetCountForDepartmentAsync(Guid customerId, Guid? userId, IList<AssetLifecycleStatus> statuses, IList<Guid?> departments)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();
            query = query.Where(a => a.CustomerId == customerId && departments.Contains(a.ManagedByDepartmentId));

            if (statuses != null)
            {
                query = query.Where(a => statuses.Contains(a.AssetLifecycleStatus));
            }

            var queryCounter = query.GroupBy(c => new
            {
                c.ManagedByDepartmentId,
            }).Select(group => new ServiceModel.AssetCounterDepartment
            {
                DepartmentId = group.Key.ManagedByDepartmentId,
                Personal = new ServiceModel.AssetCounter()
                {
                    InUse = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.InUse),
                    InputRequired = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.InputRequired),
                    Active = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Active),
                    Available = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Available),
                    Expired = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Expired),
                    Repair = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Repair),
                    ExpiresSoon = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.ExpiresSoon),
                    PendingReturn = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.PendingReturn),
                    PendingRecycle = group.Count(x => x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.PendingRecycle),
                },
                NonPersonal = new ServiceModel.AssetCounter()
                {
                    InUse = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.InUse),
                    InputRequired = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.InputRequired),
                    Active = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Active),
                    Available = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Available),
                    Expired = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Expired),
                    Repair = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.Repair),
                    ExpiresSoon = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.ExpiresSoon),
                    PendingReturn = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.PendingReturn),
                    PendingRecycle = group.Count(x => !x.IsPersonal && x.AssetLifecycleStatus == AssetLifecycleStatus.PendingRecycle),
                },

            });

            var baseAssetCounter = new ServiceModel.CustomerAssetsCounterDTO()
            {
                Departments = await queryCounter.ToListAsync(),
                OrganizationId = customerId
            };

            return baseAssetCounter;
        }
        public async Task<int> GetAssetLifecycleCountForUserAsync(Guid customerId, Guid? userId)
        {
            return await _assetContext.AssetLifeCycles.Where(a => a.CustomerId == customerId && a.ContractHolderUser.ExternalId == userId).CountAsync();
        }


        public async Task<IList<AssetLifecycle>> GetAssetLifecyclesFromListAsync(Guid customerId, IList<Guid> assetGuidList, bool includeAsset = false, bool includeImeis = false, bool includeContractHolderUser = false, bool includeLabels = false, bool asNoTracking = false)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();

            if (includeAsset)
                if (includeImeis)
                    query = query.Include(al => al.Asset).ThenInclude(mp => (mp as MobilePhone).Imeis);
                else
                    query = query.Include(al => al.Asset);

            if (includeContractHolderUser)
                query = query.Include(al => al.ContractHolderUser);

            if (includeLabels)
                query = query.Include(al => al.Labels);

            query = query.Where(al => assetGuidList.Contains(al.ExternalId) && al.CustomerId == customerId);

            if (asNoTracking)
                return await query
                .AsSplitQuery().AsNoTracking().ToListAsync();
            else
                return await query
                .AsSplitQuery().AsTracking().ToListAsync();
        }

        public async Task<IList<CustomerLabel>> AddCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels)
        {
            _assetContext.CustomerLabels.AddRange(labels);
            await SaveEntitiesAsync();
            return await _assetContext.CustomerLabels
                         .Where(c => c.CustomerId == customerId && !c.IsDeleted).AsNoTracking().ToListAsync();
        }

        public async Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId)
        {
            return await _assetContext.CustomerLabels
                         .Where(a => a.CustomerId == customerId && !a.IsDeleted).AsNoTracking().ToListAsync();
        }

        public async Task<IList<CustomerLabel>> GetCustomerLabelsFromListAsync(IList<Guid> labelsGuid, Guid customerId)
        {
            return await _assetContext.CustomerLabels
                         .Where(a => labelsGuid.Contains<Guid>(a.ExternalId)).ToListAsync();
        }

        public async Task<CustomerLabel?> GetCustomerLabelAsync(Guid labelGuid, Guid customerId)
        {
            return await _assetContext.CustomerLabels
                         .Where(l => labelGuid == l.ExternalId && customerId == l.CustomerId).FirstOrDefaultAsync();
        }

        public async Task<IList<CustomerLabel>> DeleteCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels)
        {

            _assetContext.CustomerLabels.RemoveRange(labels);
            await SaveEntitiesAsync();
            return await GetCustomerLabelsForCustomerAsync(customerId);
        }

        public async Task<IList<CustomerLabel>> UpdateCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels)
        {
            foreach (var updateLabel in labels)
            {
                var original = await GetCustomerLabelAsync(updateLabel.ExternalId, customerId) ?? throw new ResourceNotFoundException($"Labels with id {updateLabel.ExternalId} not found", Guid.Parse("968b4e19-12f7-4bef-939d-f97054fbc27e"));
                original?.PatchLabel(updateLabel.UpdatedBy, updateLabel.Label);
            }

            await SaveEntitiesAsync();
            return await GetCustomerLabelsForCustomerAsync(customerId);
        }

        public async Task<IList<AssetLifecycle>> GetAssetLifecyclesForUserAsync(Guid customerId, Guid userId, bool includeAsset = false, bool includeImeis = false, bool includeContractHolderUser = false, bool asNoTracking = false)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();

            if (includeAsset)
                if (includeImeis)
                    query = query.Include(al => al.Asset).ThenInclude(mp => (mp as MobilePhone).Imeis);
                else
                    query = query.Include(al => al.Asset);

            if(includeContractHolderUser)
                query = query.Include(al => al.ContractHolderUser);

            if(asNoTracking)
                return await query
                .Where(a => a.CustomerId == customerId && a.ContractHolderUser!.ExternalId == userId)
                .AsNoTracking().ToListAsync();
            else
                return await query
                .Where(a => a.CustomerId == customerId && a.ContractHolderUser!.ExternalId == userId)
                .AsTracking().ToListAsync();
        }

        public async Task UnAssignAssetLifecyclesForUserAsync(Guid customerId, Guid userId, Guid? departmentId, Guid callerId)
        {
            var assetLifecyclesForUser = _assetContext.AssetLifeCycles
                .Include(a => a.ContractHolderUser)
                .Where(a => a.CustomerId == customerId && a.ContractHolderUser!.ExternalId == userId);

            if (!assetLifecyclesForUser.Any()) return;

            foreach (var assetLifecycle in assetLifecyclesForUser)
            {
                if (departmentId != null)
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


        public async Task<AssetLifecycle?> GetAssetLifecycleAsync(Guid customerId, Guid assetLifecycleId, string? userId, IList<Guid?>? department,
            bool includeAsset = false, bool includeImeis = false, bool includeLabels = false, bool includeContractHolderUser = false, bool asNoTracking = false)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();
            if (includeAsset)
            {
                if (includeImeis)
                    query = query.Include(al => al.Asset).ThenInclude(mp => (mp as MobilePhone).Imeis);
                else
                    query = query.Include(al => al.Asset);
            }

            if (includeLabels)
                query = query.Include(al => al.Labels);

            if (includeContractHolderUser)
                query = query.Include(al => al.ContractHolderUser);

            query = query.Where(al => al.CustomerId == customerId && al.ExternalId == assetLifecycleId);

            if (userId != null)
            {
                query = query.Where(al => al.ContractHolderUser.ExternalId == new Guid(userId));
            }
            if (department != null)
            {
                query = query.Where(al => department.Contains(al.ManagedByDepartmentId));
            }
            if (asNoTracking)
                return await query.AsNoTracking().FirstOrDefaultAsync();
            else
                return await query.FirstOrDefaultAsync();
        }

        public async Task<User?> GetUser(Guid userId, bool asNoTracking = false)
        {
            if(asNoTracking)
                return await _assetContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.ExternalId == userId);
            else
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
            var setting = await _assetContext.CustomerSettings.FirstOrDefaultAsync(x => x.CustomerId == customerId);
            if (setting != null) throw new InvalidOperationException();
            await _assetContext.CustomerSettings.AddAsync(customerSettings);
            await SaveEntitiesAsync();
            return customerSettings;
        }

        public async Task<CustomerSettings?> GetCustomerSettingsAsync(Guid customerId, bool asNoTracking = false)
        {
            if(asNoTracking)
                return await _assetContext.CustomerSettings.Include(x => x.LifeCycleSettings).AsNoTracking().FirstOrDefaultAsync(x => x.CustomerId == customerId);
            else
                return await _assetContext.CustomerSettings.Include(x => x.LifeCycleSettings).FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }
        public async Task<LifeCycleSetting?> GetCustomerLifeCycleSettingAssetCategory(Guid customerId, int assetCategoryId)
        {
            var setting = await _assetContext.CustomerSettings.Include(x => x.LifeCycleSettings).AsNoTracking().FirstOrDefaultAsync(x => x.CustomerId == customerId);
            return setting?.LifeCycleSettings.FirstOrDefault(x => x.AssetCategoryId == assetCategoryId);
        }


        #endregion

    }
}