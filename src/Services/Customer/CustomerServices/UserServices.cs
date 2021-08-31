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

        public async Task<User> UpdateUserPostAsync(Guid customerId, Guid userId, string firstName, string lastName, string email, string employeeId)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null)
            {
                return null;
            }
            if (firstName != default && user.FirstName != firstName)
            {
                user.ChangeFirstName(firstName);
            }
            if (lastName != default && user.LastName != lastName)
            {
                user.ChangeLastName(lastName);
            }
            if (email != default && user.Email != email)
            {
                user.ChangeEmailAddress(email);
            }
            if (employeeId != default && user.EmployeeId != employeeId)
            {
                user.ChangeEmployeeId(employeeId);
            }

            await _customerRepository.SaveEntitiesAsync();
            return user;
        }

        public async Task<User> UpdateUserPatchAsync(Guid customerId, Guid userId, string firstName, string lastName, string email, string employeeId)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null)
            {
                return null;
            }

            user.ChangeFirstName(firstName);
            user.ChangeLastName(lastName);
            user.ChangeEmailAddress(email);
            user.ChangeEmployeeId(employeeId);

            await _customerRepository.SaveEntitiesAsync();
            return user;
        }

        public async Task<User> DeleteUserAsync(Guid userId, bool softDelete = true)
        {
            var user = await _customerRepository.GetUserAsync(userId);
            if (user == null)
            {
                return null;
            }
            if (user.IsDeleted && !softDelete)
                await _customerRepository.DeleteUserAsync(user);
            if (user.IsDeleted && softDelete)
                throw new UserDeletedException();
            user.IsDeleted = true;
            _customerRepository.SaveEntitiesAsync();
            return user;
        }
    }
}