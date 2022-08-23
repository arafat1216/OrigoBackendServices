using Common.Interfaces;

namespace HardwareServiceOrderServices.Models
{
    public interface IHardwareServiceOrderRepository
    {
        /// <summary>
        /// Configure customer service provider
        /// </summary>
        /// <param name="providerId">Provider identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="apiUsername">Username for calling provider's API</param>
        /// <param name="apiPassword">Password for calling provider's API</param>
        /// <returns></returns>
        [Obsolete("This will be replaced in the R&W work-package")]
        Task<string?> ConfigureCustomerServiceProviderAsync(int providerId, Guid customerId, string? apiUsername, string? apiPassword);

        /// <summary>
        /// Configure customer settings
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        [Obsolete("This will be replaced in the R&W work-package")]
        Task<CustomerSettings> ConfigureCustomerSettingsAsync(Guid customerId, Guid callerId);

        /// <summary>
        /// Configure Loan Phone
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="loanPhoneNumber">The phone-number in <c>E.164</c> format.</param>
        /// <param name="loanPhoneEmail"></param>
        /// <param name="providesLoanDevice">This parameter ensures whether a customer provides loan device</param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        [Obsolete("This will be replaced in the R&W work-package")]
        Task<CustomerSettings> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmail, bool providesLoanDevice, Guid callerId);

        [Obsolete("This will be replaced in the R&W work-package")]
        Task<CustomerSettings?> GetSettingsAsync(Guid customerId);

        [Obsolete("This will be replaced in the R&W work-package")]
        Task<string?> GetServiceIdAsync(Guid customerId);

        /// <summary>
        /// Get all customers' providers
        /// </summary>
        /// <returns></returns>
        [Obsolete("This will be replaced in the R&W work-package")]
        Task<List<CustomerServiceProvider>> GetAllCustomerProvidersAsync();

        // TODO: This method makes no sense. If we retrieve orders based on the service-provider the result will always be a list, so this needs to be investigated!
        /// <summary>
        /// Get order by service provider's order ID
        /// </summary>
        /// <param name="serviceProviderOrderId">The identifier that was provided by the service-provider. <see cref="HardwareServiceOrder.ServiceProviderOrderId1"/></param>
        /// <returns></returns>
        [Obsolete("This will be replaced in the R&W work-package")]
        Task<HardwareServiceOrder?> GetOrderByServiceProviderOrderIdAsync(string serviceProviderOrderId);

        /// <summary>
        /// Update Customer Provider's LastUpdateFetched
        /// </summary>
        /// <param name="customerServiceProvider">Existing Customer Service Provider <see cref="Models.CustomerServiceProvider"/></param>
        /// <param name="lastUpdateFetched">Last DateTime when the updates were fetched from the service provider</param>
        /// <returns></returns>
        [Obsolete("This will be replaced in the R&W work-package")]
        Task UpdateCustomerProviderLastUpdateFetchedAsync(CustomerServiceProvider customerServiceProvider, DateTimeOffset lastUpdateFetched);



        /*
         * Service-order & Service-events
         */

        /// <summary>
        ///     Update the list of service-events for an existing service-order.
        /// </summary>
        /// <param name="order"> The service-order that will be updated. </param>
        /// <param name="events"> The list of service-events. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task UpdateServiceEventsAsync(HardwareServiceOrder order, IEnumerable<ServiceEvent> events);

        Task<HardwareServiceOrder> CreateHardwareServiceOrderAsync(HardwareServiceOrder serviceOrder);

