using AssetServices.Exceptions;
using AssetServices.Models;
using AssetServices.ServiceModel;
using AutoMapper.QueryableExtensions;
using Common.Enums;
using Common.Extensions;
using Common.Interfaces;
using Common.Logging;
using Common.Seedwork;
using Common.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

#nullable enable
        public async Task<PagedModel<AssetLifecycle>> AdvancedSearchAsync(SearchParameters searchParameters, int page, int limit, CancellationToken cancellationToken)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();

            // Where filter: Customer-ID is in list
            if (searchParameters.CustomerIds is not null && searchParameters.CustomerIds.Any())
            {
                query = query.Where(e => searchParameters.CustomerIds.Contains(e.CustomerId));
            }

            // Where filter: User-ID is in list
            if (searchParameters.UserIds is not null && searchParameters.UserIds.Any())
            {
                query = query.Where(e => e.ContractHolderUser != null && searchParameters.UserIds.Contains(e.ContractHolderUser.ExternalId));
            }

            // Where filter: Department-ID is in list
            if (searchParameters.DepartmentIds is not null && searchParameters.DepartmentIds.Any())
            {
                query = query.Where(e => e.ManagedByDepartmentId.HasValue && searchParameters.DepartmentIds.Contains(e.ManagedByDepartmentId.Value));
            }

            // Where filter: Asset category ID is in list
            if (searchParameters.AssetCategoryIds is not null && searchParameters.AssetCategoryIds.Any())
            {
                // The asset-category IDs we return from our APIs is set in-code, and not kept anywhere in the database.
                // In the database, the category-type is set through a hidden discriminator (shadow-property). This determines
                // if the inherited "Asset"-entity is for example a "Tablet" or a "Mobile Phone".
                //
                // As the IDs is not actually present in the DB, it makes it a bit tricky to apply conditionally or-filters for it,
                // so we have to do some "black magic" here by extracting the class-names, before filtering on the shadow-property.
                //
                // More details: https://learn.microsoft.com/en-us/ef/core/modeling/inheritance

                List<string> types = new();

                foreach (int id in searchParameters.AssetCategoryIds)
                {
                    if (id == 1)
                    {
                        types.Add(typeof(MobilePhone).Name);
                    }

                    if (id == 2)
                    {
                        types.Add(typeof(Tablet).Name);
                    }

                    // TODO: Find a smoother way, so we don't have to update this list as we add in new types.
                }

                // On the base asset-class, let's apply a "where in" filter on the type-discriminator (the shadow property named "Discriminator")
                query = query.Where(e => types.Contains(EF.Property<string>(e.Asset, "Discriminator")));
            }

            // Where filter: Lifecycle status-ID is in list
            if (searchParameters.AssetLifecycleStatusIds is not null && searchParameters.AssetLifecycleStatusIds.Any())
            {
                query = query.Where(e => searchParameters.AssetLifecycleStatusIds.Contains((int)e.AssetLifecycleStatus));
            }

            // Where filter: Lifecycle type-ID is in list
            if (searchParameters.AssetLifecycleTypeIds is not null && searchParameters.AssetLifecycleTypeIds.Any())
            {
                query = query.Where(e => searchParameters.AssetLifecycleTypeIds.Contains((int)e.AssetLifecycleType));
            }

            // Where filter: Start period
            if (searchParameters.StartPeriod is not null && searchParameters.StartPeriod != default)
            {
                switch (searchParameters.StartPeriodSearchType)
                {
                    case DateSearchType.ExcactDate:
                        query = query.Where(e => e.StartPeriod.HasValue && e.StartPeriod.Value.Date == searchParameters.StartPeriod.Value.Date);
                        break;
                    case DateSearchType.OnOrAfterDate:
                        query = query.Where(e => e.StartPeriod.HasValue && e.StartPeriod.Value.Date >= searchParameters.StartPeriod.Value.Date);
                        break;
                    case DateSearchType.OnOrBeforeDate:
                        query = query.Where(e => e.StartPeriod.HasValue && e.StartPeriod.Value.Date <= searchParameters.StartPeriod.Value.Date);
                        break;
                    default:
                        break;
                }
            }

            // Where filter: End period
            if (searchParameters.EndPeriod is not null && searchParameters.EndPeriod != default)
            {
                switch (searchParameters.EndPeriodSearchType)
                {
                    case DateSearchType.ExcactDate:
                        query = query.Where(e => e.EndPeriod.HasValue && e.EndPeriod.Value.Date == searchParameters.EndPeriod.Value.Date);
                        break;
                    case DateSearchType.OnOrAfterDate:
                        query = query.Where(e => e.EndPeriod.HasValue && e.EndPeriod.Value.Date >= searchParameters.EndPeriod.Value.Date);
                        break;
                    case DateSearchType.OnOrBeforeDate:
                        query = query.Where(e => e.EndPeriod.HasValue && e.EndPeriod.Value.Date <= searchParameters.EndPeriod.Value.Date);
                        break;
                    default:
                        break;
                }
            }

            // Where filter: Purchase date
            if (searchParameters.PurchaseDate is not null && searchParameters.PurchaseDate != default)
            {
                switch (searchParameters.PurchaseDateSearchType)
                {
                    case DateSearchType.ExcactDate:
                        query = query.Where(e => e.PurchaseDate.Date == searchParameters.PurchaseDate.Value.Date);
                        break;
                    case DateSearchType.OnOrAfterDate:
                        query = query.Where(e => e.PurchaseDate.Date >= searchParameters.PurchaseDate.Value.Date);
                        break;
                    case DateSearchType.OnOrBeforeDate:
                        query = query.Where(e => e.PurchaseDate.Date <= searchParameters.PurchaseDate.Value.Date);
                        break;
                    default:
                        break;
                }
            }

            // Where filter: IMEI
            if (!string.IsNullOrEmpty(searchParameters.Imei))
            {
                switch (searchParameters.ImeiSearchType)
                {
                    case StringSearchType.Equals:
                        {

                            query = query.Where(e =>
                                (e.Asset is MobilePhone && (e.Asset as MobilePhone)!.Imeis.Any(xx => EF.Functions.Like(xx.Imei.ToString(), searchParameters.Imei)))
                                || (e.Asset is Tablet && (e.Asset as Tablet)!.Imeis.Any(xx => EF.Functions.Like(xx.Imei.ToString(), searchParameters.Imei)))
                            );

                            break;
                        }
                    case StringSearchType.StartsWith:
                        {
                            query = query.Where(e =>
                                (e.Asset is MobilePhone && (e.Asset as MobilePhone)!.Imeis.Any(xx => EF.Functions.Like(xx.Imei.ToString(), $"{searchParameters.Imei}%")))
                                || (e.Asset is Tablet && (e.Asset as Tablet)!.Imeis.Any(xx => EF.Functions.Like(xx.Imei.ToString(), $"{searchParameters.Imei}%")))
                            );

                            break;
                        }
                    case StringSearchType.Contains:
                        {
                            query = query.Where(e =>
                                (e.Asset is MobilePhone && (e.Asset as MobilePhone)!.Imeis.Any(xx => EF.Functions.Like(xx.Imei.ToString(), $"%{searchParameters.Imei}%")))
                                || (e.Asset is Tablet && (e.Asset as Tablet)!.Imeis.Any(xx => EF.Functions.Like(xx.Imei.ToString(), $"%{searchParameters.Imei}%")))
                            );

                            break;
                        }
                    default:
                        break;
                }
            }


            return await query.AsNoTracking()
                              .PaginateAsync(page, limit, cancellationToken);
        }
