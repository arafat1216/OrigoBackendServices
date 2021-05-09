using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerServices.Models;

namespace CustomerServices
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerServices(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IList<Customer>> GetCustomersAsync()
        {
            return await _customerRepository.GetCustomersAsync();
        }

        public async Task<Customer> AddCustomerAsync(Customer newCustomer)
        {
            return await _customerRepository.AddAsync(newCustomer);
        }

        public async Task<Customer> GetCustomerAsync(Guid customerId)
        {
            return await _customerRepository.GetCustomerAsync(customerId);
        }
    }
}