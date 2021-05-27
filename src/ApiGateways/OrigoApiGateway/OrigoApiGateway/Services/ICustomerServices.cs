using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface ICustomerServices
    {
        Task<IList<OrigoCustomer>> GetCustomersAsync();
        Task<OrigoCustomer> GetCustomerAsync(Guid customerId);
        Task<OrigoCustomer> CreateCustomerAsync(OrigoNewCustomer newCustomer);
    }
}