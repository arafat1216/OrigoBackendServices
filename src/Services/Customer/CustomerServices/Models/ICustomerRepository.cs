using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices.Models
{
    public interface ICustomerRepository
    {
        Task<Customer> AddAsync(Customer customer);
        Task<IList<Customer>> GetCustomersAsync();
        Task<Customer> GetCustomerAsync(Guid customerId);

    }
}
