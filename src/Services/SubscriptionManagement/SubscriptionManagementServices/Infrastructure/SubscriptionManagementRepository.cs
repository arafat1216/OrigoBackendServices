﻿using Common.Extensions;
using Common.Logging;
using Common.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementRepository : ISubscriptionManagementRepository
    {
        private readonly SubscriptionManagementContext _subscriptionContext;
        private readonly IFunctionalEventLogService _functionalEventLogService;
        private readonly IMediator _mediator;


        public SubscriptionManagementRepository(SubscriptionManagementContext subscriptionContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator)
        {
            _subscriptionContext = subscriptionContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
        }

        public async Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(CustomerOperatorAccount customerOperatorAccount)
        {
            var existing = await _subscriptionContext.CustomerOperatorAccounts.Include(m => m.Operator).FirstOrDefaultAsync(m => m.OperatorId == customerOperatorAccount.OperatorId && m.OrganizationId == customerOperatorAccount.OrganizationId);
            if (existing != null)
                return existing;

            var @operator = await _subscriptionContext.Operators.FindAsync(customerOperatorAccount.OperatorId);

            if (@operator == null)
            {
                throw new ArgumentException($"No operator exists with ID {customerOperatorAccount.OperatorId}");
            }

            customerOperatorAccount.Operator = @operator;
            var added = await _subscriptionContext.AddAsync(customerOperatorAccount);
            await _subscriptionContext.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<IEnumerable<CustomerOperatorAccount>> GetAllCustomerOperatorAccountsAsync(Guid organizationId)
        {
            return await _subscriptionContext.CustomerOperatorAccounts.Where(m => m.OrganizationId == organizationId).ToListAsync();
        }
        public async Task<SubscriptionOrder> AddSubscriptionOrderAsync(SubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionContext.AddAsync(subscriptionOrder);
            await _subscriptionContext.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<SubscriptionProduct?> GetSubscriptionProductAsync(int id)
        {
            return await _subscriptionContext.SubscriptionProducts.FindAsync(id);
        }

        public async Task<DataPackage?> GetDataPackageAsync(int id)
        {
            return await _subscriptionContext.DataPackages.FindAsync(id);
        }

        public async Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(int id)
        {
            return await _subscriptionContext.CustomerOperatorAccounts.Include(m => m.Operator).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IList<CustomerSubscriptionProduct>?> GetAllCustomerSubscriptionProductsAsync(Guid customerId)
        {
            var subscriptionProductsForCustomer = await _subscriptionContext.CustomerSettings
                .Include(m => m.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                        .ThenInclude(m => m.DataPackages)
                .Include(m => m.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                        .ThenInclude(m => m.Operator)
                .Include(m => m.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                        .ThenInclude(m => m.GlobalSubscriptionProduct)
                .Where(c => c.CustomerId == customerId)
                .AsSplitQuery()
                .SelectMany(m => m.CustomerOperatorSettings)
                .Select(m => m.AvailableSubscriptionProducts).Where(a => a.Count() != 0)
                .ToListAsync();


            if (subscriptionProductsForCustomer == null)
            {
                return null;
            }

            List<CustomerSubscriptionProduct> result = new();
            foreach (var customerSubscriptionProduct in subscriptionProductsForCustomer)
            {
                foreach (var product in customerSubscriptionProduct)
                {
                    result.Add(product);
                }

            }
            return result;
        }
        public async Task<CustomerSubscriptionProduct?> GetAvailableSubscriptionProductForCustomerbySubscriptionIdAsync(Guid customerId, int subscriptionId)
        {

            var subscriptionProductsForCustomer = await GetAllCustomerSubscriptionProductsAsync(customerId);
            if (subscriptionProductsForCustomer == null)
            {
                return null;
            }
            foreach (var product in subscriptionProductsForCustomer)
            {
                if (product.Id == subscriptionId)
                {
                    return product;
                }
            }

            return null;
        }

        public async Task<CustomerOperatorSettings> GetCustomerOperatorSettings(Guid customerId, string operatorName)
        {

            var customerOperatorSettings = await _subscriptionContext.CustomerSettings
                .Include(e => e.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                .Include(e => e.CustomerOperatorSettings)
                    .ThenInclude(m => m.CustomerOperatorAccounts)
                .Include(e => e.CustomerOperatorSettings)
                    .ThenInclude(m => m.Operator)
                                     .Where(c => c.CustomerId == customerId).Select(e => e.CustomerOperatorSettings.Where(e => e.Operator.OperatorName == operatorName).FirstOrDefault())
                                     .FirstOrDefaultAsync();

            return customerOperatorSettings;
        }

        public async Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(SubscriptionProduct subscriptionProduct)
        {

            var addedSubscriptionProduct = await _subscriptionContext.SubscriptionProducts.AddAsync(subscriptionProduct);
            await _subscriptionContext.SaveChangesAsync();
            return addedSubscriptionProduct.Entity;
        }

        public async Task<CustomerOperatorSettings> AddCustomerOperatorSettingsAsync(CustomerOperatorSettings customerOperatorSettings)
        {

            var addedCustomerOperatorSetting = await _subscriptionContext.CustomerOperatorSettings.AddAsync(customerOperatorSettings);
            await _subscriptionContext.SaveChangesAsync();
            return addedCustomerOperatorSetting.Entity;
        }

        public async Task<TransferToBusinessSubscriptionOrder> TransferToBusinessSubscriptionOrderAsync(TransferToBusinessSubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionContext.AddAsync(subscriptionOrder);
            await _subscriptionContext.SaveChangesAsync();
            return added.Entity;
        }

        public async Task DeleteCustomerOperatorAccountAsync(CustomerOperatorAccount customerOperatorAccount)
        {
            if (customerOperatorAccount.SubscriptionOrders.Any() || customerOperatorAccount.TransferToBusinessSubscriptionOrders.Any())
                throw new ArgumentException("This customer operator accounts cannot be deleted because there are other entities related with it.");

            _subscriptionContext.Remove(customerOperatorAccount);
            await _subscriptionContext.SaveChangesAsync();
        }

        public async Task<DataPackage?> GetDataPackageAsync(string dataPackageName)
        {
            return await _subscriptionContext.DataPackages.FirstOrDefaultAsync(m => m.DataPackageName == dataPackageName);
        }

        public async Task<CustomerSubscriptionProduct?> GetCustomerSubscriptionProductAsync(int id)
        {
            return await _subscriptionContext.CustomerSubscriptionProducts.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(Guid organizationId, int id)
        {
            return await _subscriptionContext.CustomerOperatorAccounts.Include(m => m.Operator).FirstOrDefaultAsync(m => m.Id == id && m.OrganizationId == organizationId);
        }
    

        public async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            int numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_subscriptionContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                foreach (var @event in _subscriptionContext.GetDomainEventsAsync())
                {
                    await _functionalEventLogService.SaveEventAsync(@event, _subscriptionContext.Database.CurrentTransaction);
                }
                await _subscriptionContext.SaveChangesAsync(cancellationToken);
                numberOfRecordsSaved = await _subscriptionContext.SaveChangesAsync(cancellationToken);
                await _mediator.DispatchDomainEventsAsync(_subscriptionContext);
            });
            return numberOfRecordsSaved;
        }

        public async Task<TransferToPrivateSubscriptionOrder> TransferToPrivateSubscriptionOrderAsync(TransferToPrivateSubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionContext.AddAsync(subscriptionOrder);
            await _subscriptionContext.SaveChangesAsync();
            return added.Entity;
        }
    }
}
