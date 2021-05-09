using System;
using CustomerServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface ICustomerServices
    {
        Task<IList<Customer>> GetCustomersAsync();
        Task<Customer> AddCustomerAsync(Customer newCustomer);
        Task<Customer> GetCustomerAsync(Guid customerId);
    }
}