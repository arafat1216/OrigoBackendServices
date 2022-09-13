using Common.Interfaces;
using HardwareServiceOrderServices.Exceptions;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices
{
    public interface IHardwareServiceOrderService
    {
        /// <summary>
        /// Get service provider's username
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="providerId">Provider Identifier</param>
        /// <returns>Username of the provider</returns>
        Task<string?> GetServicerProvidersUsernameAsync(Guid customerId, int providerId);

        /// <summary>
        /// Configure customer service provider
        /// </summary>
        /// <param name="providerId">Provider identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="apiUsername">Username for calling provider's API</param>
        /// <param name="apiPassword">Password for calling provider's API</param>
        /// <returns>Provider's apiUsername</returns>
        Task<string?> ConfigureCustomerServiceProviderAsync(int providerId, Guid customerId, string? apiUsername, string? apiPassword);

        // Configuration
        /// <summary>
        /// Configure service id
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task<CustomerSettingsDTO> ConfigureCustomerSettingsAsync(Guid customerId, Guid callerId);

        /// <summary>
        /// Configure loan phone
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="loanPhoneNumber">The phone-number in <c>E.164</c> format.</param>
        /// <param name="loanPhoneEmail">Email address</param>
        /// <param name="providesLoanDevice">This parameter ensures whether a customer provides loan device</param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task<CustomerSettingsDTO> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmail, bool providesLoanDevice, Guid callerId);
        Task<CustomerSettingsDTO> GetSettingsAsync(Guid customerId);

        /// <summary>
        ///     Updates a existing <see cref="CustomerSettings"/> if found. Otherwise a new one is created.
        ///     
        ///     <para>
        ///     The add/create is performed using the organization's ID, provided in the <paramref name="customerSettingsDTO"/> 
        ///     property (<see cref="CustomerSettingsDTO.CustomerId"/>). </para>
        /// </summary>
        /// <param name="customerSettingsDTO"> The new customer settings. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the created/updated entity. </returns>
        Task<CustomerSettingsDTO> AddOrUpdateCustomerSettings(CustomerSettingsDTO customerSettingsDTO);

        /// <summary>
        ///     Retrieves the <see cref="CustomerSettingsDTO"/> for a given organization.
        /// </summary>
        /// <param name="organizationId"> The organization to retrieve. </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result contains the retrieved result, or <see langword="null"/>
        ///     of no results were found. 
        /// </returns>
        Task<CustomerSettingsDTO?> GetCustomerSettings(Guid organizationId);

        // Order
        Task<HardwareServiceOrderDTO> CreateHardwareServiceOrderAsync(Guid customerId, NewHardwareServiceOrderDTO serviceOrderDTO);

        /// <summary>
        ///     Retrieves a service-order using it's ID.
        /// </summary>
        /// <param name="serviceOrderId"> The ID of the service-order that should be retrieved. </param>
        /// <param name="organizationId"> 
        ///     An optional safety-check that ensures we will only retrieve the result, if it actually belongs to this customer.
        ///     When the value is <see langword="null"/>, the filter is ignored. 
        /// </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result contains the retrieved order, or <see langword="null"/> if no orders were found. 
        /// </returns>
        Task<HardwareServiceOrderDTO?> GetServiceOrderByIdAsync(Guid serviceOrderId, Guid? organizationId = null);

        /// <summary>
        ///     Retrieves all service-orders that matches the parameters.
        /// </summary>
        /// <param name="organizationId"> Filter the results to only contain this customer. </param>
        /// <param name="userId"> Filter the results to only contain this user. When the value is <see langword="null"/>, the filter is ignored. </param>
        /// <param name="serviceTypeId"> Filter the results to only contain this service-type. When the value is <see langword="null"/>, the filter is ignored. </param>
        /// <param name="activeOnly"> When <see langword="true"/>, only active/ongoing service-orders are retrieved. When <see langword="false"/>, the filter is ignored. </param>
        /// <param name="page"> The paginated page that should be retrieved. </param>
        /// <param name="limit"> The number of items to retrieve per <paramref name="page"/>. </param>
        /// <param name="cancellationToken"></param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the retrieved, paginated results. </returns>
        Task<PagedModel<HardwareServiceOrderDTO>> GetAllServiceOrdersForOrganizationAsync(Guid organizationId, Guid? userId, int? serviceTypeId, bool activeOnly, CancellationToken cancellationToken, int page = 1, int limit = 25);

        /// <summary>
        /// Update all order status since last updated datetime
        /// </summary>
        /// <returns></returns>
        Task UpdateOrderStatusAsync();


        /*
         * Service providers
         */

        /// <summary>
        ///     Returns a list containing all service-providers in the system.
        /// </summary>
        /// <param name="includeSupportedServiceTypes"> Should <see cref="ServiceProviderDTO.SupportedServiceTypes"/> be loaded and included in the result? </param>
        /// <param name="includeOfferedServiceOrderAddons"> Should <see cref="ServiceProviderDTO.OfferedServiceOrderAddons"/> be loaded and included in the result? </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a list of all service-providers in the database. </returns>
        Task<IEnumerable<ServiceProviderDTO>> GetAllServiceProvidersAsync(bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons);

        /// <summary>
        ///     Returns a service-provider using it's ID.
        /// </summary>
        /// <param name="id"> The ID of the service-provider that should be retrieved. </param>
        /// <param name="includeSupportedServiceTypes"> Should <see cref="ServiceProviderDTO.SupportedServiceTypes"/> be loaded and included in the result? </param>
        /// <param name="includeOfferedServiceOrderAddons"> Should <see cref="ServiceProviderDTO.OfferedServiceOrderAddons"/> be loaded and included in the result? </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the retrieved <see cref="ServiceProviderDTO"/>. </returns>
        /// <exception cref="NotFoundException"> Thrown if the provided <paramref name="id"/> don't result in any matches. </exception>
        Task<ServiceProviderDTO> GetServiceProviderById(int id, bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons);


        /*
         * Customer Service Provider
         */

        /// <summary>
        ///     Retrieves all <see cref="CustomerServiceProvider"/>-configurations that's used by a given customer.
        /// </summary>
        /// <param name="organizationId"> The customers identifier. </param>
        /// <param name="includeApiCredentials"> Should <see cref="CustomerServiceProvider.ApiCredentials"/> be loaded and included in the results? </param>
        /// <param name="includeActiveServiceOrderAddons"> Should <see cref="CustomerServiceProvider.ActiveServiceOrderAddons"/> be loaded and included in the results? </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a list of all retrieved customer-service-provider details. </returns>
        Task<IEnumerable<CustomerServiceProviderDto>> GetCustomerServiceProvidersAsync(Guid organizationId, bool includeApiCredentials = false, bool includeActiveServiceOrderAddons = false);


        /// <summary>
        ///     Retrieves the <see cref="CustomerServiceProvider"/>-configuration that's used between a given customer and service-provider.
        /// </summary>
        /// <param name="organizationId"> The customers identifier. </param>
        /// <param name="serviceProviderId"> The service-provider's identifier. </param>
        /// <param name="includeApiCredentials"> Should <see cref="CustomerServiceProvider.ApiCredentials"/> be loaded and included in the results? </param>
        /// <param name="includeActiveServiceOrderAddons"> Should <see cref="CustomerServiceProvider.ActiveServiceOrderAddons"/> be loaded and included in the results? </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the matching customer-service-provider details. </returns>
        /// <exception cref="NotFoundException"> Thrown if no matching customer-service-providers were found. </exception>
        Task<CustomerServiceProviderDto> GetCustomerServiceProviderAsync(Guid organizationId, int serviceProviderId, bool includeApiCredentials = false, bool includeActiveServiceOrderAddons = false);


        /// <summary>
        ///     Deletes an existing API credential.
        /// </summary>
        /// <param name="organizationId"> The customer the API credential is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credential is used with. </param>
        /// <param name="serviceTypeId"> The service-type the API credentials can be used with. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task DeleteApiCredentialAsync(Guid organizationId, int serviceProviderId, int? serviceTypeId);


        /// <summary>
        ///     Adds or updates an API credential for a customer's service-provider configuration.
        ///     
        ///     <para>
        ///     If an existing credential already exist (using the same unique combination of <paramref name="organizationId"/>, 
        ///     <paramref name="serviceProviderId"/> and <paramref name="serviceTypeId"/> ), then it will be overwritten using the new values. </para>
        /// </summary>
        /// <param name="organizationId"> The customer the API credential is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credential is used with. </param>
        /// <param name="serviceTypeId"> The <see cref="ServiceType.Id"/> this API credential is valid for. </param>
        /// <param name="apiUsername"> The API username. If it's not applicable for the service-provider, it should be <see langword="null"/>. </param>
        /// <param name="apiPassword"> The API password. If it's not applicable for the service-provider, it should be <see langword="null"/>. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task AddOrUpdateApiCredentialAsync(Guid organizationId, int serviceProviderId, int? serviceTypeId, string? apiUsername, string? apiPassword);


        /// <summary>
        ///     Adds new <see cref="ServiceOrderAddon"/>s to a <see cref="CustomerServiceProvider"/>.
        /// </summary>
        /// <param name="organizationId"> The customer identifier. </param>
        /// <param name="serviceProviderId"> The service-provider identifier. </param>
        /// <param name="newServiceOrderAddonIds"> A list of containing the identifiers for all <see cref="ServiceOrderAddon"/> 
        ///     items that should be added. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        /// <exception cref="NotFoundException"> Thrown if one or more of the new service-order addons don't exist. </exception>
        /// <exception cref="ArgumentException"> 
        ///     Thrown if one or more of the values is invalid. For example when trying to
        ///     add a <see cref="ServiceOrderAddon"/> that belongs to another <see cref="ServiceProvider"/>. 
        /// </exception>
        Task AddServiceOrderAddonsToCustomerServiceProviderAsync(Guid organizationId, int serviceProviderId, ISet<int> newServiceOrderAddonIds);

        /// <summary>
        ///     Removes <see cref="ServiceOrderAddon"/>s from a <see cref="CustomerServiceProvider"/>.
        /// </summary>
        /// <param name="organizationId"> The customer identifier. </param>
        /// <param name="serviceProviderId"> The service-provider identifier. </param>
        /// <param name="removedServiceOrderAddonIds"> A list of containing the identifiers for all <see cref="ServiceOrderAddon"/> 
        ///     items that should be removed. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task RemoveServiceOrderAddonsFromCustomerServiceProviderAsync(Guid organizationId, int serviceProviderId, ISet<int> removedServiceOrderAddonIds);
    }
}
