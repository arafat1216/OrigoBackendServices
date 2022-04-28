using OrigoApiGateway.Models.HardwareServiceOrder;
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
        Task<HardwareServiceOrder> CreateHardwareServiceOrderAsync(Guid customerId, HardwareServiceOrder model);
        Task<HardwareServiceOrder> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId);
        Task<List<HardwareServiceOrder>> GetHardwareServiceOrdersAsync(Guid customerId);
        Task<HardwareServiceOrder> UpdateHardwareServiceOrderAsync(Guid customerId, Guid orderId, HardwareServiceOrder model);
        Task<List<HardwareServiceOrderLog>> GetHardwareServiceOrderLogsAsync(Guid customerId, Guid orderId);
    }
}
