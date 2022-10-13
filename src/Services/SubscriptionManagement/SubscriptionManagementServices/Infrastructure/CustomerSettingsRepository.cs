using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Common.Extensions;
using Common.Logging;
using Common.Seedwork;
using Common.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class CustomerSettingsRepository : ICustomerSettingsRepository
    {
        private readonly SubscriptionManagementContext _subscriptionManagementContext;
        private readonly IFunctionalEventLogService _functionalEventLogService;
        private readonly IMediator _mediator;

        public CustomerSettingsRepository(SubscriptionManagementContext subscriptionManagementContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator)
        {
            _subscriptionManagementContext = subscriptionManagementContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
        }

        public async Task<CustomerSettings?> GetCustomerSettingsAsync(Guid organizationId, bool asNoTracking, bool includeCustomerOperatorSettings = false, bool includeOperator = false,
            bool includeStandardPrivateSubscriptionProduct = false, bool includeCustomerOperatorAccounts = false, bool includeAvailableSubscriptionProducts = false,
            bool includeGlobalSubscriptionProduct = false, bool includeDataPackages = false, bool includeCustomerReferenceFields = false)
        {
            IQueryable<CustomerSettings> query = _subscriptionManagementContext.Set<CustomerSettings>();

            if (includeCustomerOperatorSettings)
                query = query.Include(cs => cs.CustomerOperatorSettings);

            if (includeOperator)
                query = query.Include(cs => cs.CustomerOperatorSettings)
                    .ThenInclude(o => o.Operator);

            if (includeStandardPrivateSubscriptionProduct)
                query = query.Include(cs => cs.CustomerOperatorSettings)
                    .ThenInclude(o => o.StandardPrivateSubscriptionProduct);

            if (includeCustomerOperatorAccounts)
                query = query.Include(cs => cs.CustomerOperatorSettings)
                    .ThenInclude(o => o.CustomerOperatorAccounts);

            if (includeAvailableSubscriptionProducts)
                query = query.Include(cs => cs.CustomerOperatorSettings)
                    .ThenInclude(o => o.AvailableSubscriptionProducts);

            if (includeGlobalSubscriptionProduct)
                query = query.Include(cs => cs.CustomerOperatorSettings)
                    .ThenInclude(o => o.AvailableSubscriptionProducts)
                    .ThenInclude(o => o.GlobalSubscriptionProduct);

            if (includeDataPackages)
                query = query.Include(cs => cs.CustomerOperatorSettings)
                    .ThenInclude(o => o.AvailableSubscriptionProducts)
                    .ThenInclude(o => o.DataPackages);

            if (includeCustomerReferenceFields)
                query = query.Include(cs => cs.CustomerReferenceFields);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.AsSplitQuery()
                .FirstOrDefaultAsync(m => m.CustomerId == organizationId);
        }


        public async Task<CustomerSettings> AddCustomerSettingsAsync(CustomerSettings customerSettings)
        {

            var addedCustomerSetting = await _subscriptionManagementContext.CustomerSettings.AddAsync(customerSettings);
            await SaveEntitiesAsync();
            return addedCustomerSetting.Entity;

        }

        public async Task<CustomerSettings> UpdateCustomerSettingsAsync(CustomerSettings customerSettings)
        {
            var updatedCustomerSetting = _subscriptionManagementContext.CustomerSettings.Update(customerSettings);
            await SaveEntitiesAsync();
            return updatedCustomerSetting.Entity;
        }

        public async Task AddCustomerOperatorSettingsAsync(Guid organizationId, IList<int> operators, Guid callerId)
        {

            var customerSettings = await _subscriptionManagementContext.CustomerSettings.Include(m => m.CustomerOperatorSettings)
                .FirstOrDefaultAsync(m => m.CustomerId == organizationId) ?? new CustomerSettings(organizationId,callerId);

            var customerOperatorSettingsList = new List<CustomerOperatorSettings>();

            foreach (var id in operators)
            {
                var @operator = await _subscriptionManagementContext.Operators.FindAsync(id);

                if (@operator == null)
                    throw new ArgumentException($"No operator exists with ID {id}");

                var customerOperatorAccounts = await _subscriptionManagementContext.CustomerOperatorAccounts.Where(m => m.OrganizationId == organizationId).ToListAsync();

                var customerOperatorSettings = new CustomerOperatorSettings(@operator, customerOperatorAccounts, callerId);
                customerOperatorSettingsList.Add(customerOperatorSettings);
            }

            customerSettings.AddCustomerOperatorSettings(customerOperatorSettingsList);

            if (customerSettings.Id == 0)
            {
                _subscriptionManagementContext.CustomerSettings.Add(customerSettings);
                await _subscriptionManagementContext.SaveChangesAsync();
            }
            else
            {
                _subscriptionManagementContext.Entry(customerSettings).State = EntityState.Modified;
                await _subscriptionManagementContext.SaveChangesAsync();
            }
        }

        public async Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId)
        {
            var customerSettings = await _subscriptionManagementContext.CustomerSettings.Include(m => m.CustomerOperatorSettings).ThenInclude(e => e.Operator).FirstOrDefaultAsync(m => m.CustomerId == organizationId);

            if (customerSettings == null)
                throw new ArgumentException("Settings does not exist for this customer");

            customerSettings.RemoveCustomerOperatorSettings(operatorId);

            _subscriptionManagementContext.Entry(customerSettings).State = EntityState.Modified;

            await _subscriptionManagementContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<CustomerReferenceField>> GetCustomerReferenceFieldsAsync(Guid organizationId, bool asNoTracking)
        {
            IQueryable<CustomerSettings> query = _subscriptionManagementContext.Set<CustomerSettings>();

            query = query.Include(o => o.CustomerReferenceFields).AsSplitQuery();

            if (asNoTracking)
                query = query.AsNoTracking();

            var customerSetting = await query
                .FirstOrDefaultAsync(m => m.CustomerId == organizationId);
            if (customerSetting == null || customerSetting.CustomerReferenceFields == null)
            {
                return new ReadOnlyCollection<CustomerReferenceField>(new List<CustomerReferenceField>());
            }
            return customerSetting.CustomerReferenceFields.ToImmutableList();
        }
        
        public async Task<SubscriptionProduct?> GetSubscriptionProductByNameAsync(string subscriptionProductName, int operatorId)
        {

            var subscriptionProduct = await _subscriptionManagementContext.SubscriptionProducts
                .Include(m => m.DataPackages)
                .FirstOrDefaultAsync(m => m.SubscriptionName == subscriptionProductName && m.OperatorId == operatorId);

            return subscriptionProduct;
        }

        public async Task<IList<SubscriptionProduct>?> GetAllOperatorSubscriptionProducts(bool asNoTracking)
        {
            IQueryable<SubscriptionProduct> query = _subscriptionManagementContext.Set<SubscriptionProduct>();

            query = query.Include(o => o.Operator).Include(d => d.DataPackages).AsSplitQuery();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }


        public async Task DeleteCustomerReferenceFieldForCustomerAsync(CustomerReferenceField customerReferenceField)
        {
            _subscriptionManagementContext.Entry(customerReferenceField).State = EntityState.Deleted;
            await _subscriptionManagementContext.SaveChangesAsync();
        }

        private async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            int numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_subscriptionManagementContext).ExecuteAsync(async () =>
            {
                var editedEntities = _subscriptionManagementContext.ChangeTracker.Entries()
                    .Where(E => E.State == EntityState.Modified)
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
    }
}
