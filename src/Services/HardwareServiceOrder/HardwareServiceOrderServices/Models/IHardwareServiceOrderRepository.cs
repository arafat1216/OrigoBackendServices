using Common.Interfaces;
using Common.Seedwork;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace HardwareServiceOrderServices.Models
{
    public interface IHardwareServiceOrderRepository
    {
        /*
         * Generics
         */

        /// <summary>
        ///     Adds a new entity, and saves it to the database.
        /// </summary>
        /// <typeparam name="TEntity"> The entities datatype. </typeparam>
        /// <param name="entityToBeAdded"> The entity that should be created. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the stored entity. </returns>
        Task<TEntity> AddAndSaveAsync<TEntity>(TEntity entityToBeAdded) where TEntity : Auditable, IDbSetEntity;

        /// <summary>
        ///     Deletes an existing entity from the database.
        /// </summary>
        /// <typeparam name="TEntity"> The entities datatype. </typeparam>
        /// <param name="entityToBeDeleted"> The entity that should be deleted. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task DeleteAndSaveAsync<TEntity>(TEntity entityToBeDeleted) where TEntity : Auditable, IDbSetEntity;

        /// <summary>
        ///     Retrieves a list of entities by it's primary-keys.
        /// </summary>
        /// <typeparam name="TEntity"> The entity datatype. </typeparam>
        /// <param name="id"> The entities primary keys. </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result contains the retrieved entity, 
        ///     or <see langword="null"/> if no matches was found.
        /// </returns>
        Task<TEntity?> GetByIdAsync<TEntity>(int id) where TEntity : EntityV2, IDbSetEntity;

        /// <summary>
        ///     Retrieves a list of entities by it's primary-key.
        /// </summary>
        /// <typeparam name="TEntity"> The entities datatype. </typeparam>
        /// <param name="ids"> A list containing the entities primary-keys. </param>
        /// <param name="asNoTracking"> 
        ///     Should the query be run using '<c>AsNoTracking</c>'? 
        ///     
        ///     <para>
        ///     To improve performance, this should be set to <see langword="true"/> for read-only operations.
        ///     However, if any write operations will occur, then this should always be set to <see langword="false"/>. </para>
        /// </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a list of all retrieved entities. </returns>
        Task<IEnumerable<TEntity>> GetByIdAsync<TEntity>(IEnumerable<int> ids, bool asNoTracking) where TEntity : EntityV2, IDbSetEntity;

        /// <summary>
        ///     Updates an existing entity, and saves it to the database.
        /// </summary>
        /// <typeparam name="TEntity"> The entities datatype. </typeparam>
        /// <param name="entityToBeUpdated"> The entity that should be updated. </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result contains the updated entity.
        /// </returns>
        Task<TEntity> UpdateAndSaveAsync<TEntity>(TEntity entityToBeUpdated) where TEntity : Auditable, IDbSetEntity;


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
        /// <param name="serviceOrderId"> The ID of the service-order that should be retrieved. </param>
        /// <param name="organizationId"> An optional safety-check that ensures we will only retrieve the result, if it actually belongs to this customer.
        ///     When the value is <see langword="null"/>, the filter is ignored. </param>
        /// <param name="asNoTracking"> 
        ///     Should the query be run using '<c>AsNoTracking</c>'? 
        ///     
        ///     <para>
        ///     To improve performance, this should be set to <see langword="true"/> for read-only operations.
        ///     However, if any write operations will occur, then this should always be set to <see langword="false"/>. </para>
        /// </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the queried service-order if a result was found.
        ///     Otherwise the value will be <see langword="null"/>. </returns>
        Task<HardwareServiceOrder?> GetServiceOrderByIdAsync(Guid serviceOrderId, bool asNoTracking, Guid? organizationId = null);

        /// <summary>
        ///     Retrieves all service-orders that matches the parameters.
        /// </summary>
        /// <param name="organizationId"> Filter the results to only contain this customer. </param>
        /// <param name="userId"> Filter the results to only contain this user. When the value is <see langword="null"/>, the filter is ignored. </param>
        /// <param name="serviceTypeId"> Filter the results to only contain this service-type. When the value is <see langword="null"/>, the filter is ignored. </param>
        /// <param name="activeOnly"> When <see langword="true"/>, only active/ongoing service-orders are retrieved. When <see langword="false"/>, the filter is ignored. </param>
        /// <param name="page"> The paginated page that should be retrieved. </param>
        /// <param name="limit"> The number of items to retrieve per <paramref name="page"/>. </param>
        /// <param name="asNoTracking"> 
        ///     Should the query be run using '<c>AsNoTracking</c>'? 
        ///     
        ///     <para>
        ///     To improve performance, this should be set to <see langword="true"/> for read-only operations.
        ///     However, if any write operations will occur, then this should always be set to <see langword="false"/>. </para>
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the retrieved, paginated results. </returns>
        Task<PagedModel<HardwareServiceOrder>> GetAllServiceOrdersForOrganizationAsync(Guid organizationId, Guid? userId, int? serviceTypeId, bool activeOnly, int page, int limit, bool asNoTracking, CancellationToken cancellationToken);


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
        ///     Retrieves a service-provider by it's ID.
        /// </summary>
        /// <param name="id"> The ID that should be retrieved. </param>
        /// <param name="includeSupportedServiceTypes"> Should <see cref="ServiceProvider.SupportedServiceTypes"/> be loaded and included in the result? </param>
        /// <param name="includeOfferedServiceOrderAddons"> Should <see cref="ServiceProvider.OfferedServiceOrderAddons"/> be loaded and included in the result? </param>
        /// <param name="asNoTracking"> 
        ///     Should the query be run using '<c>AsNoTracking</c>'? 
        ///     
        ///     <para>
        ///     To improve performance, this should be set to <see langword="true"/> for read-only operations.
        ///     However, if any write operations will occur, then this should always be set to <see langword="false"/>. </para>
        /// </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result contains the retrieved <see cref="ServiceProvider"/>,
        ///     or <see langword="null"/> if no matches were found.
        /// </returns>
        Task<ServiceProvider?> GetServiceProviderByIdAsync(int id,
                                                           bool includeSupportedServiceTypes,
                                                           bool includeOfferedServiceOrderAddons,
                                                           bool asNoTracking = false);


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
         * Customer Configuration (global configuration - not for 'Customer Service Provider')
         */

        /// <summary>
        ///     Retrieves the <see cref="CustomerSettings"/> for a given organization.
        /// </summary>
        /// <param name="organizationId"> The organization identifier. </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result contains the retrieved entity, 
        ///     or <see langword="null"/> if no matches was found. 
        /// </returns>
        Task<CustomerSettings?> GetCustomerSettingsByOrganizationIdAsync(Guid organizationId);


        /*
         * Customer Service Provider
         */

        /// <summary>
        ///     Get the customer-specific configuration for a given service provider.
        /// </summary>
        /// <param name="organizationId"> The customers identifier. </param>
        /// <param name="serviceProviderId"> The service-provider identifier. </param>
        /// <param name="includeApiCredentials"> If <see langword="true"/>, then the <see cref="CustomerServiceProvider.ApiCredentials"/>
        ///     list will be loaded and included in the result. </param>
        /// <param name="includeActiveServiceOrderAddons"> If <see langword="true"/>, then the <see cref="CustomerServiceProvider.ActiveServiceOrderAddons"/>
        ///     list will be loaded and included in the result. </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result contains the corresponding <see cref="CustomerServiceProvider"/>
        ///     if one was found. If no results was found, this will be <see langword="null"/>. 
        /// </returns>
        Task<CustomerServiceProvider?> GetCustomerServiceProviderAsync(Guid organizationId, int serviceProviderId, bool includeApiCredentials, bool includeActiveServiceOrderAddons);


        /// <summary>
        ///     Retrieves all <see cref="CustomerServiceProvider">customer service-providers</see> that matches the provided <paramref name="filter"/> parameter.
        /// </summary>
        /// <param name="filter"> The filter (where condition) that is applied to the query. </param>
        /// <param name="includeApiCredentials"> If <see langword="true"/>, then the <see cref="CustomerServiceProvider.ApiCredentials"/>,
        ///     list will be loaded and included in the results. </param>
        /// <param name="includeActiveServiceOrderAddons"> If <see langword="true"/>, then the <see cref="CustomerServiceProvider.ActiveServiceOrderAddons"/>
        ///     list will be loaded and included in the results. </param>
        /// <param name="asNoTracking"> 
        ///     Should the query be run using '<c>AsNoTracking</c>'?
        ///     
        ///     <para>
        ///     To improve performance, this should be set to <see langword="true"/> for read-only operations.
        ///     However, if any write operations will occur, then this should always be set to <see langword="false"/>. </para>
        /// </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result contains a list of all matching <see cref="CustomerServiceProvider"/> entities. 
        /// </returns>
        Task<IEnumerable<CustomerServiceProvider>> GetCustomerServiceProvidersByFilterAsync(Expression<Func<CustomerServiceProvider, bool>>? filter,
                                                                                            bool includeApiCredentials,
                                                                                            bool includeActiveServiceOrderAddons,
                                                                                            bool asNoTracking);


        /// <summary>
        ///     Updates an existing <see cref="ApiCredential"/>. If it don't exist, it is created.
        /// </summary>
        /// <param name="organizationId"> The Organization or Customer identifier </param>
        /// <param name="customerServiceProviderId"> The identifier for the <see cref="CustomerServiceProvider"/> the credentials is attached to. </param>
        /// <param name="serviceTypeId"> The service-type the credentials is used with. </param>
        /// <param name="apiUsername"> The new API username. </param>
        /// <param name="apiPassword"> The new API password. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the new or updated <see cref="ApiCredential"/>. </returns>
        Task<ApiCredential> AddOrUpdateApiCredentialAsync(Guid organizationId, int customerServiceProviderId, int? serviceTypeId, string? apiUsername, string? apiPassword);

        /// <summary>
        ///     Update <see cref="Models.ApiCredential"/>'s LastUpdateFetched property.
        /// </summary>
        /// <param name="apiCredential">Existing ApiCredentials <see cref="Models.ApiCredential"/></param>
        /// <param name="lastUpdateFetched"> Last time the orders fetched from the third party service provider</param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the new or updated <see cref="ApiCredential"/>. </returns>
        Task UpdateApiCredentialLastUpdateFetchedAsync(ApiCredential apiCredential, DateTimeOffset lastUpdateFetched);


        /*
         * Misc & Legacy
         */


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

        // TODO: This method makes no sense. If we retrieve orders based on the service-provider the result will always be a list, so this needs to be investigated!
        /// <summary>
        /// Get order by service provider's order ID
        /// </summary>
        /// <param name="serviceProviderOrderId">The identifier that was provided by the service-provider. <see cref="HardwareServiceOrder.ServiceProviderOrderId1"/></param>
        /// <returns></returns>
        [Obsolete("This will be replaced in the R&W work-package")]
        Task<HardwareServiceOrder?> GetOrderByServiceProviderOrderIdAsync(string serviceProviderOrderId);

        /// <summary>
        /// Encrypt a Text
        /// </summary>
        /// <param name="text">The text to encrypt</param>
        /// <param name="key">The "key" using which the text is encrypted </param>
        /// <returns>Returns the encrypted text.
        /// But in case of null/empty, returns the text itself</returns>
        public string? Encrypt(string? text, string key);

        /// <summary>
        /// Decrypt a Text
        /// </summary>
        /// <param name="encryptedText">The encrypted/cipher text that needs to decrypt</param>
        /// <param name="key">The "key" using which the text is decrypted </param>
        /// <returns>Returns the decrypted text.
        /// But in case of null/empty, returns the text itself</returns>
        /// <exception cref="CryptographicException"> Thrown if the method is not able to decrypt the provided text</exception>
        public string? Decrypt(string? encryptedText, string key);
    }
}
