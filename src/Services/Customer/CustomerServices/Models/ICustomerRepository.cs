using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices.Models
{
    public interface ICustomerRepository
    {
        Customer Add(Customer customer);
        Task<IList<Customer>> GetCustomersAsync();
        Task<IList<Customer>> GetCustomerAsync(Guid customerId);

    }
}
