using Common.Extensions;
using Common.Interfaces;
using Common.Seedwork;
using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;

namespace HardwareServiceOrderServices.Infrastructure
{
    public class HardwareServiceOrderRepository : IHardwareServiceOrderRepository
    {
        private readonly HardwareServiceOrderContext _hardwareServiceOrderContext;


        public HardwareServiceOrderRepository(HardwareServiceOrderContext hardwareServiceOrderContext)
        {
            _hardwareServiceOrderContext = hardwareServiceOrderContext;
        }


        /// <summary>
        ///     Applies a custom delete to entities that's registered as a <see cref="DbSet{TEntity}"/>.
        ///     
        ///     <para>
        ///     This custom deletes first updates the <paramref name="entityToBeDeleted"/> with the related <see cref="Auditable"/>-properties 
        ///     that's used for tracking deletes. Once this information is persisted, it will delete the entry from the database. </para>
        ///     
        ///     <para>
        ///     This is intended to be used with temporal tables, where the entity is deleted from the main-table instead of just being
        ///     soft-deleted. </para>
        /// </summary>
        /// <typeparam name="TEntity"> The entities datatype. </typeparam>
        /// <param name="entityToBeDeleted"> The entity that should be deleted. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        public async Task Delete<TEntity>(TEntity entityToBeDeleted) where TEntity : Auditable, IDbSetEntity
        {
            // Fetch the private-properties we need to use reflections on
            var deletedByProperty = typeof(Auditable).GetProperty(nameof(Auditable.DeletedBy));
            var dateDeletedProperty = typeof(Auditable).GetProperty(nameof(Auditable.DateDeleted));
            var isDeletedProperty = typeof(Auditable).GetProperty(nameof(Auditable.IsDeleted));

            // Update the private setter values
            deletedByProperty?.SetValue(entityToBeDeleted, _hardwareServiceOrderContext.AuthenticatedUserId);
            dateDeletedProperty?.SetValue(entityToBeDeleted, DateTimeOffset.UtcNow);
            isDeletedProperty?.SetValue(entityToBeDeleted, true);

            // Persist the changes (a regular update needs to be done pre-delete so it's updated before it becomes a temporal entry)
            // We need this operation to run synchronously, so don't make the next call async!
            _hardwareServiceOrderContext.Set<TEntity>().Update(entityToBeDeleted);
            _hardwareServiceOrderContext.SaveChanges();

            // Then we can delete it (make it a deleted record in the temporal table)
            _hardwareServiceOrderContext.Remove(entityToBeDeleted);
            await _hardwareServiceOrderContext.SaveChangesAsync();
        }


        /// <inheritdoc/>
        public async Task<TEntity> AddAndSaveAsync<TEntity>(TEntity entityToBeAdded) where TEntity : Auditable, IDbSetEntity
        {
            await _hardwareServiceOrderContext.Set<TEntity>()
                                              .AddAsync(entityToBeAdded);

            await _hardwareServiceOrderContext.SaveChangesAsync();
            return entityToBeAdded;
        }


