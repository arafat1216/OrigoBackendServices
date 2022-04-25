namespace HardwareServiceOrderServices.Models
{
    public interface IHardwareServiceOrderRepository
    {
        Task<CustomerSettings> ConfigureServiceIdAsync(Guid customerId, string serviceId, Guid callerId);
        Task<CustomerSettings> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmain, Guid callerId);
        Task<CustomerSettings> GetSettingsAsync(Guid customerId);
    }
}
