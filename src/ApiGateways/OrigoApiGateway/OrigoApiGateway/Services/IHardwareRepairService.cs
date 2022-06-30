using Common.Interfaces;
using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;

#nullable enable

namespace OrigoApiGateway.Services
{
    public interface IHardwareRepairService
    {
        // Configuration
        Task<CustomerSettings> ConfigureServiceIdAsync(Guid customerId, string serviceId);
        Task<CustomerSettings> ConfigureLoanPhoneAsync(Guid customerId, LoanDevice loanDevice);
        Task<CustomerSettings> GetSettingsAsync(Guid customerId);

        // Hardware Service Order
        Task<HardwareServiceOrder> CreateHardwareServiceOrderAsync(Guid customerId, Guid userId, NewHardwareServiceOrder model);
        Task<HardwareServiceOrder> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId);
        Task<PagedModel<HardwareServiceOrder>> GetHardwareServiceOrdersAsync(Guid customerId, Guid? userId, bool activeOnly, int page = 1, int limit = 25);
    }
}