        /// <summary>
        ///     Changes the status of a service order.
        /// </summary>
        /// <param name="orderId"> The ID of the service-order that should be updated. </param>
        /// <param name="newStatus"> The new <see cref="ServiceStatusEnum">status</see>. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the updated service-order. </returns>
        Task<HardwareServiceOrder> UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus);

        /// <summary>
        ///     Retrieves a specific service-order.
        /// </summary>
        /// <param name="orderId"> The ID of the service-order that should be retrieved. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the queried service-order if a result was found.
        ///     Otherwise the value will be <see langword="null"/>. </returns>
        Task<HardwareServiceOrder?> GetServiceOrderAsync(Guid orderId);

        Task<PagedModel<HardwareServiceOrder>> GetAllServiceOrdersAsync(Guid customerId, Guid? userId, bool activeOnly, int page, int limit, CancellationToken cancellationToken);


        /*
         * Service-status
         */

        /// <summary>
        ///     Retrieves a single service-status using a provided identifier.
        /// </summary>
        /// <param name="id"> The ID for the service-type that should be retrieved. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the corresponding <see cref="ServiceStatus"/>
        ///     if one was found. If no results was found, this will be <see langword="null"/>. </returns>
        Task<ServiceStatus?> GetServiceStatusAsync(int id);


        /*
         * Service-type
         */

        /// <summary>
        ///     Retrieves a single service-type using a provided identifier.
        /// </summary>
        /// <param name="id"> The ID for the service-type that should be retrieved. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the corresponding <see cref="ServiceType"/>
        ///     if one was found. If no results was found, this will be <see langword="null"/>. </returns>
        Task<ServiceType?> GetServiceTypeAsync(int id);


        /*
         * Service Provider
         */

        /// <summary>
        ///     Retrieves all service-providers that exist in the system.
        /// </summary>
        /// <param name="includeSupportedServiceTypes"> Should <see cref="ServiceProvider.SupportedServiceTypes"/> be loaded and included in the result? </param>
        /// <param name="includeOfferedServiceOrderAddons"> Should <see cref="ServiceProvider.OfferedServiceOrderAddons"/> be loaded and included in the result? </param>
        /// <param name="asNoTracking"> 
        ///     Should the query be run using '<c>AsNoTracking</c>'? 
        ///     
        ///     <para>
        ///     To improve performance, this should be set to <see langword="true"/> for read-only operations.
        ///     However, if any write operations will occur, then this should always be set to <see langword="false"/>. </para>
        /// </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a collection of all matching items. </returns>
        /// <seealso cref="GetAllServiceProvidersWithAddonFilterAsync(bool, bool, bool, bool)"/>
        Task<IEnumerable<ServiceProvider>> GetAllServiceProvidersAsync(bool includeSupportedServiceTypes,
                                                                       bool includeOfferedServiceOrderAddons,
                                                                       bool asNoTracking);


        /// <summary>
        ///     Retrieves all service-providers that exist in the system, and applies a conditional include for their offered service-addons.
        /// </summary>
        /// <param name="onlyCustomerTogglable"> If <see langword="true"/>, the <see cref="ServiceProvider.OfferedServiceOrderAddons"/> list will be filtered to
        ///     only contain items where "<c><see cref="ServiceOrderAddon.IsCustomerTogglable"/> == <see langword="true"/></c>". </param>
        /// <param name="onlyUserSelectable"> If <see langword="true"/>, the <see cref="ServiceProvider.OfferedServiceOrderAddons"/> list will be filtered to
        ///     only contain items where "<c><see cref="ServiceOrderAddon.IsUserSelectable"/> == <see langword="true"/></c>". </param>
        /// <param name="includeSupportedServiceTypes"> Should <see cref="ServiceProvider.SupportedServiceTypes"/> be loaded and included in the result? </param>
        /// <param name="asNoTracking"> 
        ///     Should the query be run using '<c>AsNoTracking</c>'? 
        ///     
        ///     <para>
        ///     To improve performance, this should be set to <see langword="true"/> for read-only operations.
        ///     However, if any write operations will occur, then this should always be set to <see langword="false"/>. </para>
        /// </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a collection of all matching items. </returns>
        /// <seealso cref="GetAllServiceProvidersAsync(bool, bool, bool)"/>
        Task<IEnumerable<ServiceProvider>> GetAllServiceProvidersWithAddonFilterAsync(bool onlyCustomerTogglable,
                                                                                      bool onlyUserSelectable,
                                                                                      bool includeSupportedServiceTypes,
                                                                                      bool asNoTracking);


        /*
         * Customer Service Provider
         */

        /// <summary>
        ///     Get the customer-specific configuration for a given service provider.
        /// </summary>
        /// <param name="customerId"> The customers identifier. </param>
        /// <param name="serviceProviderId"> The service-provider identifier. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the corresponding <see cref="CustomerServiceProvider"/>
        ///     if one was found. If no results was found, this will be <see langword="null"/>. </returns>
        Task<CustomerServiceProvider?> GetCustomerServiceProviderAsync(Guid customerId, int serviceProviderId);

        /// <summary>
        ///     Updates an existing <see cref="ApiCredential"/>. If it don't exist, it is created.
        /// </summary>
        /// <param name="customerServiceProviderId"> The identifier for the <see cref="CustomerServiceProvider"/> the credentials is attached to. </param>
        /// <param name="serviceTypeId"> The service-type the credentials is used with. </param>
        /// <param name="apiUsername"> The new API username. </param>
        /// <param name="apiPassword"> The new API password. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the new or updated <see cref="ApiCredential"/>. </returns>
        Task<ApiCredential> AddOrUpdateApiCredentialAsync(int customerServiceProviderId, int serviceTypeId, string? apiUsername, string? apiPassword);

        /// <summary>
        ///     Deletes an existing <see cref="ApiCredential"/>.
        /// </summary>
        /// <param name="apiCredential"> The credential to be deleted. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task DeleteApiCredentialAsync(ApiCredential apiCredential);

    }
}
