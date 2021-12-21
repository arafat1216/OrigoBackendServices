using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using Microsoft.Extensions.Logging;

namespace CustomerServices
{
    public class UserServices : IUserServices
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<UserServices> _logger;
        private readonly IOrganizationRepository _customerRepository;
        private readonly IOktaServices _oktaServices;
        private readonly IMapper _mapper;
        private readonly IUserPermissionServices _userPermissionServices;

        public UserServices(ILogger<UserServices> logger, IOrganizationRepository customerRepository,
            IOktaServices oktaServices, IMapper mapper, IUserPermissionServices userPermissionServices)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            _oktaServices = oktaServices;
            _mapper = mapper;
            _userPermissionServices = userPermissionServices;
        }

        public Task<int> GetUsersCountAsync(Guid customerId)
        {
            return _customerRepository.GetUsersCount(customerId);
        }

        public async Task<IList<UserDTO>> GetAllUsersAsync(Guid customerId)
        {
            var allUsers = await _customerRepository.GetAllUsersAsync(customerId);
            var list = new List<UserDTO>();
            foreach (var user in allUsers)
            {
                var userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Role = await GetRoleNameForUser(user.Email);
                list.Add(userDTO);
            }

            return list;
        }

        private async Task<string> GetRoleNameForUser(string userEmail)
        {
            var userPermissions = await _userPermissionServices.GetUserPermissionsAsync(userEmail);
            return userPermissions != null && userPermissions.Any()
                ? userPermissions.FirstOrDefault()?.Role.Name
                : string.Empty;
        }

        public async Task<User> GetUserAsync(Guid customerId, Guid userId)
        {
            return await _customerRepository.GetUserAsync(customerId, userId);
        }

        public async Task<UserDTO> GetUserWithRoleAsync(Guid customerId, Guid userId)
        {
            var userDTO = _mapper.Map<UserDTO>(await _customerRepository.GetUserAsync(customerId, userId));
            userDTO.Role = await GetRoleNameForUser(userDTO.Email);
            return userDTO;
        }

        public async Task<UserDTO> AddUserForCustomerAsync(Guid customerId, string firstName, string lastName,
            string email, string mobileNumber, string employeeId, UserPreference userPreference)
        {
            var customer = await _customerRepository.GetOrganizationAsync(customerId);
            if (customer == null) throw new CustomerNotFoundException();
            if (userPreference == null || userPreference.Language == null)
                // Set a default language setting
                userPreference = new UserPreference("EN");
            var newUser = new User(customer, Guid.NewGuid(), firstName, lastName, email, mobileNumber, employeeId,
                userPreference);

            newUser = await _customerRepository.AddUserAsync(newUser);

            var oktaUserId = await _oktaServices.AddOktaUserAsync(newUser.UserId, newUser.FirstName, newUser.LastName,
                newUser.Email, newUser.MobileNumber, true);
            newUser = await AssignOktaUserIdAsync(newUser.Customer.OrganizationId, newUser.UserId, oktaUserId);

            return _mapper.Map<UserDTO>(newUser);
        }

        private async Task<User> AssignOktaUserIdAsync(Guid customerId, Guid userId, string oktaUserId)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null)
                throw new UserNotFoundException($"Unable to find {userId}");
            user.ActivateUser(oktaUserId);
            await _customerRepository.SaveEntitiesAsync();
            return user;
        }

        public async Task<UserDTO> SetUserActiveStatus(Guid customerId, Guid userId, bool isActive)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null)
                throw new UserNotFoundException($"Unable to find {userId}");

            // Do not call if there is no change
            if (isActive == user.IsActive)
                return _mapper.Map<UserDTO>(user);

            var userExistsInOkta = await _oktaServices.UserExistsInOktaAsync(user.OktaUserId);
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
                    var oktaUserId = await _oktaServices.AddOktaUserAsync(user.UserId, user.FirstName, user.LastName,
                        user.Email, user.MobileNumber, true);
                    user = await AssignOktaUserIdAsync(user.Customer.OrganizationId, user.UserId, oktaUserId);
                }
                else
                {
                    user.DeactivateUser();
                }
            }

            await _customerRepository.SaveEntitiesAsync();
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateUserPatchAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, UserPreference userPreference)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null) return null;
            if (firstName != default && user.FirstName != firstName) user.ChangeFirstName(firstName);
            if (lastName != default && user.LastName != lastName) user.ChangeLastName(lastName);
            if (email != default && user.Email != email) user.ChangeEmailAddress(email);
            if (employeeId != default && user.EmployeeId != employeeId) user.ChangeEmployeeId(employeeId);
            if (userPreference != null && userPreference.Language != null &&
                userPreference.Language != user.UserPreference?.Language)
                user.ChangeUserPreferences(userPreference);

            await _customerRepository.SaveEntitiesAsync();
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateUserPutAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, UserPreference userPreference)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null) return null;

            user.ChangeFirstName(firstName);
            user.ChangeLastName(lastName);
            user.ChangeEmailAddress(email);
            user.ChangeEmployeeId(employeeId);
            if (userPreference != null)
                user.ChangeUserPreferences(userPreference);

            await _customerRepository.SaveEntitiesAsync();
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> DeleteUserAsync(Guid userId, bool softDelete = true)
        {
            var user = await _customerRepository.GetUserAsync(userId);
            if (user == null) return null;
            if (user.IsDeleted && !softDelete)
                await _customerRepository.DeleteUserAsync(user);
            if (user.IsDeleted && softDelete)
                throw new UserDeletedException();

            user.SetDeleteStatus(true);
            await _customerRepository.SaveEntitiesAsync();
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> AssignDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null || department == null)
                return null;
            user.AssignDepartment(department);
            await _customerRepository.SaveEntitiesAsync();
            return _mapper.Map<UserDTO>(user);
        }

        public async Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null) throw new UserNotFoundException($"Unable to find {userId}");
            if (department == null)
            {
                throw new DepartmentNotFoundException($"Unable to find {departmentId}");
            }

            user.AssignManagerToDepartment(department);
            await _customerRepository.SaveEntitiesAsync();
        }

        public async Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null) throw new UserNotFoundException($"Unable to find {userId}");
            if (department == null)
            {
                throw new DepartmentNotFoundException($"Unable to find {departmentId}");
            }

            user.UnassignManagerFromDepartment(department);
            await _customerRepository.SaveEntitiesAsync();
        }

        public async Task<UserDTO> UnassignDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null || department == null)
                return null;
            user.UnassignDepartment(department);
            await _customerRepository.SaveEntitiesAsync();
            return _mapper.Map<UserDTO>(user);
        }
    }
}