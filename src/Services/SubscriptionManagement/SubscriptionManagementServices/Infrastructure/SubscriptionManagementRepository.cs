﻿using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementRepository : ISubscriptionManagementRepository
    {
        private readonly SubscriptionManagementContext _subscriptionContext;

        public SubscriptionManagementRepository(SubscriptionManagementContext subscriptionContext)
        {
            _subscriptionContext = subscriptionContext;
        }

        public async Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(CustomerOperatorAccount customerOperatorAccount)
        {
            var added = await _subscriptionContext.AddAsync(customerOperatorAccount);
            await _subscriptionContext.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<Operator?> GetOperatorAsync(int id)
        {
            return await _subscriptionContext.Operators.FindAsync(id);
        }

        public async Task<IEnumerable<CustomerOperatorAccount>> GetAllCustomerOperatorAccountsAsync(Guid organizationId)
        {
            return await _subscriptionContext.CustomerOperatorAccounts.Where(m => m.OrganizationId == organizationId).ToListAsync();
        }

        public async Task<Operator?> GetOperatorAsync(string name)
        {
            return await _subscriptionContext.Operators.Where(o => o.OperatorName == name).FirstOrDefaultAsync();

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

        public async Task<DataPackage?> GetDatapackageAsync(int id)
        {
            return await _subscriptionContext.DataPackages.FindAsync(id);
        }

        public Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid customerId, string operatorName, string productName, IList<string> datapackages)
        {
            //Needs implementing - Rolf should supply the model for setting
            throw new NotImplementedException();
        }

        public async Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(int id)
        {
            return await _subscriptionContext.CustomerOperatorAccounts.FindAsync(id);
        }

        public async Task<IList<Operator>> GetAllOperatorsForCustomerAsync(Guid organizationId)
        {
            var customerOperators = await _subscriptionContext.CustomerOperatorAccounts
                .Include(m => m.Operator)
                .Where(m => m.OrganizationId == organizationId).Select(m => m.Operator).ToListAsync();

            return customerOperators;
        }

        public async Task<IList<SubscriptionProduct>?> GetOperatorSubscriptionProductForCustomerAsync(Guid customerId, string operatorName)
        {
            var subscriptionProuctsForCustomer = await _subscriptionContext.CustomerSettings
                .Include(m => m.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                        .ThenInclude(m => m.DataPackages)
                .Include(m => m.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                        .ThenInclude(m => m.Operator)
                .Where(c => c.CustomerId == customerId)
                .SelectMany(m => m.CustomerOperatorSettings.Where(m => m.Operator.OperatorName == operatorName))
                .Select(m => m.AvailableSubscriptionProducts)
                        .FirstOrDefaultAsync();

            //var subscription = await _subscriptionContext.CustomerSettings
            //          .Include(x => x.CustomerOperatorSettings.Select(r => r.AvailableSubscriptionProducts.Select(p => p.DataPackages)).ToListAsync();

            //          .Include(x => x.Shifts.Select(r => r.Rate).Select(rt => rt.RuleType))
            //          .Include(x => x.Shifts.Select(r => r.Rate).Select(rt => rt.RateType))
            //          .FirstOrDefaultAsync();

            if (subscriptionProuctsForCustomer == null)
            {
                return null;
            }

            IList<SubscriptionProduct> result = new List<SubscriptionProduct>();
            foreach (var subscriptionProuct in subscriptionProuctsForCustomer)
            {

                result.Add(subscriptionProuct);

            }
            return result;
        }
        public async Task<IList<SubscriptionProduct>?> GetSubscriptionProductForOperatorAsync(string operatorName)
        {
            var operatorSubscriptionProducts = await _subscriptionContext.SubscriptionProducts
                .Where(s => s.Operator.OperatorName == operatorName).ToListAsync();

            return operatorSubscriptionProducts;
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

        public async Task<CustomerSettings> AddCustomerSettingsAsync(CustomerSettings customerSettings)
        {

            var addedCustomerSetting = await _subscriptionContext.CustomerSettings.AddAsync(customerSettings);
            await _subscriptionContext.SaveChangesAsync();
            return addedCustomerSetting.Entity;

        }
        public async Task<CustomerOperatorSettings> AddCustomerOperatorSettingsAsync(CustomerOperatorSettings customerOperatorSettings)
        {

            var addedCustomerOperatorSetting = await _subscriptionContext.CustomerOperatorSettings.AddAsync(customerOperatorSettings);
            await _subscriptionContext.SaveChangesAsync();
            return addedCustomerOperatorSetting.Entity;
        }

        public Task<SubscriptionProduct> DeleteOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        { 
            throw new NotImplementedException();
        }
        public async Task<CustomerOperatorSettings> UpdateCustomerOperatorSettingsAsync(CustomerOperatorSettings customerOperatorSettings)
        {
            var addedCustomerOperatorSetting = _subscriptionContext.CustomerOperatorSettings.Update(customerOperatorSettings);
            await _subscriptionContext.SaveChangesAsync();

            return addedCustomerOperatorSetting.Entity;
        }

        public async Task<TransferSubscriptionOrder> TransferSubscriptionOrderAsync(TransferSubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionContext.AddAsync(subscriptionOrder);
            await _subscriptionContext.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(Guid organizationId, string accountNumber)
        {
            return await _subscriptionContext.CustomerOperatorAccounts
                .Include(m => m.SubscriptionOrders)
                .Include(m => m.CustomerOperatorSettings)
                .Include(m => m.TransferSubscriptionOrders)
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.AccountNumber == accountNumber);
        }

        public async Task DeleteCustomerOperatorAccountAsync(CustomerOperatorAccount customerOperatorAccount)
        {
            if (customerOperatorAccount.CustomerOperatorSettings.Any() || customerOperatorAccount.SubscriptionOrders.Any() || customerOperatorAccount.TransferSubscriptionOrders.Any())
                throw new ArgumentException("This customer operator accounts cannot be deleted because there are other entities related with it.");

            _subscriptionContext.Remove(customerOperatorAccount);
            await _subscriptionContext.SaveChangesAsync();
        }

        public async Task<CustomerSettings> GetCustomerSettingsAsync(Guid customerId)
        {
            var customerSetting = await _subscriptionContext.CustomerSettings.FirstOrDefaultAsync(m => m.CustomerId == customerId);
            return customerSetting;
        }

        public async Task<CustomerSettings> UpdateCustomerSettingsAsync(CustomerSettings customersettings)
        {
            var updatedCustomerSetting = _subscriptionContext.CustomerSettings.Update(customersettings);
            await _subscriptionContext.SaveChangesAsync();

            return updatedCustomerSetting.Entity;
        }

        public async Task<SubscriptionProduct?> GetSubscriptionProductByNameAsync(string subscriptionProductName, int operatorId)
        {
            
            var subscriptionProduct = await _subscriptionContext.SubscriptionProducts.FirstOrDefaultAsync(m => m.SubscriptionName == subscriptionProductName && m.OperatorId == operatorId);
            
            return subscriptionProduct;
        }

        public async Task<IList<Operator>> GetAllOperatorsAsync()
        {
            return await _subscriptionContext.Operators.ToListAsync();
        }
    }
}
