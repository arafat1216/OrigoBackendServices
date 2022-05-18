namespace HardwareServiceOrderServices.Models
{
    public interface IHardwareServiceOrderRepository
    {
        Task<CustomerSettings> ConfigureServiceIdAsync(Guid customerId, string serviceId, Guid callerId);
        Task<CustomerSettings> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmain, Guid callerId);
        Task<CustomerSettings> GetSettingsAsync(Guid customerId);
        Task<IEnumerable<HardwareServiceOrder>> GetAllOrdersAsync(DateTime? olderThan = null, List<int>? statusIds = null);
        Task<HardwareServiceOrder> GetOrderByIdAsync(Guid orderId);
        Task<List<HardwareServiceOrder>> GetAllOrdersAsync(Guid customerId);
    }
}
