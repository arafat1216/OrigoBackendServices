using Common.Interfaces;
using Common.Seedwork;
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

        // Order
        Task<HardwareServiceOrderDTO> CreateHardwareServiceOrderAsync(Guid customerId, NewHardwareServiceOrderDTO serviceOrderDTO);
        Task<HardwareServiceOrderDTO> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId);
        Task<PagedModel<HardwareServiceOrderDTO>> GetHardwareServiceOrdersAsync(Guid customerId, Guid? userId, bool activeOnly, CancellationToken cancellationToken, int page = 1, int limit = 25);

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


        /*
         * Customer Service Provider
         */


        Task<IEnumerable<CustomerServiceProviderDto>> GetCustomerServiceProvidersAsync(Guid organizationId, bool includeApiCredentials = false);


        /// <summary>
        ///     Retrieves the <see cref="CustomerServiceProvider"/>-configuration that's used between a given customer and service-provider.
        /// </summary>
        /// <param name="organizationId"> The customers identifier. </param>
        /// <param name="serviceProviderId"> The service-provider's identifier. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the matching customer-service-provider details. </returns>
        /// <exception cref="NotFoundException"> Thrown if no matching customer-service-providers were found. </exception>
        Task<CustomerServiceProviderDto> GetCustomerServiceProviderByIdAsync(Guid organizationId, int serviceProviderId, bool includeApiCredentials = false);


        /// <summary>
        ///     Deletes an existing API credential.
        /// </summary>
        /// <param name="organizationId"> The customer the API credential is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credential is used with. </param>
        /// <param name="serviceTypeId"> The service-type the API credentials can be used with. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task DeleteApiCredentialAsync(Guid organizationId, int serviceProviderId, int serviceTypeId);

        /// <summary>
        ///     Register a new API credential.
        /// </summary>
        /// <param name="organizationId"> The customer the API credential is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credential is used with. </param>
        /// <param name="serviceTypeId"> The <see cref="ServiceType.Id"/> this API credential is valid for. </param>
        /// <param name="apiUsername"> The API username. If it's not applicable for the service-provider, it should be <see langword="null"/>. </param>
        /// <param name="apiPassword"> The API password. If it's not applicable for the service-provider, it should be <see langword="null"/>. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task AddOrUpdateApiCredentialAsync(Guid organizationId, int serviceProviderId, int serviceTypeId, string? apiUsername, string? apiPassword);



    }
}
