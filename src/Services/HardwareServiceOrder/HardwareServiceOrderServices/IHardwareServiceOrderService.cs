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
        Task<CustomerSettingsDTO> ConfigureCustomerSettingsAsync(Guid customerId,  Guid callerId);
        Task<CustomerSettingsDTO> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmail, Guid callerId);
        Task<CustomerSettingsDTO> GetSettingsAsync(Guid customerId);

        // Order
        Task<HardwareServiceOrderDTO> CreateHardwareServiceOrderAsync(Guid customerId, NewHardwareServiceOrderDTO model);
        Task<HardwareServiceOrderDTO> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId);
        Task<PagedModel<HardwareServiceOrderDTO>> GetHardwareServiceOrdersAsync(Guid customerId, Guid? userId, bool activeOnly, CancellationToken cancellationToken, int page = 1, int limit = 25);
        Task<List<HardwareServiceOrderLogDTO>> GetHardwareServiceOrderLogsAsync(Guid customerId, Guid orderId);
        
        /// <summary>
        /// Update all order status since last updated datetime
        /// </summary>
        /// <returns></returns>
        Task UpdateOrderStatusAsync();
    }
}