        /// <inheritdoc/>
        public async Task<TEntity?> GetByIdAsync<TEntity>(int id) where TEntity : EntityV2, IDbSetEntity
        {
            return await _hardwareServiceOrderContext.Set<TEntity>()
                                                     .FindAsync(id);
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<TEntity>> GetByIdAsync<TEntity>(IEnumerable<int> ids) where TEntity : EntityV2, IDbSetEntity
        {
            return await _hardwareServiceOrderContext.Set<TEntity>()
                                                     .Where(e => ids.Contains(e.Id))
                                                     .ToListAsync();
        }


        /// <inheritdoc/>
        public async Task<TEntity> UpdateAndSaveAsync<TEntity>(TEntity entityToBeUpdated) where TEntity : Auditable, IDbSetEntity
        {
            _hardwareServiceOrderContext.Set<TEntity>()
                                        .Update(entityToBeUpdated);

            await _hardwareServiceOrderContext.SaveChangesAsync();
            return entityToBeUpdated;
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

            var existing = await GetCustomerServiceProviderAsync(customerId, providerId, false, false);

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
        public async Task<PagedModel<HardwareServiceOrder>> GetAllServiceOrdersAsync(Guid customerId, Guid? userId, int? serviceTypeId, bool activeOnly, int page, int limit, CancellationToken cancellationToken)
        {
            var orders = _hardwareServiceOrderContext.HardwareServiceOrders
                .Where(m => m.CustomerId == customerId);

            if (userId is not null)
                orders = orders.Where(m => m.Owner.UserId == userId);

            if (serviceTypeId is not null)
                orders = orders.Where(e => e.ServiceTypeId == serviceTypeId);

            if (activeOnly)
                orders = orders.Where(m => m.StatusId == (int)ServiceStatusEnum.Registered || m.StatusId == (int)ServiceStatusEnum.RegisteredInTransit ||
                m.StatusId == (int)ServiceStatusEnum.RegisteredUserActionNeeded || m.StatusId == (int)ServiceStatusEnum.Ongoing ||
                m.StatusId == (int)ServiceStatusEnum.OngoingInTransit || m.StatusId == (int)ServiceStatusEnum.OngoingReadyForPickup ||
                m.StatusId == (int)ServiceStatusEnum.OngoingUserActionNeeded || m.StatusId == (int)ServiceStatusEnum.Unknown);

            return await orders.OrderByDescending(m => m.DateCreated).PaginateAsync(page, limit, cancellationToken);

        }


        /// <inheritdoc/>
        public async Task<HardwareServiceOrder?> GetServiceOrderAsync(Guid orderId)
        {
            var order = await _hardwareServiceOrderContext.HardwareServiceOrders.FirstOrDefaultAsync(m => m.ExternalId == orderId);
            return order;
        }


        /// <inheritdoc/>
        public async Task<CustomerSettings?> GetSettingsAsync(Guid customerId)
        {
            return await _hardwareServiceOrderContext.CustomerSettings.FirstOrDefaultAsync(m => m.CustomerId == customerId);
        }


        /// <inheritdoc/>
        public async Task<HardwareServiceOrder> CreateHardwareServiceOrderAsync(HardwareServiceOrder serviceOrder)
        {
            var serviceType = await GetServiceTypeAsync(serviceOrder.ServiceTypeId);
            var serviceStatus = await GetServiceStatusAsync((int)ServiceStatusEnum.Registered);
            var serviceProvider = await GetCustomerServiceProviderAsync(serviceOrder.CustomerId, (int)ServiceProviderEnum.ConmodoNo, false, false);

            if (serviceProvider == null)
                throw new ArgumentException($"Failed to fetch ServiceProvider");

            if (serviceType == null)
                throw new ArgumentException($"Failed to fetch ServiceType");

            if (serviceStatus == null)
                throw new ArgumentException($"Failed to fetch ServiceStatus");

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
        public async Task<HardwareServiceOrder> UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus)
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
        public async Task<IEnumerable<ServiceProvider>> GetAllServiceProvidersAsync(bool includeSupportedServiceTypes,
                                                                                    bool includeOfferedServiceOrderAddons,
                                                                                    bool asNoTracking)
        {
            IQueryable<ServiceProvider> query = _hardwareServiceOrderContext.ServiceProviders;

            if (includeSupportedServiceTypes)
                query = query.Include(serviceProvider => serviceProvider.SupportedServiceTypes);

            if (includeOfferedServiceOrderAddons)
                query = query.Include(serviceProvider => serviceProvider.OfferedServiceOrderAddons);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }


        /// <inheritdoc/>
        public async Task<ServiceProvider?> GetServiceProviderByIdAsync(int id, 
                                                                        bool includeSupportedServiceTypes, 
                                                                        bool includeOfferedServiceOrderAddons,
                                                                        bool asNoTracking = false)
        {
            IQueryable<ServiceProvider> query = _hardwareServiceOrderContext.ServiceProviders;

            if (includeSupportedServiceTypes)
                query = query.Include(serviceProvider => serviceProvider.SupportedServiceTypes);

            if (includeOfferedServiceOrderAddons)
                query = query.Include(serviceProvider => serviceProvider.OfferedServiceOrderAddons);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<ServiceProvider>> GetAllServiceProvidersWithAddonFilterAsync(bool onlyCustomerTogglable,
                                                                                                   bool onlyUserSelectable,
                                                                                                   bool includeSupportedServiceTypes,
                                                                                                   bool asNoTracking)
        {
            IQueryable<ServiceProvider> query = _hardwareServiceOrderContext.ServiceProviders;

            // Include 'OfferedServiceOrderAddons' using conditional where filters that's determined by the parameters.
            // This implementation is clunky, and should likely be revisited later.
            if (onlyCustomerTogglable && onlyUserSelectable)
                query = query.Include(e => e.OfferedServiceOrderAddons!.Where(e => e.IsCustomerTogglable && e.IsUserSelectable));
            else if (onlyCustomerTogglable)
                query = query.Include(e => e.OfferedServiceOrderAddons!.Where(e => e.IsCustomerTogglable));
            else if (onlyUserSelectable)
                query = query.Include(e => e.OfferedServiceOrderAddons!.Where(e => e.IsUserSelectable));
            else
                query = query.Include(e => e.OfferedServiceOrderAddons);

            // Apply a conditional include for 'SupportedServiceTypes'
            if (includeSupportedServiceTypes)
                query = query.Include(serviceProvider => serviceProvider.SupportedServiceTypes);

            if (asNoTracking)
                query = query.AsNoTracking();

            var result = await query.ToListAsync();
            return result;
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<CustomerServiceProvider>> GetCustomerServiceProvidersByFilterAsync(Expression<Func<CustomerServiceProvider, bool>>? filter,
                                                                                                         bool includeApiCredentials,
                                                                                                         bool includeActiveServiceOrderAddons,
                                                                                                         bool asNoTracking)
        {
            IQueryable<CustomerServiceProvider> query = _hardwareServiceOrderContext.Set<CustomerServiceProvider>();

            if (filter is not null)
                query = query.Where(filter);

            if (includeApiCredentials)
                query = query.Include(entity => entity.ApiCredentials);

            if (includeActiveServiceOrderAddons)
                query = query.Include(entity => entity.ActiveServiceOrderAddons);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }


        /// <inheritdoc/>
        public async Task<CustomerServiceProvider?> GetCustomerServiceProviderAsync(Guid organizationId,
                                                                                    int serviceProviderId,
                                                                                    bool includeApiCredentials,
                                                                                    bool includeActiveServiceOrderAddons)
        {
            IQueryable<CustomerServiceProvider> query = _hardwareServiceOrderContext.Set<CustomerServiceProvider>()
                                                                                    .Where(entity => entity.CustomerId == organizationId && entity.ServiceProviderId == serviceProviderId);

            if (includeApiCredentials)
                query = query.Include(entity => entity.ApiCredentials);

            if (includeActiveServiceOrderAddons)
                query = query.Include(entity => entity.ActiveServiceOrderAddons);

            return await query.FirstOrDefaultAsync();
        }



        /// <inheritdoc/>
        public async Task<ApiCredential> AddOrUpdateApiCredentialAsync(int customerServiceProviderId, int? serviceTypeId, string? apiUsername, string? apiPassword)
        {
            ApiCredential? apiCredential = await _hardwareServiceOrderContext.ApiCredentials.FirstOrDefaultAsync(entity => entity.CustomerServiceProviderId == customerServiceProviderId && entity.ServiceTypeId == serviceTypeId);

            if (apiCredential is null)
            {
                apiCredential = new(customerServiceProviderId, serviceTypeId, apiUsername, apiPassword);
                await _hardwareServiceOrderContext.AddAsync(apiCredential);
            }
            else
            {
                apiCredential.ApiUsername = apiUsername;
                apiCredential.ApiPassword = apiPassword;

                _hardwareServiceOrderContext.Update(apiCredential);
            }

            await _hardwareServiceOrderContext.SaveChangesAsync();
            return apiCredential;
        }
        
        /// <inheritdoc/>
        public async Task UpdateApiCredentialLastUpdateFetchedAsync(ApiCredential apiCredential, DateTimeOffset lastUpdateFetched)
        {
            apiCredential.LastUpdateFetched = lastUpdateFetched;

            _hardwareServiceOrderContext.Update(apiCredential);

            await _hardwareServiceOrderContext.SaveChangesAsync();
        }
    }
}
