using Common.Interfaces;
using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;
using System;
using System.Collections.Generic;
using System.Threading;
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
        Task<OrigoHardwareServiceOrderDetail> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId);
        Task<PagedModel<OrigoHardwareServiceOrder>> GetHardwareServiceOrdersAsync(Guid customerId, Guid? userId, bool activeOnly, int page = 1, int limit = 25);
        Task<List<HardwareServiceOrderLog>> GetHardwareServiceOrderLogsAsync(Guid customerId, Guid orderId);
    }
}
