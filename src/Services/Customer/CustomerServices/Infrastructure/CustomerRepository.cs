using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _context;
        public CustomerRepository(CustomerContext context)
        {
            _context = context;
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<IList<Customer>> GetCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> GetCustomerAsync(Guid customerId)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<IList<User>> GetAllUsersAsync(Guid customerId)
        {
            return await _context.Users.Include(u => u.Customer).Where(u => u.Customer.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<User> GetUserAsync(Guid customerId, Guid userId)
        {
            return await _context.Users.Include(u => u.Customer).Where(u => u.Customer.CustomerId == customerId && u.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<User> AddUserAsync(User newUser)
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task<IList<ProductModule>> GetModulesAsync()
        {
            return await _context.ProductModules.Include(p => p.ProductModuleGroup).ToListAsync();
        }
    }
}