#nullable restore


        public async Task<PagedModel<AssetLifecycle>> GetAssetLifecyclesAsync(Guid customerId, string? userId, IList<AssetLifecycleStatus>? status, IList<Guid?>? department, int[]? category,
           Guid[]? label, bool? isActiveState, bool? isPersonal, DateTime? endPeriodMonth, DateTime? purchaseMonth, string? search, int page, int limit, CancellationToken cancellationToken,
           bool includeAsset = false, bool includeImeis = false, bool includeLabels = false, bool includeContractHolderUser = false)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>()
                                                            .Where(al => al.CustomerId == customerId);

            if (includeAsset)
            {
                if (includeImeis)
                {
                    query = query.Include(al => al.Asset)
                                 .ThenInclude(mp => (mp as MobilePhone).Imeis);

                    query = query.Include(al => al.Asset)
                                 .ThenInclude(mp => (mp as Tablet).Imeis);
                }
                else
                {
                    query = query.Include(al => al.Asset);
                }
            }

            if (includeLabels)
                query = query.Include(al => al.Labels);

            if (includeContractHolderUser)
                query = query.Include(al => al.ContractHolderUser);

            if (!string.IsNullOrEmpty(search))
            {
                // See if the query-string contains a potential IMEI number. If not, we can skip the related search.
                bool potentialImeiFound = false;
                long imei = 0;
                if (search.Length >= 15 && long.TryParse(search, out imei))
                {
                    potentialImeiFound = true;
                }

                // See if the query-string contains any numbers. If so we can likely skips searching for some of the parameters, such as names.
                bool numbersFound = false;
                if (Regex.IsMatch(search, "\\d"))
                {
                    numbersFound = true;
                }

                // See if the query-string contains a potential MAC address. If not, we can skip the related search. This is valid if the query-string
                // starts with one of the six MAC address groups (two hexadecimal digits, followed by either '-' or ':').
                bool potentialMacAddressFound = false;
                if (Regex.IsMatch(search, "^([0-9A-Fa-f]{2}[:-])"))
                {
                    potentialMacAddressFound = true;
                }

                query = query.Where(al =>
                                        al.Alias.ToLower().Contains(search.ToLower()) ||
                                        (al.Asset != null && (
                                            //al.Asset.Brand.ToLower().Contains(search.ToLower()) ||
                                            //al.Asset.ProductName.ToLower().Contains(search.ToLower()) ||
                                            (al.Asset is MobilePhone && (al.Asset as MobilePhone)!.SerialNumber.ToLower().Contains(search.ToLower())) ||
                                            (potentialMacAddressFound && al.Asset is MobilePhone && (al.Asset as MobilePhone)!.MacAddress.ToLower().Contains(search.ToLower())) ||
                                            (potentialImeiFound && al.Asset is MobilePhone && (al.Asset as MobilePhone)!.Imeis.Any(im => im.Imei == imei)) ||
                                            (al.Asset is Tablet && (al.Asset as Tablet)!.SerialNumber.ToLower().Contains(search.ToLower())) ||
                                            (potentialMacAddressFound && al.Asset is Tablet && (al.Asset as Tablet)!.MacAddress.ToLower().Contains(search.ToLower())) ||
                                            (potentialImeiFound && al.Asset is Tablet && (al.Asset as Tablet)!.Imeis.Any(im => im.Imei == imei))
                                        )) ||
                                        (al.ContractHolderUser != null && (
                                            !numbersFound && al.ContractHolderUser.Name.ToLower().Contains(search.ToLower())
                                        ))
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
                if (isActiveState.Value)
                {
                    query = query.Where(al =>
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.InputRequired ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.InUse ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.PendingReturn ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.Repair ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.Available ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.Active ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.ExpiresSoon ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.PendingRecycle ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.Expired);
                }
                else
                {
                    query = query.Where(al =>
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.Lost ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.Stolen ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.BoughtByUser ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.Recycled ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.Discarded ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.Returned ||
                                            al.AssetLifecycleStatus == AssetLifecycleStatus.Inactive);
                }
            }

            if (endPeriodMonth.HasValue)
            {
                query = query.Where(al => al.EndPeriod.HasValue && al.EndPeriod.Value.Month == endPeriodMonth.Value.Month && al.EndPeriod.Value.Year == endPeriodMonth.Value.Year);
            }

            if (purchaseMonth.HasValue)
            {
                query = query.Where(al => al.PurchaseDate.Month == purchaseMonth.Value.Month && al.PurchaseDate.Year == purchaseMonth.Value.Year);
            }

            return await query.AsSplitQuery()
                              .AsNoTracking()
                              .PaginateAsync(page, limit, cancellationToken);
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


        public async Task<IList<AssetLifecycle>> GetAssetLifecyclesFromListAsync(Guid customerId, IList<Guid> assetGuidList, bool asNoTracking, bool includeAsset = false, bool includeImeis = false, bool includeContractHolderUser = false, bool includeLabels = false)
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
                query.AsNoTracking();

            return await query.AsSplitQuery().ToListAsync();
        }

        public async Task<IList<CustomerLabel>> AddCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels)
        {
            _assetContext.CustomerLabels.AddRange(labels);
            await SaveEntitiesAsync();
            return await _assetContext.CustomerLabels
                         .Where(c => c.CustomerId == customerId && !c.IsDeleted).AsNoTracking().ToListAsync();
        }

        public async Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId, bool asNoTracking)
        {
            IQueryable<CustomerLabel> query = _assetContext.Set<CustomerLabel>();

            query = query.Where(a => a.CustomerId == customerId && !a.IsDeleted);

            if (asNoTracking)
                query.AsNoTracking();

            return await query.ToListAsync();
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
            return await GetCustomerLabelsForCustomerAsync(customerId, true);
        }

        public async Task<IList<CustomerLabel>> UpdateCustomerLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> labels)
        {
            foreach (var updateLabel in labels)
            {
                var original = await GetCustomerLabelAsync(updateLabel.ExternalId, customerId) ?? throw new ResourceNotFoundException($"Labels with id {updateLabel.ExternalId} not found", Guid.Parse("968b4e19-12f7-4bef-939d-f97054fbc27e"));
                original?.PatchLabel(updateLabel.UpdatedBy, updateLabel.Label);
            }

            await SaveEntitiesAsync();
            return await GetCustomerLabelsForCustomerAsync(customerId, true);
        }

        public async Task<IList<AssetLifecycle>> GetAssetLifecyclesForUserAsync(Guid customerId, Guid userId, bool asNoTracking, bool includeAsset = false, bool includeImeis = false, bool includeContractHolderUser = false)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();

            query = query.Where(a => a.CustomerId == customerId && a.ContractHolderUser!.ExternalId == userId);

            if (includeAsset)
                if (includeImeis)
                    query = query.Include(al => al.Asset).ThenInclude(mp => (mp as MobilePhone).Imeis);
                else
                    query = query.Include(al => al.Asset);

            if (includeContractHolderUser)
                query = query.Include(al => al.ContractHolderUser);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
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
            if (asNoTracking)
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

        /// <inheritdoc/>
        public async Task<IList<string>> GetActiveImeisList(List<string> imeis)
        {
            IQueryable<AssetLifecycle> query = _assetContext.Set<AssetLifecycle>();
            query = query.Include(al => al.Asset).ThenInclude(mp => (mp as MobilePhone).Imeis).AsNoTracking();
            query = query.Where(q =>
            q.AssetLifecycleStatus == AssetLifecycleStatus.InputRequired ||
            q.AssetLifecycleStatus == AssetLifecycleStatus.InUse ||
            q.AssetLifecycleStatus == AssetLifecycleStatus.PendingReturn ||
            q.AssetLifecycleStatus == AssetLifecycleStatus.Repair ||
            q.AssetLifecycleStatus == AssetLifecycleStatus.Available ||
            q.AssetLifecycleStatus == AssetLifecycleStatus.Active ||
            q.AssetLifecycleStatus == AssetLifecycleStatus.ExpiresSoon ||
            q.AssetLifecycleStatus == AssetLifecycleStatus.PendingRecycle ||
            q.AssetLifecycleStatus == AssetLifecycleStatus.Expired
            && !q.IsDeleted);

            var existingImeis = await query.Where(q => (q.Asset as MobilePhone).Imeis.Any(i => imeis.Contains(i.Imei.ToString())))
                .SelectMany(x => (x.Asset as MobilePhone).Imeis.Select(s => s.Imei.ToString())).ToListAsync();

            return existingImeis;
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
            if (asNoTracking)
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