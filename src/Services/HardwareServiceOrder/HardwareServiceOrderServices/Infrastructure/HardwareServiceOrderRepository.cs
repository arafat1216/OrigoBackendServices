using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
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

        /// <inheritdoc cref="IHardwareServiceOrderRepository.ConfigureLoanPhoneAsync(Guid, string, string, Guid)"/>
        public async Task<CustomerSettings> ConfigureLoanPhoneAsync(
            Guid customerId,
            string loanPhoneNumber,
            string loanPhoneEmail,
            Guid callerId
            )
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

        private async Task ConfigureCustomerServiceProviderAsync(List<int> assetCategoryIds, int providerId, Guid customerId, string? apiUsername, string? apiPassword)
        {
            var serviceProvider = await _hardwareServiceOrderContext.ServiceProviders.FirstOrDefaultAsync(m => m.Id == providerId);

            if (serviceProvider == null)
                throw new ArgumentException($"No service provider exists with ID {providerId}", nameof(providerId));

            foreach (var assetCategoryId in assetCategoryIds)
            {
                var existing = await _hardwareServiceOrderContext.CustomerServiceProviders.FirstOrDefaultAsync(m => m.AssetCategoryId == assetCategoryId && m.ServiceProviderId == providerId && m.CustomerId == customerId);

                if (existing != null)
                {
                    existing.ApiUserName = apiUsername;
                    existing.ApiPassword = apiPassword;
                    _hardwareServiceOrderContext.Entry(existing).State = EntityState.Modified;
                }
                else
                {
                    _hardwareServiceOrderContext.CustomerServiceProviders.Add(new CustomerServiceProvider
                    {
                        AssetCategoryId = assetCategoryId,
                        ServiceProviderId = providerId,
                        CustomerId = customerId,
                        ApiUserName = apiUsername,
                        ApiPassword = apiPassword
                    });
                }
                await _hardwareServiceOrderContext.SaveChangesAsync();
            }
        }

        /// <inheritdoc cref="IHardwareServiceOrderRepository.ConfigureServiceIdAsync(Guid, List{int}, int, string, string, Guid, string,string,)"/>
        public async Task<CustomerSettings> ConfigureServiceIdAsync(
            Guid customerId,
            List<int> assetCategoryIds,
            int providerId,
            string loanPhoneNumber,
            string loanPhoneEmail,
            Guid callerId,
            string? apiUsername = null,
            string? apiPassowrd = null)
        {
            var settings = await GetSettingsAsync(customerId);

            if (settings == null)
            {
                settings = new CustomerSettings(customerId, callerId);
                _hardwareServiceOrderContext.Add(settings);
                await _hardwareServiceOrderContext.SaveChangesAsync();
            }

            await ConfigureCustomerServiceProviderAsync(assetCategoryIds, providerId, customerId, apiUsername, apiPassowrd);

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

        public async Task<HardwareServiceOrder> CreateHardwareServiceOrder(HardwareServiceOrder serviceOrder)
        {
            if (serviceOrder.ServiceProvider == null || serviceOrder.ServiceType == null 
                || serviceOrder.Status == null)
            {
                throw new Exception();
            }
            _hardwareServiceOrderContext.HardwareServiceOrders.Add(serviceOrder);
            //_hardwareServiceOrderContext.ServiceProviders.Add(serviceOrder.ServiceProvider);
            //_hardwareServiceOrderContext.ServiceStatuses.Add(serviceOrder.Status);
            //_hardwareServiceOrderContext.ServiceTypes.Add(serviceOrder.ServiceType);
            await _hardwareServiceOrderContext.SaveChangesAsync();
            var savedHardwareServiceOrder = await _hardwareServiceOrderContext.HardwareServiceOrders
              .FirstOrDefaultAsync(a => a.ExternalId == serviceOrder.ExternalId);
            if (savedHardwareServiceOrder == null)
            {
                throw new Exception();
            }
            return savedHardwareServiceOrder;
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

        public async Task<string?> GetServiceIdAsync(Guid customerId)
        {
            var entity = await _hardwareServiceOrderContext.CustomerServiceProviders.FirstOrDefaultAsync(m => m.CustomerId == customerId);

            return entity?.ApiUserName;
        }

        public async Task<List<CustomerServiceProvider>> GetAllCustomerProvidersAsync()
        {
            return await _hardwareServiceOrderContext.CustomerServiceProviders.ToListAsync();
        }

        /// <inheritdoc cref="IHardwareServiceOrderRepository.GetOrderByServiceProviderOrderIdAsync(string)"/>
        public async Task<HardwareServiceOrder?> GetOrderByServiceProviderOrderIdAsync(string serviceProviderOrderId)
        {
            return await _hardwareServiceOrderContext
                .HardwareServiceOrders
                .Include(m => m.ServiceEvents)
                .FirstOrDefaultAsync(m => m.ServiceProviderOrderId1 == serviceProviderOrderId);
        }

        /// <inheritdoc cref="IHardwareServiceOrderRepository.UpdateCustomerProviderLastUpdateFetchedAsync(CustomerServiceProvider, DateTimeOffset)"/>
        public async Task UpdateCustomerProviderLastUpdateFetchedAsync(CustomerServiceProvider customerServiceProvider, DateTimeOffset lastUpdateFetched)
        {
            customerServiceProvider.LastUpdateFetched = lastUpdateFetched;

            _hardwareServiceOrderContext.Entry(customerServiceProvider).State = EntityState.Modified;

            await _hardwareServiceOrderContext.SaveChangesAsync();
        }

        /// <inheritdoc cref="IHardwareServiceOrderRepository.UpdateServiceEventsAsync(HardwareServiceOrder, IEnumerable{ServiceEvent})"/>
        public async Task UpdateServiceEventsAsync(HardwareServiceOrder order, IEnumerable<ServiceEvent> events)
        {
            foreach (var serviceEvent in events)
            {
                if (!order.ServiceEvents.Any(m => m.ServiceStatusId == serviceEvent.ServiceStatusId && m.Timestamp == serviceEvent.Timestamp))
                {
                    order.AddServiceEvent(serviceEvent);
                    await _hardwareServiceOrderContext.SaveChangesAsync();
                }
            }
        }
    }
}
