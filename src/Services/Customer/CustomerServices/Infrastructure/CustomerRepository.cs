using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerServices.Models;

namespace CustomerServices.Infrastructure
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _context;
        public CustomerRepository(CustomerContext context)
        {

        }

        public Customer Add(Customer customer)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Customer>> GetCustomersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<Customer>> GetCustomerAsync(Guid customerId)
        {
            throw new NotImplementedException();
        }
    }
}
