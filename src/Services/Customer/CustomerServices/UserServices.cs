#nullable enable
using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Common.Extensions;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;

namespace CustomerServices
{
    public class UserServices : IUserServices
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<UserServices> _logger;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOktaServices _oktaServices;
        private readonly IMapper _mapper;
        private readonly IUserPermissionServices _userPermissionServices;

        public UserServices(ILogger<UserServices> logger, IOrganizationRepository customerRepository,
            IOktaServices oktaServices, IMapper mapper, IUserPermissionServices userPermissionServices)
        {
            _logger = logger;
            _organizationRepository = customerRepository;
            _oktaServices = oktaServices;
            _mapper = mapper;
            _userPermissionServices = userPermissionServices;
        }

        public Task<int> GetUsersCountAsync(Guid customerId)
        {
            return _organizationRepository.GetUsersCount(customerId);
        }

        public async Task<PagedModel<UserDTO>> GetAllUsersAsync(Guid customerId, string[]? role, Guid[]? assignedToDepartment, IList<int>? userStatus, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 100)
        {
            return await _organizationRepository.GetAllUsersAsync(customerId, role, assignedToDepartment, userStatus, cancellationToken, search, page, limit);
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
            return await _organizationRepository.GetUserAsync(customerId, userId);
        }

        public async Task<UserDTO> GetUserWithRoleAsync(Guid customerId, Guid userId)
        {
            var user = await _organizationRepository.GetUserAsync(userId);
            var userDTO = _mapper.Map<UserDTO>(user);
            if (userDTO == null)
                return null;
            userDTO.Role = await GetRoleNameForUser(userDTO.Email);

            if (user.Department != null)
            {
                var department = await _organizationRepository.GetDepartmentAsync(user.Customer.OrganizationId, user.Department.ExternalDepartmentId);
                userDTO.DepartmentName = department.Name;
            }

            return userDTO;
        }

        public async Task<UserDTO> GetUserWithRoleAsync(Guid userId)
        {
            var user = await _organizationRepository.GetUserAsync(userId);
            var userDTO = _mapper.Map<UserDTO>(user);
            if (userDTO == null)
                return null;
            userDTO.Role = await GetRoleNameForUser(userDTO.Email);

            if (user.Department != null)
            {
                var department = await _organizationRepository.GetDepartmentAsync(user.Customer.OrganizationId, user.Department.ExternalDepartmentId);
                userDTO.DepartmentName = department.Name;
            }

            return userDTO;
        }

        public async Task<UserDTO> AddUserForCustomerAsync(Guid customerId, string firstName, string lastName,
            string email, string mobileNumber, string employeeId, UserPreference userPreference, Guid callerId, string role)
        {
            var customer = await _organizationRepository.GetOrganizationAsync(customerId, includeDepartments: true);
            if (customer == null) throw new CustomerNotFoundException();
            if (userPreference == null || userPreference.Language == null)
            {
                // Set a default language setting
                userPreference = new UserPreference("EN", callerId);
            }
            // Check if email address is used by another user
            var emailInUse = await _organizationRepository.GetUserByUserName(email);
            if (emailInUse != null)
                throw new UserNameIsInUseException("Email address is already in use.");

            //Check if mobile number is used by another user
            var mobileNumberInUse = await _organizationRepository.GetUserByMobileNumber(mobileNumber);
            if (mobileNumberInUse != null) throw new InvalidPhoneNumberException("Phone number already in use.");

            var newUser = new User(customer, Guid.NewGuid(), firstName, lastName, email, mobileNumber, employeeId,
                userPreference, callerId);

            newUser = await _organizationRepository.AddUserAsync(newUser);
            if (customer.AddUsersToOkta)
            {
                var oktaUserId = await _oktaServices.AddOktaUserAsync(newUser.UserId, newUser.FirstName, newUser.LastName,
                    newUser.Email, newUser.MobileNumber, true);
                newUser = await AssignOktaUserIdAsync(newUser.Customer.OrganizationId, newUser.UserId, oktaUserId, callerId);
            }

            var mappedNewUserDTO = _mapper.Map<UserDTO>(newUser);

            //Add user permission if role is added in the request - type of role gets checked in AssignUserPermissionAsync
            UserPermissions? userPermission;
            
                if (role != null)
                {
                    try
                    {

                        userPermission = await _userPermissionServices.AssignUserPermissionsAsync(email, role, new List<Guid>() { customerId }, callerId);
                    }
                    catch (InvalidRoleNameException)
                    {
                        userPermission = await _userPermissionServices.AssignUserPermissionsAsync(email, PredefinedRole.EndUser.ToString(), new List<Guid>() { customerId }, callerId);

                    }
                }
                else
                {
                    userPermission = await _userPermissionServices.AssignUserPermissionsAsync(email, PredefinedRole.EndUser.ToString(), new List<Guid>() { customerId }, callerId);
                }

                if (userPermission != null)
                {
                   mappedNewUserDTO.Role = userPermission.Role.Name.ToString();
                }
                else
                {
                    mappedNewUserDTO.Role = null;
                }


            return mappedNewUserDTO;
        }

