﻿using Common.Interfaces;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;

#nullable enable

namespace OrigoApiGateway.Services
{
    /// <summary>
    ///     Defines the methods and microservice-calls that should be available for hardware-services.
    /// </summary>
    public interface IHardwareServiceOrderService
    {

        /// <summary>
        ///     Retrieve all service-providers from the solution.
        /// </summary>
        /// <param name="includeSupportedServiceTypes"> If <see langword="true"/>, then the <see cref="Models.HardwareServiceOrder.Backend.ServiceProvider.SupportedServiceTypeIds">service-provider's supported service-type</see> 
        ///     list is included/loaded for all service-providers. Otherwise, the value is ignored and will be <see langword="null"/>. </param>
        /// <param name="includeOfferedServiceOrderAddons"> If <see langword="true"/>, then the <see cref="Models.HardwareServiceOrder.Backend.ServiceOrderAddon">service-provider's service-addon</see>
        ///     list is included/loaded for all service-providers. Otherwise, the value is ignored and will be <see langword="null"/>. </param>
        /// <returns> A collection that contains all service-providers that exists in the solution. </returns>
        Task<IEnumerable<Models.HardwareServiceOrder.Backend.ServiceProvider>> GetAllServiceProvidersAsync(bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons);

        /// <inheritdoc cref="GetAllServiceProvidersAsync(bool, bool)"/>
        /// <remarks>
        ///     The results is filtered to only include the data that's relevant for the customer-portals configuration pages.
        /// </remarks>
        Task<IEnumerable<CustomerPortalServiceProvider>> CustomerPortalGetAllServiceProvidersAsync(bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons);

        /// <summary>
        ///     Retrieves all <see cref="CustomerServiceProvider"/>-configurations that's used by a given customer.
        /// </summary>
        /// <param name="organizationId"> The customers identifier. </param>
        /// <param name="includeApiCredentialIndicators"> Should <see cref="CustomerServiceProvider.ApiCredentials"/> be loaded and included in the results? </param>
        /// <param name="includeActiveServiceOrderAddons"> Should <see cref="CustomerServiceProvider.ActiveServiceOrderAddons"/> be loaded and included in the results? </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a list of all retrieved customer-service-provider details. </returns>
        Task<IEnumerable<CustomerServiceProvider>> GetCustomerServiceProvidersAsync(Guid organizationId, bool includeApiCredentialIndicators, bool includeActiveServiceOrderAddons);

        /// <inheritdoc cref="GetCustomerServiceProvidersAsync(Guid, bool, bool)"/>
        /// <remarks>
        ///     The results is filtered to only include the data that's relevant for the customer-portals configuration pages.
        /// </remarks>
        Task<IEnumerable<CustomerPortalCustomerServiceProvider>> CustomerPortalGetCustomerServiceProvidersAsync(Guid organizationId, bool includeActiveServiceOrderAddons);

        /// <inheritdoc cref="GetCustomerServiceProvidersAsync(Guid, bool, bool)"/>
        /// <remarks>
        ///     The results is filtered to only include the data that's relevant for the user-portal/order forms.
        /// </remarks>
        Task<IEnumerable<UserPortalCustomerServiceProvider>> UserPortalGetCustomerServiceProvidersAsync(Guid organizationId, bool includeActiveServiceOrderAddons);

        /// <summary>
        ///     Deletes an existing API credential.
        /// </summary>
        /// <param name="organizationId"> The customer the API credential is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credential is used with. </param>
        /// <param name="serviceTypeId"> The service-type the API credentials can be used with. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task DeleteApiCredentialsAsync(Guid organizationId, int serviceProviderId, int? serviceTypeId);

        /// <summary>
        ///     Adds or updates an API credential for a customer's service-provider configuration.
        ///     
        ///     <para>
        ///     If an existing credential already exist (using the same unique combination of <paramref name="organizationId"/>, 
        ///     <paramref name="serviceProviderId"/> and <see cref="NewApiCredential.ServiceTypeId"/>), then it will be overwritten using the new values. </para>
        /// </summary>
        /// <param name="organizationId"> The customer the API credential is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credential is used with. </param>
        /// <param name="apiCredential"> The API credential details to add or update. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task AddOrUpdateApiCredentialAsync(Guid organizationId, int serviceProviderId, NewApiCredential apiCredential);

        /// <summary>
        ///     Adds new service-order addons to a customer's service-provider configuration. (<see cref="CustomerServiceProvider"/>). Pre-existing items will not be affected.
        ///     
        ///     <para>
        ///     You may only add service-order addons that is provided by the corresponding <paramref name="serviceProviderId"/>. </para>
        /// </summary>
        /// <param name="organizationId"> The customer/organization that's being configured. </param>
        /// <param name="serviceProviderId"> The service-provider that's being configured. </param>
        /// <param name="newServiceOrderAddonIds"> A list containing the service-order IDs that should be added. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task AddServiceAddonFromBackofficeAsync(Guid organizationId, int serviceProviderId, ISet<int> newServiceOrderAddonIds);

        /// <inheritdoc cref="AddServiceAddonFromBackofficeAsync(Guid, int, ISet{int})"/>
        /// <remarks>
        ///     It is only possible to add new <paramref name="newServiceOrderAddonIds"/> where 
        ///     "<c><see cref="ServiceOrderAddon.IsCustomerTogglable"/> == <see langword="true"/></c>.
        /// </remarks>
        /// <exception cref="ArgumentException"> Thrown when one or more of the inputs is invalid. </exception>
        Task AddServiceAddonFromCustomerPortalAsync(Guid organizationId, int serviceProviderId, ISet<int> newServiceOrderAddonIds);

