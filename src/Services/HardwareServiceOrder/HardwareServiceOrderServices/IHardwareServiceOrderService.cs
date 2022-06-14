using Common.Interfaces;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices
{
    public interface IHardwareServiceOrderService
    {
        // Configuration
        Task<CustomerSettingsDTO> ConfigureServiceIdAsync(Guid customerId, CustomerSettingsDTO customerSettings, Guid callerId);
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
