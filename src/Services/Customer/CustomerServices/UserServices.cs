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
        private readonly IOktaServices _oktaServices;

        public UserServices(ILogger<UserServices> logger, IOrganizationRepository customerRepository, IOktaServices oktaServices)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            _oktaServices = oktaServices;
        }

        public Task<IList<User>> GetAllUsersAsync(Guid customerId)
        {
            return _customerRepository.GetAllUsersAsync(customerId);
        }

        public Task<User> GetUserAsync(Guid customerId, Guid userId)
        {
            return _customerRepository.GetUserAsync(customerId, userId);
        }

        public async Task<User> AddUserForCustomerAsync(Guid customerId, string firstName, string lastName, string email, string mobileNumber, string employeeId, UserPreference userPreference)
        {
            var customer = await _customerRepository.GetOrganizationAsync(customerId);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
            if(userPreference == null || userPreference.Language == null)
            {
                // Set a default language setting
                userPreference = new UserPreference("EN");
            }
            var newUser = new User(customer, Guid.NewGuid(), firstName, lastName, email, mobileNumber, employeeId, userPreference);

            newUser = await _customerRepository.AddUserAsync(newUser);

            var oktaUserId = await _oktaServices.AddOktaUserAsync(newUser.UserId, newUser.FirstName, newUser.LastName, newUser.Email, newUser.MobileNumber, true);
            newUser = await AssignOktaUserIdAsync(newUser.Customer.OrganizationId, newUser.UserId, oktaUserId);

            return newUser;
        }

        public async Task<User> AssignOktaUserIdAsync(Guid customerId, Guid userId, string oktaUserId)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null)
                throw new UserNotFoundException($"Unable to find {userId}");
            user.ActivateUser(oktaUserId);
            await _customerRepository.SaveEntitiesAsync();
            return user;
        }

        public async Task<User> SetUserActiveStatus(Guid customerId, Guid userId, bool isActive)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null)
                throw new UserNotFoundException($"Unable to find {userId}");

            // Do not call if there is no change
            if (isActive == user.IsActive)
                return user;

            bool userExistsInOkta = await _oktaServices.UserExistsInOkta(user.OktaUserId);
            if (userExistsInOkta)
            {
                if (isActive)
                {
                    // set active, but reuse the userId set on creation of user : (This may change in future)
                    await _oktaServices.AddUserToGroup(user.OktaUserId);
                    user.ActivateUser(user.OktaUserId);
                }
                else
                {
                    await _oktaServices.RemoveUserFromGroup(user.OktaUserId);
                    user.DeactivateUser();
                }
            }
            else
            {
                if (isActive)
                {
                    var oktaUserId = await _oktaServices.AddOktaUserAsync(user.UserId, user.FirstName, user.LastName, user.Email, user.MobileNumber, true);
                    user = await AssignOktaUserIdAsync(user.Customer.OrganizationId, user.UserId, oktaUserId);
                }
                else
                {
                    user.DeactivateUser();
                }
            }

            await _customerRepository.SaveEntitiesAsync();
            return user;
        }

        public async Task<User> UpdateUserPatchAsync(Guid customerId, Guid userId, string firstName, string lastName, string email, string employeeId, UserPreference userPreference)
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
            if (userPreference != null &&
                userPreference.Language != null &&
                userPreference?.Language != user.UserPreference.Language)
            {
                user.ChangeUserPreferences(userPreference);
            }

            await _customerRepository.SaveEntitiesAsync();
            return user;
        }

        public async Task<User> UpdateUserPutAsync(Guid customerId, Guid userId, string firstName, string lastName, string email, string employeeId, UserPreference userPreference)
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
            if (userPreference != null)
                user.ChangeUserPreferences(userPreference);

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

            user.SetDeleteStatus(true);
            await _customerRepository.SaveEntitiesAsync();
            return user;
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
            if (user == null)
            {
                throw new UserNotFoundException($"Unable to find {userId}");
            }
            if (department == null)
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