using AutoMapper;
using Common.Interfaces;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<PagedModel<UserDTO>> GetAllUsersAsync(Guid customerId, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 100)
        {
            var allUsers = await _customerRepository.GetAllUsersAsync(customerId,cancellationToken, search, page, limit);
            var list = new List<UserDTO>();
            foreach (var user in allUsers.Items)
            {
                var userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Role = await GetRoleNameForUser(user.Email);
                list.Add(userDTO);
            }

            return new PagedModel<UserDTO>
            {
                Items = list,
                CurrentPage = allUsers.CurrentPage,
                PageSize = allUsers.PageSize,
                TotalItems = allUsers.TotalItems,
                TotalPages = allUsers.TotalPages
            };
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
            if (userDTO == null)
                return null;
            userDTO.Role = await GetRoleNameForUser(userDTO.Email);
            return userDTO;
        }

        public async Task<UserDTO> GetUserWithRoleAsync(Guid userId)
        {
            var userDTO = _mapper.Map<UserDTO>(await _customerRepository.GetUserAsync(userId));
            if (userDTO == null)
                return null;
            userDTO.Role = await GetRoleNameForUser(userDTO.Email);
            return userDTO;
        }

        public async Task<UserDTO> AddUserForCustomerAsync(Guid customerId, string firstName, string lastName,
            string email, string mobileNumber, string employeeId, UserPreference userPreference, Guid callerId, string role)
        {
            var customer = await _customerRepository.GetOrganizationAsync(customerId);
            if (customer == null) throw new CustomerNotFoundException();
            if (userPreference == null || userPreference.Language == null)
            {
                // Set a default language setting
                userPreference = new UserPreference("EN", callerId);
            }
            // Check if email address is used by another user
            var emailInUse = await _customerRepository.GetUserByUserName(email);
            if (emailInUse != null)
                throw new UserNameIsInUseException("Email address is already in use.");

            //Check if mobile number is used by another user
            var mobileNumberInUse = await _customerRepository.GetUserByMobileNumber(mobileNumber);
            if (mobileNumberInUse != null) throw new InvalidPhoneNumberException("Phone number already in use.");

            var newUser = new User(customer, Guid.NewGuid(), firstName, lastName, email, mobileNumber, employeeId,
                userPreference, callerId);

            newUser = await _customerRepository.AddUserAsync(newUser);

            var oktaUserId = await _oktaServices.AddOktaUserAsync(newUser.UserId, newUser.FirstName, newUser.LastName,
                newUser.Email, newUser.MobileNumber, true);
            newUser = await AssignOktaUserIdAsync(newUser.Customer.OrganizationId, newUser.UserId, oktaUserId, callerId);


            var mappedNewUserDTO = _mapper.Map<UserDTO>(newUser);

            //Add user permission if role is added in the request - type of role gets checked in AssignUserPermissionAsync
            if (role != null)
            {
                try
                {
                    var userPermission = await _userPermissionServices.AssignUserPermissionsAsync(email, role, new List<Guid>() { customerId }, callerId);
                    if (userPermission != null)
                    {
                        mappedNewUserDTO.Role = role;
                    }
                }
                catch (InvalidRoleNameException)
                {
                    mappedNewUserDTO.Role = null;
                }
            }

            return mappedNewUserDTO;
        }

        private async Task<User> AssignOktaUserIdAsync(Guid customerId, Guid userId, string oktaUserId, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null)
                throw new UserNotFoundException($"Unable to find {userId}");
            user.ActivateUser(oktaUserId, callerId);
            await _customerRepository.SaveEntitiesAsync();
            return user;
        }

        public async Task<UserDTO> SetUserActiveStatus(Guid customerId, Guid userId, bool isActive, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null)
                throw new UserNotFoundException($"Unable to find {userId}");

            //get role from current email
            var role = await GetRoleNameForUser(user.Email);
            UserDTO userDTO;

            // Do not call if there is no change
            if (isActive == user.IsActive)
            {
                userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Role = role;
                return userDTO;
            }

            var userExistsInOkta = await _oktaServices.UserExistsInOktaAsync(user.OktaUserId);
            if (userExistsInOkta)
            {
                if (isActive)
                {
                    // set active, but reuse the userId set on creation of user : (This may change in future)
                    await _oktaServices.AddUserToGroup(user.OktaUserId);
                    user.ActivateUser(user.OktaUserId, callerId);
                }
                else
                {
                    await _oktaServices.RemoveUserFromGroup(user.OktaUserId);
                    user.DeactivateUser(callerId);
                }
            }
            else
            {
                if (isActive)
                {
                    var oktaUserId = await _oktaServices.AddOktaUserAsync(user.UserId, user.FirstName, user.LastName,
                        user.Email, user.MobileNumber, true);
                    user = await AssignOktaUserIdAsync(user.Customer.OrganizationId, user.UserId, oktaUserId, callerId);
                }
                else
                {
                    user.DeactivateUser(callerId);
                }
            }

            userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = role;

            await _customerRepository.SaveEntitiesAsync();
            return userDTO;
        }

        public async Task<UserDTO> UpdateUserPatchAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, string mobileNumber, UserPreference userPreference, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            //get role from current email
            var role = await GetRoleNameForUser(user.Email);

            if (user == null) return null;
            if (firstName != default && user.FirstName != firstName) user.ChangeFirstName(firstName, callerId);
            if (lastName != default && user.LastName != lastName) user.ChangeLastName(lastName, callerId);
            if (email != default && user.Email?.ToLower().Trim() != email?.ToLower().Trim())
            {
                // Check if email address is used by another user
                var username = await _customerRepository.GetUserByUserName(email);
                if (username != null && username.Id != user.Id)
                    throw new UserNameIsInUseException("Email address is already in use.");
                user.ChangeEmailAddress(email, callerId);
            }
            if (employeeId != default && user.EmployeeId != employeeId) user.ChangeEmployeeId(employeeId, callerId);
            if (mobileNumber != default && user.MobileNumber?.Trim() != mobileNumber?.Trim())
            {
                // Check if mobile address is used by another user
                var mobileNumberInUse = await _customerRepository.GetUserByMobileNumber(mobileNumber);
                if (mobileNumberInUse != null) throw new InvalidPhoneNumberException("Phone number already in use.");
                user.ChangeMobileNumber(mobileNumber, callerId);
            }
            if (userPreference != null && userPreference.Language != null &&
                userPreference.Language != user.UserPreference?.Language)
                user.ChangeUserPreferences(userPreference, callerId);

            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            if (userDTO == null)
            {
                return null;
            }

            userDTO.Role = role;

            await _customerRepository.SaveEntitiesAsync();
            return userDTO;
        }

        public async Task<UserDTO> UpdateUserPutAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, string mobileNumber, UserPreference userPreference, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null) return null;
            //Need to fetch the role based on the mail already registered on the user in the case there is a change in email from put and then resulting in no role for mail
            //The role assignment for response get assign after mapping to user DTO
            var role = await GetRoleNameForUser(user.Email);

            user.ChangeFirstName(firstName, callerId);
            user.ChangeLastName(lastName, callerId);
            if (email != default && user.Email?.ToLower().Trim() != email?.ToLower().Trim())
            {
                // Check if email address is used by another user
                var username = await _customerRepository.GetUserByUserName(email);
                if (username != null && username.Id != user.Id)
                    throw new UserNameIsInUseException("Email address is already in use.");
            }
            user.ChangeEmailAddress(email, callerId);
            user.ChangeEmployeeId(employeeId, callerId);
            if (mobileNumber != default && user.MobileNumber?.Trim() != mobileNumber?.Trim())
            {
                // Check if mobile address is used by another user
                var mobileNumberInUse = await _customerRepository.GetUserByMobileNumber(mobileNumber);
                if (mobileNumberInUse != null) throw new InvalidPhoneNumberException("Phone number already in use.");
            }
            user.ChangeMobileNumber(mobileNumber, callerId);
            if (userPreference != null)
            {
                user.ChangeUserPreferences(userPreference, callerId);
            }

            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            if (userDTO == null)
            {
                return null;
            }

            userDTO.Role = role;

            await _customerRepository.SaveEntitiesAsync();
            return userDTO;
        }

        public async Task<UserDTO> DeleteUserAsync(Guid userId, Guid callerId, bool softDelete = true)
        {
            var user = await _customerRepository.GetUserAsync(userId);
            if (user == null) return null;
            if (user.IsDeleted && !softDelete)
                await _customerRepository.DeleteUserAsync(user);
            if (user.IsDeleted && softDelete)
                throw new UserDeletedException();

            user.SetDeleteStatus(true, callerId);

            //Get the users role and assign it to the users DTO
            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = await GetRoleNameForUser(user.Email);

            await _customerRepository.SaveEntitiesAsync();
            return userDTO;
        }

        public async Task<UserDTO> AssignDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null || department == null)
                return null;
            user.AssignDepartment(department, callerId);

            //Get the users role and assign it to the users DTO
            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = await GetRoleNameForUser(user.Email);

            await _customerRepository.SaveEntitiesAsync();
            return userDTO;
        }

        public async Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null) throw new UserNotFoundException($"Unable to find {userId}");
            if (department == null)
            {
                throw new DepartmentNotFoundException($"Unable to find {departmentId}");
            }

            user.AssignManagerToDepartment(department, callerId);
            await _customerRepository.SaveEntitiesAsync();
        }

        public async Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null) throw new UserNotFoundException($"Unable to find {userId}");
            if (department == null)
            {
                throw new DepartmentNotFoundException($"Unable to find {departmentId}");
            }

            user.UnassignManagerFromDepartment(department, callerId);
            await _customerRepository.SaveEntitiesAsync();
        }

        public async Task<UserDTO> UnassignDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null || department == null)
                return null;
            user.UnassignDepartment(department, callerId);

            //Get the users role and assign it to the users DTO
            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = await GetRoleNameForUser(user.Email);

            await _customerRepository.SaveEntitiesAsync();
            return userDTO;
        }
    }
}