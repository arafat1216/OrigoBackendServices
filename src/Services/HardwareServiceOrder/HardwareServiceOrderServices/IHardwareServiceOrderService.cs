using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices
{
    public interface IHardwareServiceOrderService
    {
        //Configuration
        Task<CustomerSettingsDTO> ConfigureServiceIdAsync(Guid customerId, string serviceId, Guid callerId);
        Task<CustomerSettingsDTO> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmain, Guid callerId);
        Task<CustomerSettingsDTO> GetSettingsAsync(Guid customerId);

        //Order
        Task<HardwareServiceOrderDTO> CreateHardwareServiceOrderAsync(Guid customerId, HardwareServiceOrderDTO model);
        Task<HardwareServiceOrderDTO> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId);
        Task<List<HardwareServiceOrderDTO>> GetHardwareServiceOrdersAsync(Guid customerId);
        Task<HardwareServiceOrderDTO> UpdateHardwareServiceOrderAsync(Guid customerId, Guid orderId, HardwareServiceOrderDTO model);
        Task<List<HardwareServiceOrderLogDTO>> GetHardwareServiceOrderLogsAsync(Guid customerId, Guid orderId);
    }
}
