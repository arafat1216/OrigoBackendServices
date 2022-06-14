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
        /// <param name="assetCategoryIds">List of asset category id</param>
        /// <param name="providerId">Provider identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="apiUsername">Username for calling provider's API</param>
        /// <param name="apiPassword">Password for calling provider's API</param>
        /// <returns>Provider's apiUsername</returns>
        Task<string?> ConfigureCustomerServiceProviderAsync(List<int> assetCategoryIds, int providerId, Guid customerId, string? apiUsername, string? apiPassword);

        // Configuration
        /// <summary>
        /// Configure service id
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="customerSettings">Details of customer</param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task<CustomerSettingsDTO> ConfigureCustomerSettingsAsync(Guid customerId, CustomerSettingsDTO customerSettings, Guid callerId);
        Task<CustomerSettingsDTO> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmail, Guid callerId);
        Task<CustomerSettingsDTO> GetSettingsAsync(Guid customerId);

        // Order
        Task<HardwareServiceOrderResponseDTO> CreateHardwareServiceOrderAsync(Guid customerId, HardwareServiceOrderDTO model);
        Task<HardwareServiceOrderResponseDTO> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId);
        Task<PagedModel<HardwareServiceOrderResponseDTO>> GetHardwareServiceOrdersAsync(Guid customerId, Guid? userId, CancellationToken cancellationToken, int page = 1, int limit = 500);
        Task<List<HardwareServiceOrderLogDTO>> GetHardwareServiceOrderLogsAsync(Guid customerId, Guid orderId);
        /// <summary>
        /// Update all order status since last updated datetime
        /// </summary>
        /// <returns></returns>
        Task UpdateOrderStatusAsync();
        Task<HardwareServiceOrderDTO> UpdateHardwareServiceOrderAsync(Guid customerId, Guid orderId, HardwareServiceOrderDTO model);
    }
}
