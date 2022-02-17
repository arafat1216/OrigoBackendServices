using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class CustomerSettingsRepository : ICustomerSettingsRepository
    {
        public CustomerSettingsRepository(SubscriptionManagementContext subscriptionManagementContext)
        {
            SubscriptionManagementContext = subscriptionManagementContext;
        }

        public SubscriptionManagementContext SubscriptionManagementContext { get; }

        public async Task AddCustomerSettingsAsync(Guid customerId, IList<int> operators)
        {
            var customerSettings = await SubscriptionManagementContext.CustomerSettings.Include(m => m.CustomerOperatorSettings)
                .FirstOrDefaultAsync(m => m.CustomerId == customerId) ?? new CustomerSettings(customerId);

            var customerOperatorSettingsList = new List<CustomerOperatorSettings>();

            foreach (var id in operators)
            {
                var @operator = await SubscriptionManagementContext.Operators.FindAsync(id);

                if (@operator == null)
                    throw new ArgumentException($"No operator exists with ID {id}");

                var customerOperatorAccounts = await SubscriptionManagementContext.CustomerOperatorAccounts.Where(m => m.OrganizationId == customerId).ToListAsync();

                var customerOperatorSettings = new CustomerOperatorSettings(@operator, customerOperatorAccounts);
                customerOperatorSettingsList.Add(customerOperatorSettings);
            }

            customerSettings.AddCustomerOperatorSettings(customerOperatorSettingsList);

            if (customerSettings.Id == 0)
            {
                SubscriptionManagementContext.CustomerSettings.Add(customerSettings);
                await SubscriptionManagementContext.SaveChangesAsync();
            }
            else
            {
                SubscriptionManagementContext.Entry(customerSettings).State = EntityState.Modified;
                await SubscriptionManagementContext.SaveChangesAsync();
            }
        }

        public async Task DeleteOperatorForCustomerAsync(Guid customerId, int operatorId)
        {
            var customerSettings = await SubscriptionManagementContext.CustomerSettings.Include(m => m.CustomerOperatorSettings).ThenInclude(e => e.Operator).FirstOrDefaultAsync(m => m.CustomerId == customerId);

            if (customerSettings == null)
                throw new ArgumentException("Settings does not exist for this customer");

            customerSettings.RemoveCustomerOperatorSettings(operatorId);

            SubscriptionManagementContext.Entry(customerSettings).State = EntityState.Modified;

            await SubscriptionManagementContext.SaveChangesAsync();
        }
    }
}