        private async Task<User> AssignOktaUserIdAsync(Guid customerId, Guid userId, string oktaUserId, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null)
                throw new UserNotFoundException($"Unable to find {userId}");
            user.ChangeUserStatus(oktaUserId,callerId,UserStatus.Activated);
            await _organizationRepository.SaveEntitiesAsync();
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
            if ((user.UserStatus == UserStatus.Activated && isActive) || (user.UserStatus == UserStatus.Deactivated && !isActive))
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
                    user.ChangeUserStatus(user.OktaUserId, callerId, UserStatus.Activated);
                }
                else
                {
                    await _oktaServices.RemoveUserFromGroupAsync(user.OktaUserId);
                    user.ChangeUserStatus(null,callerId,UserStatus.Deactivated);
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
                    user.ChangeUserStatus(null, callerId,UserStatus.Deactivated);
                }
            }

            userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = role;
            if (user.Department != null)
            {
                var department = await _organizationRepository.GetDepartmentAsync(user.Customer.OrganizationId, user.Department.ExternalDepartmentId);
                userDTO.DepartmentName = department.Name;
            }

            await _organizationRepository.SaveEntitiesAsync();
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
                var username = await _organizationRepository.GetUserByUserName(email);
                if (username != null && username.Id != user.Id)
                    throw new UserNameIsInUseException("Email address is already in use.");
                user.ChangeEmailAddress(email, callerId);
            }
            if (employeeId != default && user.EmployeeId != employeeId) user.ChangeEmployeeId(employeeId, callerId);
            if (mobileNumber != default && user.MobileNumber?.Trim() != mobileNumber?.Trim())
            {
                // Check if mobile address is used by another user
                var mobileNumberInUse = await _organizationRepository.GetUserByMobileNumber(mobileNumber);
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
            if (user.Department != null) 
            {
                var department = await _organizationRepository.GetDepartmentAsync(user.Customer.OrganizationId, user.Department.ExternalDepartmentId);
                userDTO.DepartmentName = department.Name;
            } 

            await _organizationRepository.SaveEntitiesAsync();
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
                var username = await _organizationRepository.GetUserByUserName(email);
                if (username != null && username.Id != user.Id)
                    throw new UserNameIsInUseException("Email address is already in use.");
            }
            user.ChangeEmailAddress(email, callerId);
            user.ChangeEmployeeId(employeeId, callerId);
            if (mobileNumber != default && user.MobileNumber?.Trim() != mobileNumber?.Trim())
            {
                // Check if mobile address is used by another user
                var mobileNumberInUse = await _organizationRepository.GetUserByMobileNumber(mobileNumber);
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
            if (user.Department != null)
            {
                var department = await _organizationRepository.GetDepartmentAsync(user.Customer.OrganizationId, user.Department.ExternalDepartmentId);
                userDTO.DepartmentName = department.Name;
            }

            await _organizationRepository.SaveEntitiesAsync();
            return userDTO;
        }

        public async Task<UserDTO> DeleteUserAsync(Guid customerId, Guid userId, Guid callerId, bool softDelete = true)
        {
            var user = await _organizationRepository.GetUserAsync(userId);

            if (!softDelete)
                throw new UserDeletedException();

            if (user == null || user.IsDeleted) return null;

            user.UnAssignAllDepartments(customerId);

            user.SetDeleteStatus(true, callerId);

            await _organizationRepository.SaveEntitiesAsync();

            if (!string.IsNullOrEmpty(user.OktaUserId))
            {
                await _oktaServices.RemoveUserFromGroupAsync(user.OktaUserId);
            }

            await PublishEvent(customerId, userId, user.Department?.ExternalDepartmentId);

            //Get the users role and assign it to the users DTO
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = await GetRoleNameForUser(user.Email);
            return userDTO;
        }

        private async Task PublishEvent(Guid customerId, Guid userId, Guid? departmentId)
        {
            // Publish event
            try
            {
                var source = new CancellationTokenSource();
                var cancellationToken = source.Token;
                using var client = new DaprClientBuilder().Build();
                await client.PublishEventAsync("customer-pub-sub", "user-deleted",
                    new { CustomerId = customerId, UserId = userId, DepartmentId = departmentId, CreatedDate = DateTime.UtcNow }, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError("Unable to publish event for deleted user. {exception.Message}", exception);
            }
        }

        public async Task<UserDTO> AssignDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _organizationRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null || department == null)
                return null;
            user.AssignDepartment(department, callerId);

            //Get the users role and deparrtment name and assign it to the users DTO
            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = await GetRoleNameForUser(user.Email);
            userDTO.DepartmentName = department.Name;

            await _organizationRepository.SaveEntitiesAsync();
            return userDTO;
        }

        public async Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            var customer = await _organizationRepository.GetOrganizationAsync(customerId, includeDepartments: true);
            if (customer == null) throw new CustomerNotFoundException($"Unable to find {customerId}");

            var user = await GetUserAsync(customerId, userId);
            if (user == null) throw new UserNotFoundException($"Unable to find {userId}");

            //Check if the customer has the department
            var departments = await _organizationRepository.GetDepartmentsAsync(customerId);
            var department = departments.FirstOrDefault(d => d.ExternalDepartmentId == departmentId);

            if (department == null) throw new DepartmentNotFoundException($"Unable to find {departmentId}");

            //check if the user already is a manager for this department
            var manager = department.Managers.FirstOrDefault(a => a.UserId == userId);
            if (manager != null) return;

            //Find subdepartments of the department the user user is to be manager for
            var subDepartmentsList = department.Subdepartments(departments);
            List<Guid> accessList = subDepartmentsList.Select(a => a.ExternalDepartmentId).ToList();

            //Check if user have department role for other departments
            var usersPermission = await _userPermissionServices.GetUserPermissionsAsync(user.Email);
            UserPermissions managerPermission = null;
            if (usersPermission != null) managerPermission = usersPermission.FirstOrDefault(a => a.Role.Name == PredefinedRole.DepartmentManager.ToString() || a.Role.Name == PredefinedRole.Manager.ToString());

            //User needs to have the role as department manager before getting assigned as a manager for a department
            if (managerPermission == null) throw new MissingRolePermissionsException();

            //Check if user has department manager role for given customer
            if(!managerPermission.AccessList.Contains(customerId)) throw new MissingRolePermissionsException();

            //add user as manager for the department
            user.AssignManagerToDepartment(department, callerId);
            await _organizationRepository.SaveEntitiesAsync();

            foreach (var access in accessList)
            {
                    if (!managerPermission.AccessList.Contains(access))
                    {
                        managerPermission.AddAccess(access, callerId);
                    }
            }

            await _userPermissionServices.UpdatePermission(managerPermission);
        }

        public async Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            var customer = await _organizationRepository.GetOrganizationAsync(customerId, includeDepartments: true);
            if (customer == null) throw new CustomerNotFoundException($"Unable to find {customerId}");

            var user = await GetUserAsync(customerId, userId);
            if (user == null) throw new UserNotFoundException($"Unable to find {userId}");

            //Check if the customer has the department
            var departments = await _organizationRepository.GetDepartmentsAsync(customerId);
            var department = departments.FirstOrDefault(d => d.ExternalDepartmentId == departmentId);

            if (department == null) throw new DepartmentNotFoundException($"Unable to find {departmentId}");

            user.UnassignManagerFromDepartment(department, callerId);
            await _organizationRepository.SaveEntitiesAsync();

            //Find subdepartments of the department 
            var subDepartmentsList = department.Subdepartments(departments);
            List<Guid> accessList = subDepartmentsList.Select(a => a.ExternalDepartmentId).ToList();

            //Check if user have permissions for department
            var usersPermission = await _userPermissionServices.GetUserPermissionsAsync(user.Email);
            if (usersPermission == null) return;

            var managerPermission = usersPermission.FirstOrDefault(a => a.Role.Name == PredefinedRole.DepartmentManager.ToString() || a.Role.Name == PredefinedRole.Manager.ToString());

            //User needs to have the role as department manager before getting assigned as a manager for a department
            if (managerPermission == null) throw new MissingRolePermissionsException();

            //Check if user has department manager role for given customer
            if (!managerPermission.AccessList.Contains(customerId)) throw new MissingRolePermissionsException();

            foreach (var access in accessList)
            {
                managerPermission.RemoveAccess(access, callerId);
            }
            await _userPermissionServices.UpdatePermission(managerPermission);

            //add EndUser role if this is the only department 
            if (managerPermission.AccessList.Count > 0 && managerPermission.AccessList.All(a => a == customerId))
            {
                var endUserPermission = await _userPermissionServices.AssignUserPermissionsAsync(user.Email, PredefinedRole.EndUser.ToString(), new List<Guid> { customerId }, callerId);

                if (endUserPermission == null)
                {
                    throw new MissingRolePermissionsException();
                }
            }

        }

        public async Task<UserDTO> UnassignDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            var department = await _organizationRepository.GetDepartmentAsync(customerId, departmentId);
            if (user == null || department == null)
                return null;
            user.UnassignDepartment(department, callerId);

            //Get the users role and assign it to the users DTO
            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = await GetRoleNameForUser(user.Email);

            await _organizationRepository.SaveEntitiesAsync();
            return userDTO;
        }

        public async Task<UserInfo> GetUserInfoFromUserName(string userName)
        {
            return _mapper.Map<UserInfo>(await _organizationRepository.GetUserByUserName(userName));
        }
        public async Task<UserInfo> GetUserInfoFromUserId(Guid userId)
        {
            return _mapper.Map<UserInfo>(await _organizationRepository.GetUserAsync(userId));
        }
    }
}