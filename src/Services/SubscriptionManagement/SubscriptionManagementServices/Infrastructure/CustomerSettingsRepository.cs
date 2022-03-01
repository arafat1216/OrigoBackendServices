using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Common.Extensions;
using Common.Logging;
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

        public async Task<CustomerSettings?> GetCustomerSettingsAsync(Guid organizationId)
        {
            return await _subscriptionManagementContext.CustomerSettings
                .Include(cs => cs.CustomerOperatorSettings)
                .ThenInclude(o => o.Operator)
                .Include(cs => cs.CustomerOperatorSettings)
                .ThenInclude(o => o.CustomerOperatorAccounts)
                .Include(cs => cs.CustomerOperatorSettings)
                .ThenInclude(op => op.AvailableSubscriptionProducts)
                .ThenInclude(d => d.GlobalSubscriptionProduct)
                .Include(cs => cs.CustomerOperatorSettings)
                .ThenInclude(op => op.AvailableSubscriptionProducts)
                .ThenInclude(d => d.DataPackages)
                .Include(cs => cs.CustomerReferenceFields).AsSplitQuery()
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

        public async Task<IReadOnlyCollection<CustomerReferenceField>> GetCustomerReferenceFieldsAsync(Guid organizationId)
        {
            var customerSetting = await _subscriptionManagementContext.CustomerSettings
                .Include(cs => cs.CustomerReferenceFields)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.CustomerId == organizationId);
            if (customerSetting == null || customerSetting.CustomerReferenceFields == null)
            {
                return new ReadOnlyCollection<CustomerReferenceField>(new List<CustomerReferenceField>());
            }
            return customerSetting.CustomerReferenceFields.ToImmutableList();
        }

        public async Task DeleteOperatorSubscriptionProductForCustomerAsync(CustomerSubscriptionProduct customerSubscriptionProduct)
        {
            if (customerSubscriptionProduct.GlobalSubscriptionProduct == null)
            {
                _subscriptionManagementContext.DataPackages.RemoveRange(customerSubscriptionProduct.DataPackages);
            }

            _subscriptionManagementContext.Entry(customerSubscriptionProduct).State = EntityState.Deleted;
            await _subscriptionManagementContext.SaveChangesAsync();
        }

        public async Task<SubscriptionProduct?> GetSubscriptionProductByNameAsync(string subscriptionProductName, int operatorId)
        {

            var subscriptionProduct = await _subscriptionManagementContext.SubscriptionProducts
                .Include(m => m.DataPackages)
                .FirstOrDefaultAsync(m => m.SubscriptionName == subscriptionProductName && m.OperatorId == operatorId);

            return subscriptionProduct;
        }

        public async Task<IList<SubscriptionProduct>?> GetAllOperatorSubscriptionProducts()
        {
            return await _subscriptionManagementContext.SubscriptionProducts
                .Include(o => o.Operator)
                .Include(d => d.DataPackages)
                .AsSplitQuery()
                .ToListAsync();
        }


        public async Task DeleteCustomerReferenceFieldForCustomerAsync(CustomerReferenceField customerReferenceField)
        {
            _subscriptionManagementContext.Entry(customerReferenceField).State = EntityState.Deleted;
            await _subscriptionManagementContext.SaveChangesAsync();
        }

        public async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            int numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_subscriptionManagementContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                foreach (var @event in _subscriptionManagementContext.GetDomainEventsAsync())
                {
                    await _functionalEventLogService.SaveEventAsync(@event, _subscriptionManagementContext.Database.CurrentTransaction);
                }
                await _subscriptionManagementContext.SaveChangesAsync(cancellationToken);
                numberOfRecordsSaved = await _subscriptionManagementContext.SaveChangesAsync(cancellationToken);
                await _mediator.DispatchDomainEventsAsync(_subscriptionManagementContext);
            });
            return numberOfRecordsSaved;
        }
    }
}
