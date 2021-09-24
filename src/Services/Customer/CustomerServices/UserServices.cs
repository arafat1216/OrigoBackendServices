using CustomerServices.Exceptions;
using CustomerServices.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class UserServices : IUserServices
    {
        private readonly ILogger<UserServices> _logger;
        private readonly IOrganizationRepository _customerRepository;

        public UserServices(ILogger<UserServices> logger, IOrganizationRepository customerRepository)
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
            var customer = await _customerRepository.GetOrganizationAsync(customerId);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }

            var newUser = new User(customer, Guid.NewGuid(), firstName, lastName, email, mobileNumber, employeeId);

            return await _customerRepository.AddUserAsync(newUser);
        }

        public async Task<User> AssignDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null || department == null)
                return null;
            user.AssignDepartment(department);
            await _customerRepository.SaveEntitiesAsync();
            return user;
        }

        public async Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null) {
                throw new UserNotFoundException($"Unable to find {userId}");
            }
            if(department == null)
            {
                throw new DepartmentNotFoundException($"Unable to find {departmentId}"); ;
            }
                
            user.AssignManagerToDepartment(department);
            await _customerRepository.SaveEntitiesAsync();
            return;
        }

        public async Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null)
            {
                throw new UserNotFoundException($"Unable to find {userId}");
            }
            if (department == null)
            {
                throw new DepartmentNotFoundException($"Unable to find {departmentId}"); ;
            }

            user.UnassignManagerFromDepartment(department);
            await _customerRepository.SaveEntitiesAsync();
            return;
        }

        public async Task<User> UnassignDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null || department == null)
                return null;
            user.UnassignDepartment(department);
            await _customerRepository.SaveEntitiesAsync();
            return user;
        }
    }
}