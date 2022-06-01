using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareServiceOrderServices.Infrastructure
{
    public class HardwareServiceOrderRepository : IHardwareServiceOrderRepository
    {
        private readonly HardwareServiceOrderContext _hardwareServiceOrderContext;
        public HardwareServiceOrderRepository(HardwareServiceOrderContext hardwareServiceOrderContext)
        {
            _hardwareServiceOrderContext = hardwareServiceOrderContext;
        }

        public async Task<CustomerSettings> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmail, Guid callerId)
        {
            var settings = await GetSettingsAsync(customerId);
            if (settings == null)
            {
                var newSettings = new CustomerSettings(customerId, loanPhoneNumber, loanPhoneEmail, callerId);
                _hardwareServiceOrderContext.Add(newSettings);
                await _hardwareServiceOrderContext.SaveChangesAsync();
                return newSettings;
            }

            settings.LoanDevicePhoneNumber = loanPhoneNumber;
            settings.LoanDeviceEmail = loanPhoneEmail;
            _hardwareServiceOrderContext.Entry(settings).State = EntityState.Modified;
            await _hardwareServiceOrderContext.SaveChangesAsync();
            return settings;
        }

        public async Task<CustomerSettings> ConfigureServiceIdAsync(Guid customerId, string serviceId, Guid callerId)
        {
            var settings = await GetSettingsAsync(customerId);
            if (settings == null)
            {
                var newSettings = new CustomerSettings(customerId, serviceId, callerId);
                _hardwareServiceOrderContext.Add(newSettings);
                await _hardwareServiceOrderContext.SaveChangesAsync();
                return newSettings;
            }

            settings.ServiceId = serviceId;
            _hardwareServiceOrderContext.Entry(settings).State = EntityState.Modified;
            await _hardwareServiceOrderContext.SaveChangesAsync();
            return settings;
        }

        /// <summary>
        /// Get all status regardless of customer
        /// </summary>
        /// <param name="olderThan">Must return orders those are older than specified date</param>
        /// <param name="statusIds">The value-mappings can be retrieved from <see cref="ServiceStatusEnum"/>.</param>
        /// <returns></returns>
        public async Task<IEnumerable<HardwareServiceOrder>> GetAllOrdersAsync(DateTime? olderThan = null, List<int>? statusIds = null)
        {
            var orders = _hardwareServiceOrderContext.HardwareServiceOrders.Include(m => m.Status).AsQueryable();

            if (olderThan != null)
                orders = orders.Where(m => m.CreatedDate <= olderThan);

            if (statusIds != null)
                orders = orders.Where(m => statusIds.Contains(m.Status.Id));

            return await orders.ToListAsync();
        }

        public Task<List<HardwareServiceOrder>> GetAllOrdersAsync(Guid customerId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="GetOrderByIdAsync(Guid)"/>
        public async Task<HardwareServiceOrder> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _hardwareServiceOrderContext.HardwareServiceOrders.FirstOrDefaultAsync(m => m.ExternalId == orderId);
            return order;
        }

        public async Task<CustomerSettings> GetSettingsAsync(Guid customerId)
        {
            return await _hardwareServiceOrderContext.CustomerSettings.FirstOrDefaultAsync(m => m.CustomerId == customerId);
        }

        /// <summary>
        /// Update the status of a service order
        /// </summary>
        /// <param name="orderId">Order Identifier</param>
        /// <param name="newStatus">New status <see cref="ServiceStatusEnum"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws exception when orderId is invalid</exception>
        public async Task<HardwareServiceOrder> UpdateOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus)
        {
            var order = await _hardwareServiceOrderContext.HardwareServiceOrders.FirstOrDefaultAsync(m => m.ExternalId == orderId);

            if (order == null)
                throw new ArgumentException($"No service order exists with ID {orderId}", nameof(orderId));

            var status = await _hardwareServiceOrderContext.ServiceStatuses.FindAsync((int)newStatus);

            if (status == null)
                throw new ArgumentException("New status is invalid");

            order.Status = status;

            _hardwareServiceOrderContext.Entry(order).State = EntityState.Modified;

            await _hardwareServiceOrderContext.SaveChangesAsync();

            return order;
        }
    }
}
