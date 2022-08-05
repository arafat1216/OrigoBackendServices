using Common.Extensions;
using Common.Interfaces;
using HardwareServiceOrderServices.Conmodo.ApiModels;
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


        /// <inheritdoc/>
        public async Task<CustomerSettings> ConfigureLoanPhoneAsync(
            Guid customerId,
            string loanPhoneNumber,
            string loanPhoneEmail,
            bool providesLoanDevice,
            Guid callerId
            )
        {
            var settings = await GetSettingsAsync(customerId);

            if (settings == null)
            {
                var newSettings = new CustomerSettings(customerId, loanPhoneNumber, loanPhoneEmail, providesLoanDevice, callerId);
                _hardwareServiceOrderContext.Add(newSettings);
                await _hardwareServiceOrderContext.SaveChangesAsync();
                return newSettings;
            }

            // If providesLoanDevice is false, LoanDevicePhoneNumber & LoanDeviceEmail must be set to empty
            settings.LoanDevicePhoneNumber = providesLoanDevice ? loanPhoneNumber : "";
            settings.LoanDeviceEmail = providesLoanDevice ? loanPhoneEmail : "";
            settings.ProvidesLoanDevice = providesLoanDevice;

            _hardwareServiceOrderContext.Entry(settings).State = EntityState.Modified;

            await _hardwareServiceOrderContext.SaveChangesAsync();

            return settings;
        }


        /// <inheritdoc/>
        public async Task<string?> ConfigureCustomerServiceProviderAsync(int providerId, Guid customerId, string? apiUsername, string? apiPassword)
        {
            var serviceProvider = await _hardwareServiceOrderContext.ServiceProviders.FirstOrDefaultAsync(m => m.Id == providerId);

            if (serviceProvider == null)
                throw new ArgumentException($"No service provider exists with ID {providerId}", nameof(providerId));

            var existing = await GetCustomerServiceProviderAsync(customerId, providerId);

            if (existing == null)
            {
                _hardwareServiceOrderContext.CustomerServiceProviders.Add(new CustomerServiceProvider
                {
                    ServiceProviderId = providerId,
                    CustomerId = customerId,
                    ApiUserName = apiUsername,
                    ApiPassword = apiPassword
                });

                await _hardwareServiceOrderContext.SaveChangesAsync();

                return apiUsername;
            }

            existing.ApiUserName = apiUsername;
            existing.ApiPassword = apiPassword;

            _hardwareServiceOrderContext.Entry(existing).State = EntityState.Modified;

            await _hardwareServiceOrderContext.SaveChangesAsync();

            return existing.ApiUserName;
        }


        /// <inheritdoc/>
        public async Task<CustomerSettings> ConfigureCustomerSettingsAsync(Guid customerId, Guid callerId)
        {
            var settings = await GetSettingsAsync(customerId);

            if (settings == null)
            {
                settings = new CustomerSettings(customerId, callerId);
                _hardwareServiceOrderContext.Add(settings);
                await _hardwareServiceOrderContext.SaveChangesAsync();
            }

            return settings;
        }


        /// <inheritdoc/>
        public async Task<PagedModel<HardwareServiceOrder>> GetAllOrdersAsync(Guid customerId, Guid? userId, bool activeOnly, int page, int limit, CancellationToken cancellationToken)
        {
            var orders = _hardwareServiceOrderContext.HardwareServiceOrders
                .Where(m => m.CustomerId == customerId);

            if (userId != null)
                orders = orders.Where(m => m.Owner.UserId == userId);

            if (activeOnly)
                orders = orders.Where(m => m.StatusId == (int)ServiceStatusEnum.Registered || m.StatusId == (int)ServiceStatusEnum.RegisteredInTransit ||
                m.StatusId == (int)ServiceStatusEnum.RegisteredUserActionNeeded || m.StatusId == (int)ServiceStatusEnum.Ongoing ||
                m.StatusId == (int)ServiceStatusEnum.OngoingInTransit || m.StatusId == (int)ServiceStatusEnum.OngoingReadyForPickup ||
                m.StatusId == (int)ServiceStatusEnum.OngoingUserActionNeeded || m.StatusId == (int)ServiceStatusEnum.Unknown);

            return await orders.OrderByDescending(m => m.DateCreated).PaginateAsync(page, limit, cancellationToken);

        }


        /// <inheritdoc/>
        public async Task<HardwareServiceOrder?> GetOrderAsync(Guid orderId)
        {
            var order = await _hardwareServiceOrderContext.HardwareServiceOrders.FirstOrDefaultAsync(m => m.ExternalId == orderId);
            return order;
        }


        /// <inheritdoc/>
        public async Task<HardwareServiceOrder?> GetOrderAsync(Guid customerId, Guid orderId)
        {
            return await _hardwareServiceOrderContext.HardwareServiceOrders.FirstOrDefaultAsync(m => m.ExternalId == orderId && m.CustomerId == customerId);
        }


        /// <inheritdoc/>
        public async Task<CustomerSettings?> GetSettingsAsync(Guid customerId)
        {
            return await _hardwareServiceOrderContext.CustomerSettings.FirstOrDefaultAsync(m => m.CustomerId == customerId);
        }


        /// <inheritdoc/>
        public async Task<HardwareServiceOrder> CreateHardwareServiceOrder(HardwareServiceOrder serviceOrder)
        {
            var serviceType = await GetServiceTypeAsync((int)ServiceTypeEnum.SUR) ?? new ServiceType { Id = (int)ServiceTypeEnum.SUR };
            var serviceStatus = await GetServiceStatusAsync((int)ServiceStatusEnum.Registered);
            var serviceProvider = await GetCustomerServiceProviderAsync(serviceOrder.CustomerId, (int)ServiceProviderEnum.ConmodoNo);

            if (serviceProvider == null || serviceType == null || serviceStatus == null)
            {
                throw new Exception();
            }

            _hardwareServiceOrderContext.HardwareServiceOrders.Add(serviceOrder);

            await _hardwareServiceOrderContext.SaveChangesAsync();

            var savedHardwareServiceOrder = await _hardwareServiceOrderContext.HardwareServiceOrders.FirstOrDefaultAsync(a => a.ExternalId == serviceOrder.ExternalId);

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


        /// <inheritdoc/>
        public async Task<string?> GetServiceIdAsync(Guid customerId)
        {
            var entity = await _hardwareServiceOrderContext.CustomerServiceProviders.FirstOrDefaultAsync(m => m.CustomerId == customerId);

            return entity?.ApiUserName;
        }


        /// <inheritdoc/>
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


        /// <inheritdoc/>
        public async Task UpdateCustomerProviderLastUpdateFetchedAsync(CustomerServiceProvider customerServiceProvider, DateTimeOffset lastUpdateFetched)
        {
            customerServiceProvider.LastUpdateFetched = lastUpdateFetched;

            _hardwareServiceOrderContext.Entry(customerServiceProvider).State = EntityState.Modified;

            await _hardwareServiceOrderContext.SaveChangesAsync();
        }


        /// <inheritdoc/>
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


        /// <inheritdoc/>
        public async Task<ServiceType?> GetServiceTypeAsync(int id)
        {
            return await _hardwareServiceOrderContext.ServiceTypes.FirstOrDefaultAsync(m => m.Id == id);
        }


        /// <inheritdoc/>
        public async Task<ServiceStatus?> GetServiceStatusAsync(int id)
        {
            return await _hardwareServiceOrderContext.ServiceStatuses.FirstOrDefaultAsync(m => m.Id == id);
        }


        /// <inheritdoc/>
        public async Task<CustomerServiceProvider?> GetCustomerServiceProviderAsync(Guid customerId, int providerId)
        {
            return await _hardwareServiceOrderContext.CustomerServiceProviders.FirstOrDefaultAsync(m => m.CustomerId == customerId && m.ServiceProviderId == providerId);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ServiceProvider>> GetAllServiceProvidersAsync(bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons, bool asNoTracking)
        {
            IQueryable<ServiceProvider> query = _hardwareServiceOrderContext.ServiceProviders;

            if (includeSupportedServiceTypes)
                query = query.Include(e => e.SupportedServiceTypes);

            if (includeOfferedServiceOrderAddons)
                query = query.Include(e => e.OfferedServiceOrderAddons);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
    }
}
