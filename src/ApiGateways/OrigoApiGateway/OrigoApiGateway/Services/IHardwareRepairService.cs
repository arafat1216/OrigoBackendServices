using Common.Interfaces;
using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;

#nullable enable

namespace OrigoApiGateway.Services
{
    [Obsolete("This is superseded by the new 'Hardware Service' service-class. All new functionality should instead be placed in that one.")]
    public interface IHardwareRepairService
    {
        // Configuration
        Task<CustomerSettings> ConfigureServiceIdAsync(Guid customerId, string serviceId);
        Task<CustomerSettings> ConfigureLoanPhoneAsync(Guid customerId, LoanDevice loanDevice);
        Task<CustomerSettings> GetSettingsAsync(Guid customerId);

        // Hardware Service Order
        Task<HardwareServiceOrder> CreateHardwareServiceOrderAsync(Guid customerId, Guid userId, NewHardwareServiceOrder model);
        Task<HardwareServiceOrder> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId);
        Task<PagedModel<HardwareServiceOrder>> GetHardwareServiceOrdersAsync(Guid customerId, Guid? userId, int? serviceTypeId, bool activeOnly, int page = 1, int limit = 25);
    }
}
