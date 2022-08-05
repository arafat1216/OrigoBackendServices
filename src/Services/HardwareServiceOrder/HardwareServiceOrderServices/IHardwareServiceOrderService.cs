using Common.Interfaces;
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


        /// <summary>
        ///     Returns a list containing all service-providers in the system.
        /// </summary>
        /// <param name="includeSupportedServiceTypes"> Should <see cref="ServiceProviderDTO.SupportedServiceTypes"/> be loaded and included in the result? </param>
        /// <param name="includeOfferedServiceOrderAddons"> Should <see cref="ServiceProviderDTO.OfferedServiceOrderAddons"/> be loaded and included in the result? </param>
        /// <returns> A list of all service-providers. </returns>
        Task<IEnumerable<ServiceProviderDTO>> GetAllServiceProvidersAsync(bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons);
    }
}
