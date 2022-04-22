using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices
{
    public interface IHardwareServiceOrderService
    {
        Task<CustomerSettingsDTO> ConfigureServiceIdAsync(Guid customerId, string serviceId, Guid callerId);
        Task<CustomerSettingsDTO> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmain, Guid callerId);
        Task<CustomerSettingsDTO> GetSettingsAsync(Guid customerId);
    }
}
