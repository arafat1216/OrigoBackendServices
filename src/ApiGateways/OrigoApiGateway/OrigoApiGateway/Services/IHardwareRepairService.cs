using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IHardwareRepairService
    {
        //Configuration
        Task<CustomerSettings> ConfigureServiceIdAsync(Guid customerId, string serviceId);
        Task<CustomerSettings> ConfigureLoanPhoneAsync(Guid customerId, LoanDevice loanDevice);
        Task<CustomerSettings> GetSettingsAsync(Guid customerId);

        //Hardware Service Order
        Task<OrigoHardwareServiceOrder> CreateHardwareServiceOrderAsync(Guid customerId, Guid userId, NewHardwareServiceOrder model);
        Task<OrigoHardwareServiceOrder> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId);
        Task<List<OrigoHardwareServiceOrder>> GetHardwareServiceOrdersAsync(Guid customerId);
        Task<OrigoHardwareServiceOrder> UpdateHardwareServiceOrderAsync(Guid customerId, Guid orderId, NewHardwareServiceOrder model);
        Task<List<HardwareServiceOrderLog>> GetHardwareServiceOrderLogsAsync(Guid customerId, Guid orderId);
    }
}
