using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using Microsoft.Extensions.Logging;

namespace CustomerServices
{
    public class UserServices : IUserServices
    {
        private readonly ILogger<UserServices> _logger;
        private readonly ICustomerRepository _customerRepository;

        public UserServices(ILogger<UserServices> logger, ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public Task<IList<User>> GetAllUsersAsync(Guid customerId)
        {
            return _customerRepository.GetAllUsersAsync(customerId);
        }

        public Task<User> GetUserAsync(Guid customerId, Guid userId)
        {
            return _customerRepository.GetUserAsync(customerId, userId);
        }

        public async Task<User> AddUserForCustomerAsync(Guid customerId, string firstName, string lastName, string email, string mobileNumber, string employeeId)
        {
            var customer = await _customerRepository.GetCustomerAsync(customerId);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }

            var newUser = new User(customer, Guid.NewGuid(), firstName, lastName, email, mobileNumber, employeeId);

            return await _customerRepository.AddUserAsync(newUser);
        }
    }
}