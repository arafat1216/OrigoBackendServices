using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface ICustomerServices
    {
        Task<IList<Customer>> GetCustomersAsync();
        Task<Customer> AddCustomerAsync(Customer newCustomer);
        Task<Customer> GetCustomerAsync(Guid customerId);
        Task<IList<ProductModuleGroup>> GetCustomerProductModulesAsync(Guid customerId);
        Task<ProductModuleGroup> AddProductModulesAsync(Guid customerId, Guid moduleGroupId);
        Task<ProductModuleGroup> RemoveProductModulesAsync(Guid customerId, Guid moduleGroupId);
        Task<ProductModuleGroup> GetProductModuleGroup(Guid moduleGroupId);
    }
}