        /// <summary>
        ///     Removes service-order addons from a customer's service-provider configuration. (<see cref="CustomerServiceProvider"/>).
        /// </summary>
        /// <param name="organizationId"> The customer/organization that's being configured. </param>
        /// <param name="serviceProviderId"> The service-provider that's being configured. </param>
        /// <param name="removedServiceOrderAddonIds"> A list containing the service-order IDs that should be removed. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task RemoveServiceAddonFromBackofficeAsync(Guid organizationId, int serviceProviderId, ISet<int> removedServiceOrderAddonIds);

        /// <inheritdoc cref="RemoveServiceAddonFromBackofficeAsync(Guid, int, ISet{int})"/>
        /// <remarks>
        ///     It is only possible to add new <paramref name="removedServiceOrderAddonIds"/> where 
        ///     "<c><see cref="ServiceOrderAddon.IsCustomerTogglable"/> == <see langword="true"/></c>.
        /// </remarks>
        /// <exception cref="ArgumentException"> Thrown when one or more of the inputs is invalid. </exception>
        Task RemoveServiceAddonFromCustomerPortalAsync(Guid organizationId, int serviceProviderId, ISet<int> removedServiceOrderAddonIds);

        /// <summary>
        ///     Creates a new Service Order for a particular Service Type. The Service Type could be SUR, Remarketing and so on
        /// </summary>
        /// <param name="customerId"> The customer/organization identifier </param>
        /// <param name="userId"> The user identifier </param>
        /// <param name="serviceTypeId"> Id of a ServiceType. This value resembles the values of enum <b>ServiceTypeEnum</b> </param>
        /// <param name="model"> The request body to create a new Service Order </param>
        /// <param name="asset"> The Asset for which User wants to create a Service Order </param>
        /// <exception cref="HttpRequestException"> Thrown when an HttpException happens during calling a third party service. </exception>
        /// <exception cref="NotSupportedException"> Thrown when the content or input type is not valid. </exception>
        /// <exception cref="ArgumentException"> Thrown when one or more of the invalid inputs due to which the service is unable to fetch necessary data from third party </exception>
        /// <exception cref="Exception"> Thrown when any unknown error happens </exception>
        Task<HardwareServiceOrder?> CreateHardwareServiceOrderAsync(Guid customerId, Guid userId, int serviceTypeId, NewHardwareServiceOrder model, HardwareSuperType asset);

        /// <summary>
        ///     Retrieves all service-orders that matches the parameters.
        /// </summary>
        /// <param name="organizationId"> Filter the results to only contain this customer. </param>
        /// <param name="filterOptions"> A class containing the various query-parameter filters. </param>
        /// <param name="userId"> Filter the results to only contain this user. When the value is <see langword="null"/>, the filter is ignored. </param>
        /// <param name="serviceTypeId"> Filter the results to only contain this service-type. When the value is <see langword="null"/>, the filter is ignored. </param>
        /// <param name="activeOnly"> When <see langword="true"/>, only active/ongoing service-orders are retrieved. When <see langword="false"/>, the filter is ignored. </param>
        /// <param name="search"> If a value is provided, a lightweight "contains" search is applied to a few select key-properties. When <see langword="null"/>, the filter is ignored. </param>
        /// <param name="page"> The paginated page that should be retrieved. </param>
        /// <param name="limit"> The number of items to retrieve per <paramref name="page"/>. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the retrieved, paginated results. </returns>
        Task<PagedModel<HardwareServiceOrder>> GetAllServiceOrdersForOrganizationAsync(Guid organizationId, FilterOptionsForHardwareServiceOrder filterOptions, Guid? userId, int? serviceTypeId, bool activeOnly, string? search = null, int page = 1, int limit = 25);

        /// <summary>
        ///     Retrieves a service-order using it's ID.
        /// </summary>
        /// <param name="organizationId"> Filter the results to only contain this customer. </param>
        /// <param name="serviceOrderId"> The ID of the service-order that should be retrieved. </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result contains the retrieved order, or <see langword="null"/> if no orders were found. 
        /// </returns>
        Task<HardwareServiceOrder?> GetServiceOrderByIdAndOrganizationAsync(Guid organizationId, Guid serviceOrderId);


        /// <summary>
        ///     Retrieves the <see cref="CustomerSettings"/> for a given organization.
        /// </summary>
        /// <param name="organizationId"> The organization to retrieve. </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result contains the retrieved result, or <see langword="null"/>
        ///     of no results were found. 
        /// </returns>
        Task<CustomerSettings?> GetCustomerSettingsAsync(Guid organizationId);

        /// <summary>
        ///     Retrieves the <see cref="LoanDeviceSettings"/> for a given organization.
        /// </summary>
        /// <param name="organizationId"> The organization to retrieve. </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result contains the retrieved result, or <see langword="null"/>
        ///     of no results were found. 
        /// </returns>
        Task<LoanDeviceSettings?> GetCustomerLoanDeviceSettingsAsync(Guid organizationId);

        /// <summary>
        ///     Updates a existing <see cref="CustomerSettings"/> for the given organization. 
        ///     If not <see cref="CustomerSettings"/> exists, then it will be created.
        /// </summary>
        /// <param name="organizationId"> The organization identifier. </param>
        /// <param name="newCustomerSettings"> The new customer settings. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the created/updated entity. </returns>
        Task<CustomerSettings> AddOrUpdateCustomerSettings(Guid organizationId, NewCustomerSettings newCustomerSettings);
    }
}
