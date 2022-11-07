using Common.Enums;
using Common.Extensions;
using Common.Interfaces;
using Common.Logging;
using Common.Seedwork;
using Common.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
using System.Linq;
using CancelSubscriptionOrder = SubscriptionManagementServices.Models.CancelSubscriptionOrder;
using NewSubscriptionOrder = SubscriptionManagementServices.Models.NewSubscriptionOrder;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementRepository<T> : ISubscriptionManagementRepository<T> where T: ISubscriptionOrder
    {
        private readonly SubscriptionManagementContext _subscriptionManagementContext;
        private readonly IFunctionalEventLogService _functionalEventLogService;
        private readonly IMediator _mediator;


        public SubscriptionManagementRepository(SubscriptionManagementContext subscriptionManagementContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator)
        {
            _subscriptionManagementContext = subscriptionManagementContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
        }

        private async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_subscriptionManagementContext).ExecuteAsync(async () =>
            {
                var editedEntities = _subscriptionManagementContext.ChangeTracker.Entries()
                    .Where(entity => entity.State == EntityState.Modified)
                    .ToList();

                editedEntities.ForEach(entity =>
                {
                    if (!entity.Entity.GetType().IsSubclassOf(typeof(ValueObject)))
                    {
                        entity.Property("LastUpdatedDate").CurrentValue = DateTime.UtcNow;
                    }
                });
                if (!_subscriptionManagementContext.IsSQLite)
                {
                    // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                    foreach (var @event in _subscriptionManagementContext.GetDomainEventsAsync())
                    {
                        await _functionalEventLogService.SaveEventAsync(@event, _subscriptionManagementContext.Database.CurrentTransaction);
                    }
                }
                numberOfRecordsSaved = await _subscriptionManagementContext.SaveChangesAsync(cancellationToken);
                await _mediator.DispatchDomainEventsAsync(_subscriptionManagementContext);
            });
            return numberOfRecordsSaved;
        }

        public async Task<List<ISubscriptionOrder>> GetAllSubscriptionOrdersForCustomer(Guid organizationId)
        {
            var orders = await _subscriptionManagementContext.TransferSubscriptionOrders.Include(o => o.PrivateSubscription)
                .Include(o => o.BusinessSubscription).Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            var subscriptionOrderList = orders.ToList();

            var transferToPrivateOrders = await _subscriptionManagementContext.TransferToPrivateSubscriptionOrders
                .Include(o => o.UserInfo).Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(transferToPrivateOrders);

            var changeOrders = await _subscriptionManagementContext.ChangeSubscriptionOrder
                .Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(changeOrders);

            var cancelOrders = await _subscriptionManagementContext.CancelSubscriptionOrders
                .Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(cancelOrders);

            var orderSimSubscriptionOrders = await _subscriptionManagementContext.OrderSimSubscriptionOrders
                .Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(orderSimSubscriptionOrders);

            var activateSimOrders = await _subscriptionManagementContext.ActivateSimOrders
                .Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(activateSimOrders);

            var newSubscription = await _subscriptionManagementContext.NewSubscriptionOrders
                .Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(newSubscription);

            return subscriptionOrderList.OrderByDescending(o=> o.CreatedDate).Take(15).ToList();
        }
        public async Task<PagedModel<SubscriptionOrderListItemDTO>> GetAllSubscriptionOrdersForCustomer(Guid organizationId, string? search, IList<int>? OrderType, int page, int limit, CancellationToken cancellationToken)
        {
            IQueryable<SubscriptionOrderBaseData>? query = null;
            if (OrderType == null || !OrderType.Any())
            {
                OrderType = Enum.GetValues<SubscriptionOrderTypes>().Select(x => (int)x).ToList();
            }
            foreach (var orderType in OrderType)
            {
                switch ((SubscriptionOrderTypes)orderType)
                {
                    case SubscriptionOrderTypes.TransferToPrivate:
                        if(query == null)
                        {
                            query = _subscriptionManagementContext.Set<TransferToPrivateSubscriptionOrder>()
                                    .Where(x => x.OrganizationId == organizationId)
                                        .Select(x => new SubscriptionOrderBaseData
                                        {
                                            SubscriptionOrderId = x.SubscriptionOrderId,
                                            CreatedDate = x.CreatedDate,
                                            OrderTypeId = (int)SubscriptionOrderTypes.TransferToPrivate,
                                            OrderType = SubscriptionOrderTypes.TransferToPrivate.ToString(),
                                            CustomerId = x.OrganizationId,
                                            PhoneNumber = x.MobileNumber,
                                            OrderExecutionDate = x.OrderExecutionDate,
                                            CreatedBy = x.CreatedBy,
                                            OrderNumber = x.SalesforceTicketId,
                                            PrivateFirstName = x.UserInfo.FirstName,
                                            PrivateLastName = x.UserInfo.LastName,
                                            BusinessFirstName = "",
                                            BusinessLastName = ""
                                        });
                        }
                        else
                        {
                            query = query.Concat(_subscriptionManagementContext.Set<TransferToPrivateSubscriptionOrder>()
                                    .Where(x => x.OrganizationId == organizationId)
                                        .Select(x => new SubscriptionOrderBaseData
                                        {
                                            SubscriptionOrderId = x.SubscriptionOrderId,
                                            CreatedDate = x.CreatedDate,
                                            OrderTypeId = (int)SubscriptionOrderTypes.TransferToPrivate,
                                            OrderType = SubscriptionOrderTypes.TransferToPrivate.ToString(),
                                            CustomerId = x.OrganizationId,
                                            PhoneNumber = x.MobileNumber,
                                            OrderExecutionDate = x.OrderExecutionDate,
                                            CreatedBy = x.CreatedBy,
                                            OrderNumber = x.SalesforceTicketId,
                                            PrivateFirstName = x.UserInfo.FirstName,
                                            PrivateLastName = x.UserInfo.LastName,
                                            BusinessFirstName = "",
                                            BusinessLastName = ""
                                        }));
                        }
                        break;
                    case SubscriptionOrderTypes.TransferToBusiness:
                        if (query == null)
                        {
                            query = _subscriptionManagementContext.Set<TransferToBusinessSubscriptionOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                        .Select(x => new SubscriptionOrderBaseData
                        {
                            SubscriptionOrderId = x.SubscriptionOrderId,
                            CreatedDate = x.CreatedDate,
                            OrderTypeId = (int)SubscriptionOrderTypes.TransferToBusiness,
                            OrderType = SubscriptionOrderTypes.TransferToBusiness.ToString(),
                            CustomerId = x.OrganizationId,
                            PhoneNumber = x.MobileNumber,
                            OrderExecutionDate = x.OrderExecutionDate,
                            CreatedBy = x.CreatedBy,
                            OrderNumber = x.SalesforceTicketId,
                            PrivateFirstName = x.PrivateSubscription!.FirstName,
                            PrivateLastName = x.PrivateSubscription.LastName,
                            BusinessFirstName = x.BusinessSubscription!.Name,
                            BusinessLastName = ""
                        });
                        }
                        else
                        {
                            query = query.Concat(_subscriptionManagementContext.Set<TransferToBusinessSubscriptionOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                        .Select(x => new SubscriptionOrderBaseData
                        {
                            SubscriptionOrderId = x.SubscriptionOrderId,
                            CreatedDate = x.CreatedDate,
                            OrderTypeId = (int)SubscriptionOrderTypes.TransferToBusiness,
                            OrderType = SubscriptionOrderTypes.TransferToBusiness.ToString(),
                            CustomerId = x.OrganizationId,
                            PhoneNumber = x.MobileNumber,
                            OrderExecutionDate = x.OrderExecutionDate,
                            CreatedBy = x.CreatedBy,
                            OrderNumber = x.SalesforceTicketId,
                            PrivateFirstName = x.PrivateSubscription!.FirstName,
                            PrivateLastName = x.PrivateSubscription.LastName,
                            BusinessFirstName = x.BusinessSubscription!.Name,
                            BusinessLastName = ""
                        }));
                        }
                        break;
                    case SubscriptionOrderTypes.OrderSim:
                        if (query == null)
                        {
                            query = _subscriptionManagementContext.Set<OrderSimSubscriptionOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                            .Select(x => new SubscriptionOrderBaseData
                            {
                                SubscriptionOrderId = x.SubscriptionOrderId,
                                CreatedDate = x.CreatedDate,
                                OrderTypeId = (int)SubscriptionOrderTypes.OrderSim,
                                OrderType = SubscriptionOrderTypes.OrderSim.ToString(),
                                CustomerId = x.OrganizationId,
                                PhoneNumber = "",
                                OrderExecutionDate = x.CreatedDate,
                                CreatedBy = x.CreatedBy,
                                OrderNumber = x.SalesforceTicketId,
                                PrivateFirstName = x.SendToName,
                                PrivateLastName = "",
                                BusinessFirstName = "",
                                BusinessLastName = ""
                            });
                        }
                        else
                        {
                            query = query.Concat(_subscriptionManagementContext.Set<OrderSimSubscriptionOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                            .Select(x => new SubscriptionOrderBaseData
                            {
                                SubscriptionOrderId = x.SubscriptionOrderId,
                                CreatedDate = x.CreatedDate,
                                OrderTypeId = (int)SubscriptionOrderTypes.OrderSim,
                                OrderType = SubscriptionOrderTypes.OrderSim.ToString(),
                                CustomerId = x.OrganizationId,
                                PhoneNumber = "",
                                OrderExecutionDate = x.CreatedDate,
                                CreatedBy = x.CreatedBy,
                                OrderNumber = x.SalesforceTicketId,
                                PrivateFirstName = x.SendToName,
                                PrivateLastName = "",
                                BusinessFirstName = "",
                                BusinessLastName = ""
                            }));
                        }
                        break;
                    case SubscriptionOrderTypes.ActivateSim:
                        if (query == null)
                        {
                            query = _subscriptionManagementContext.Set<ActivateSimOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                            .Select(x => new SubscriptionOrderBaseData
                            {
                                SubscriptionOrderId = x.SubscriptionOrderId,
                                CreatedDate = x.CreatedDate,
                                OrderTypeId = (int)SubscriptionOrderTypes.ActivateSim,
                                OrderType = SubscriptionOrderTypes.ActivateSim.ToString(),
                                CustomerId = x.OrganizationId,
                                PhoneNumber = x.MobileNumber,
                                OrderExecutionDate = x.CreatedDate,
                                CreatedBy = x.CreatedBy,
                                OrderNumber = x.SalesforceTicketId,
                                PrivateFirstName = "",
                                PrivateLastName = "",
                                BusinessFirstName = "",
                                BusinessLastName = ""
                            });
                        }
                        else
                        {
                            query = query.Concat(_subscriptionManagementContext.Set<ActivateSimOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                            .Select(x => new SubscriptionOrderBaseData
                            {
                                SubscriptionOrderId = x.SubscriptionOrderId,
                                CreatedDate = x.CreatedDate,
                                OrderTypeId = (int)SubscriptionOrderTypes.ActivateSim,
                                OrderType = SubscriptionOrderTypes.ActivateSim.ToString(),
                                CustomerId = x.OrganizationId,
                                PhoneNumber = x.MobileNumber,
                                OrderExecutionDate = x.CreatedDate,
                                CreatedBy = x.CreatedBy,
                                OrderNumber = x.SalesforceTicketId,
                                PrivateFirstName = "",
                                PrivateLastName = "",
                                BusinessFirstName = "",
                                BusinessLastName = ""
                            }));
                        }
                        break;
                    case SubscriptionOrderTypes.NewSubscription:
                        if (query == null)
                        {
                            query = _subscriptionManagementContext.Set<NewSubscriptionOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                            .Select(x => new SubscriptionOrderBaseData
                            {
                                SubscriptionOrderId = x.SubscriptionOrderId,
                                CreatedDate = x.CreatedDate,
                                OrderTypeId = (int)SubscriptionOrderTypes.NewSubscription,
                                OrderType = SubscriptionOrderTypes.NewSubscription.ToString(),
                                CustomerId = x.OrganizationId,
                                PhoneNumber = "",
                                OrderExecutionDate = x.OrderExecutionDate,
                                CreatedBy = x.CreatedBy,
                                OrderNumber = x.SalesforceTicketId,
                                PrivateFirstName = x.PrivateSubscription!.FirstName,
                                PrivateLastName = x.PrivateSubscription!.LastName,
                                BusinessFirstName = x.SimCardReceiverFirstName!,
                                BusinessLastName = x.SimCardReceiverLastName!
                            });
                        }
                        else
                        {
                            query = query.Concat(_subscriptionManagementContext.Set<NewSubscriptionOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                            .Select(x => new SubscriptionOrderBaseData
                            {
                                SubscriptionOrderId = x.SubscriptionOrderId,
                                CreatedDate = x.CreatedDate,
                                OrderTypeId = (int)SubscriptionOrderTypes.NewSubscription,
                                OrderType = SubscriptionOrderTypes.NewSubscription.ToString(),
                                CustomerId = x.OrganizationId,
                                PhoneNumber = "",
                                OrderExecutionDate = x.OrderExecutionDate,
                                CreatedBy = x.CreatedBy,
                                OrderNumber = x.SalesforceTicketId,
                                PrivateFirstName = x.PrivateSubscription!.FirstName,
                                PrivateLastName = x.PrivateSubscription!.LastName,
                                BusinessFirstName = x.SimCardReceiverFirstName!,
                                BusinessLastName = x.SimCardReceiverLastName!
                            }));
                        }
                        break;
                    case SubscriptionOrderTypes.ChangeSubscription:
                        if (query == null)
                        {
                            query = _subscriptionManagementContext.Set<ChangeSubscriptionOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                            .Select(x => new SubscriptionOrderBaseData
                            {
                                SubscriptionOrderId = x.SubscriptionOrderId,
                                CreatedDate = x.CreatedDate,
                                OrderTypeId = (int)SubscriptionOrderTypes.ChangeSubscription,
                                OrderType = SubscriptionOrderTypes.ChangeSubscription.ToString(),
                                CustomerId = x.OrganizationId,
                                PhoneNumber = x.MobileNumber,
                                OrderExecutionDate = x.CreatedDate,
                                CreatedBy = x.CreatedBy,
                                OrderNumber = x.SalesforceTicketId,
                                PrivateFirstName = x.SubscriptionOwner != null ? x.SubscriptionOwner : "Owner not specified",
                                PrivateLastName = "",
                                BusinessFirstName = "",
                                BusinessLastName = ""
                            });
                        }
                        else
                        {
                            query = query.Concat(_subscriptionManagementContext.Set<ChangeSubscriptionOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                            .Select(x => new SubscriptionOrderBaseData
                            {
                                SubscriptionOrderId = x.SubscriptionOrderId,
                                CreatedDate = x.CreatedDate,
                                OrderTypeId = (int)SubscriptionOrderTypes.ChangeSubscription,
                                OrderType = SubscriptionOrderTypes.ChangeSubscription.ToString(),
                                CustomerId = x.OrganizationId,
                                PhoneNumber = x.MobileNumber,
                                OrderExecutionDate = x.CreatedDate,
                                CreatedBy = x.CreatedBy,
                                OrderNumber = x.SalesforceTicketId,
                                PrivateFirstName = x.SubscriptionOwner != null ? x.SubscriptionOwner : "Owner not specified",
                                PrivateLastName = "",
                                BusinessFirstName = "",
                                BusinessLastName = ""
                            }));
                        }
                        break;
                    case SubscriptionOrderTypes.CancelSubscription:
                        if (query == null)
                        {
                            query = _subscriptionManagementContext.Set<CancelSubscriptionOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                            .Select(x => new SubscriptionOrderBaseData
                            {
                                SubscriptionOrderId = x.SubscriptionOrderId,
                                CreatedDate = x.CreatedDate,
                                OrderTypeId = (int)SubscriptionOrderTypes.CancelSubscription,
                                OrderType = SubscriptionOrderTypes.CancelSubscription.ToString(),
                                CustomerId = x.OrganizationId,
                                PhoneNumber = x.MobileNumber,
                                OrderExecutionDate = x.DateOfTermination,
                                CreatedBy = x.CreatedBy,
                                OrderNumber = x.SalesforceTicketId,
                                PrivateFirstName = "",
                                PrivateLastName = "",
                                BusinessFirstName = "",
                                BusinessLastName = ""
                            });
                        }
                        else
                        {
                            query = query.Concat(_subscriptionManagementContext.Set<CancelSubscriptionOrder>()
                        .Where(x => x.OrganizationId == organizationId)
                            .Select(x => new SubscriptionOrderBaseData
                            {
                                SubscriptionOrderId = x.SubscriptionOrderId,
                                CreatedDate = x.CreatedDate,
                                OrderTypeId = (int)SubscriptionOrderTypes.CancelSubscription,
                                OrderType = SubscriptionOrderTypes.CancelSubscription.ToString(),
                                CustomerId = x.OrganizationId,
                                PhoneNumber = x.MobileNumber,
                                OrderExecutionDate = x.DateOfTermination,
                                CreatedBy = x.CreatedBy,
                                OrderNumber = x.SalesforceTicketId,
                                PrivateFirstName = "",
                                PrivateLastName = "",
                                BusinessFirstName = "",
                                BusinessLastName = ""
                            }));
                        }
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query!.Where(x => x.PhoneNumber.ToLower().Contains(search.ToLower()) || x.OrderNumber.ToLower().Contains(search.ToLower()));
            }

            return await query!.Select(x => new SubscriptionOrderListItemDTO()
            {
                OrderNumber = x.OrderNumber!,
                OrderTypeId = x.OrderTypeId,
                SubscriptionOrderId = x.SubscriptionOrderId,
                CreatedDate = x.CreatedDate,
                OrderType = x.OrderType,
                PhoneNumber = x.PhoneNumber,
                NewSubscriptionOrderOwnerName = x.OrderType == SubscriptionOrderTypes.CancelSubscription.ToString() ? 
                    x.PhoneNumber : x.OrderType == SubscriptionOrderTypes.ActivateSim.ToString() ?
                    (string.IsNullOrEmpty(x.PhoneNumber) ? "Order reference not specified" : x.PhoneNumber) : string.IsNullOrEmpty(($"{x.PrivateFirstName} {x.PrivateLastName}").Trim()) ? 
                    $"{x.BusinessFirstName} {x.BusinessLastName}" : $"{x.PrivateFirstName} {x.PrivateLastName}",
                OrderExecutionDate = x.OrderExecutionDate,
                CreatedBy = x.CreatedBy
            }).PaginateAsync(page, limit, cancellationToken);
        }





        public async Task<int> GetTotalSubscriptionOrdersCountForCustomer(Guid organizationId, IList<SubscriptionOrderTypes>? orderTypes = null, string? phoneNumber = null, bool checkOrderExist = false)
        {
            int subscriptionsCount = 0;
            if (orderTypes == null || orderTypes.Count() == 0)
            {
                orderTypes = Enum.GetValues<SubscriptionOrderTypes>().ToList();
            }
            foreach (var orderType in orderTypes)
            {
                switch (orderType)
                {
                    case SubscriptionOrderTypes.TransferToPrivate:
                        var transferToPrivateOrdersCount = await _subscriptionManagementContext.TransferToPrivateSubscriptionOrders
                            .Where(o => o.OrganizationId == organizationId && (string.IsNullOrEmpty(phoneNumber) || o.MobileNumber == phoneNumber ))
                            .CountAsync();
                        subscriptionsCount += transferToPrivateOrdersCount;
                        break;
                    case SubscriptionOrderTypes.TransferToBusiness:
                        var transferToBusinessOrdersCount = await _subscriptionManagementContext.TransferSubscriptionOrders
                            .Where(o => o.OrganizationId == organizationId && (string.IsNullOrEmpty(phoneNumber) || o.MobileNumber == phoneNumber))
                            .CountAsync();
                        subscriptionsCount += transferToBusinessOrdersCount;
                        break;
                    case SubscriptionOrderTypes.OrderSim:
                        if (string.IsNullOrEmpty(phoneNumber))
                        {
                            var orderSimSubscriptionOrdersCount = await _subscriptionManagementContext.OrderSimSubscriptionOrders
                            .Where(o => o.OrganizationId == organizationId)
                            .CountAsync();
                            subscriptionsCount += orderSimSubscriptionOrdersCount;
                        }
                        break;
                    case SubscriptionOrderTypes.ActivateSim:
                        var activateSimOrdersCount = await _subscriptionManagementContext.ActivateSimOrders
                            .Where(o => o.OrganizationId == organizationId && (string.IsNullOrEmpty(phoneNumber) || o.MobileNumber == phoneNumber))
                            .CountAsync();
                        subscriptionsCount += activateSimOrdersCount;
                        break;
                    case SubscriptionOrderTypes.NewSubscription:
                        if (string.IsNullOrEmpty(phoneNumber))
                        {
                            var newSubscriptionCount = await _subscriptionManagementContext.NewSubscriptionOrders
                                        .Where(o => o.OrganizationId == organizationId)
                            .CountAsync();
                            subscriptionsCount += newSubscriptionCount;
                        }
                        
                        break;
                    case SubscriptionOrderTypes.ChangeSubscription:
                        var changeOrdersCount = await _subscriptionManagementContext.ChangeSubscriptionOrder
                            .Where(o => o.OrganizationId == organizationId && (string.IsNullOrEmpty(phoneNumber) || o.MobileNumber == phoneNumber))
                            .CountAsync();
                        subscriptionsCount += changeOrdersCount;
                        break;
                    case SubscriptionOrderTypes.CancelSubscription:
                        var cancelOrdersCount = await _subscriptionManagementContext.CancelSubscriptionOrders
                            .Where(o => o.OrganizationId == organizationId && (string.IsNullOrEmpty(phoneNumber) || o.MobileNumber == phoneNumber))
                            .CountAsync();
                        subscriptionsCount += cancelOrdersCount;
                        break;
                    default:
                        break;
                }

                if (checkOrderExist && subscriptionsCount > 0)
                    return subscriptionsCount;
            }
            return subscriptionsCount;
        }
        public async Task<T> AddSubscriptionOrder(T subscriptionOrder)
        {
            var added = await _subscriptionManagementContext.AddAsync(subscriptionOrder);
            // ReSharper disable once UnusedVariable
            var numberOfRecords = await SaveEntitiesAsync();
            return (T)added.Entity;
        }

        public async Task<OrderSimSubscriptionOrder> GetOrderSimOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.OrderSimSubscriptionOrders
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }
        public async Task<ActivateSimOrder> GetActivateSimOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.ActivateSimOrders
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }

        public async Task<TransferToBusinessSubscriptionOrder> GetTransferToBusinessOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.TransferSubscriptionOrders
                .Include(p => p.PrivateSubscription).ThenInclude(p => p.RealOwner)
                .Include(b => b.BusinessSubscription)
                .Include(a => a.SubscriptionAddOnProducts)
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }

        public async Task<TransferToPrivateSubscriptionOrder> GetTransferToPrivateOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.TransferToPrivateSubscriptionOrders
                .Include(p => p.UserInfo)
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }
        public async Task<NewSubscriptionOrder> GetNewSubscriptionOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.NewSubscriptionOrders
                .Include(p => p.PrivateSubscription)
                .Include(b => b.BusinessSubscription)
                .Include(a => a.SubscriptionAddOnProducts)
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }

        public async Task<ChangeSubscriptionOrder> GetChangeSubscriptionOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.ChangeSubscriptionOrder
               .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }

        public async Task<CancelSubscriptionOrder> GetCancelSubscriptionOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.CancelSubscriptionOrders
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);
            
            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }
    }
}