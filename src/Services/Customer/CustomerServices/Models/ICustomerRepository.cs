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
        Task<IList<User>> GetAllUsersAsync(Guid customerId);
        Task<User> GetUserAsync(Guid customerId, Guid userId);
        Task<User> AddUserAsync(User newUser);
        Task<IList<Module>> GetModulesAsync();
    }
}
