using OrigoApiGateway.Models.HardwareServiceOrder;
using System;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IHardwareRepairService
    {
        Task<CustomerSettings> ConfigureServiceIdAsync(Guid customerId, string serviceId);
        Task<CustomerSettings> ConfigureLoanPhoneAsync(Guid customerId, LoanDevice loanDevice);
        Task<CustomerSettings> GetSettingsAsync(Guid customerId);
    }
}
