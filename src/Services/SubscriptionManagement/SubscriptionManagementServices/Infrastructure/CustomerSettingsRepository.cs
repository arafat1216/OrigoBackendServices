using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class CustomerSettingsRepository : ICustomerSettingsRepository
    {
        private readonly SubscriptionManagementContext _subscriptionManagementContext;

        public CustomerSettingsRepository(SubscriptionManagementContext subscriptionManagementContext)
        {
            _subscriptionManagementContext = subscriptionManagementContext;
        }

        public async Task<CustomerSettings?> GetCustomerSettingsAsync(Guid customerId)
        {
            return await _subscriptionManagementContext.CustomerSettings
                .Include(cs => cs.CustomerOperatorSettings)
                .ThenInclude(o => o.Operator)
                .Include(cs => cs.CustomerOperatorSettings)
                .ThenInclude(op => op.AvailableSubscriptionProducts).AsSplitQuery()
                .FirstOrDefaultAsync(m => m.CustomerId == customerId);
        }


        public async Task<CustomerSettings> AddCustomerSettingsAsync(CustomerSettings customerSettings)
        {

            var addedCustomerSetting = await _subscriptionManagementContext.CustomerSettings.AddAsync(customerSettings);
            await _subscriptionManagementContext.SaveChangesAsync();
            return addedCustomerSetting.Entity;

        }

        public async Task<CustomerSettings> UpdateCustomerSettingsAsync(CustomerSettings customerSettings)
        {
            var updatedCustomerSetting = _subscriptionManagementContext.CustomerSettings.Update(customerSettings);
            await _subscriptionManagementContext.SaveChangesAsync();

            return updatedCustomerSetting.Entity;
        }

        public async Task AddCustomerOperatorSettingsAsync(Guid customerId, IList<int> operators)
        {
            var customerSettings = await _subscriptionManagementContext.CustomerSettings.Include(m => m.CustomerOperatorSettings)
                .FirstOrDefaultAsync(m => m.CustomerId == customerId) ?? new CustomerSettings(customerId);

            var customerOperatorSettingsList = new List<CustomerOperatorSettings>();

            foreach (var id in operators)
            {
                var @operator = await _subscriptionManagementContext.Operators.FindAsync(id);

                if (@operator == null)
                    throw new ArgumentException($"No operator exists with ID {id}");

                var customerOperatorAccounts = await _subscriptionManagementContext.CustomerOperatorAccounts.Where(m => m.OrganizationId == customerId).ToListAsync();

                var customerOperatorSettings = new CustomerOperatorSettings(@operator, customerOperatorAccounts);
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

        public async Task DeleteOperatorForCustomerAsync(Guid customerId, int operatorId)
        {
            var customerSettings = await _subscriptionManagementContext.CustomerSettings.Include(m => m.CustomerOperatorSettings).ThenInclude(e => e.Operator).FirstOrDefaultAsync(m => m.CustomerId == customerId);

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
    }
}
