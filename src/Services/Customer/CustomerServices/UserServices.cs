﻿#nullable enable
using AutoMapper;
using Common.Enums;
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
using Common.Model.EventModels;
using Dapr.Client;
using CustomerServices.Email;

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
        private readonly IEmailService _emailService;

        public UserServices(ILogger<UserServices> logger, IOrganizationRepository customerRepository,
            IOktaServices oktaServices, IMapper mapper, IUserPermissionServices userPermissionServices, 
            IEmailService emailService)
        {
            _logger = logger;
            _organizationRepository = customerRepository;
            _oktaServices = oktaServices;
            _mapper = mapper;
            _userPermissionServices = userPermissionServices;
            _emailService = emailService;
        }

        public async Task<OrganizationUserCount?> GetUsersCountAsync(Guid customerId, Guid[]? assignedToDepartment, string[]? role)
        {
            return await _organizationRepository.GetOrganizationUsersCountAsync(customerId, assignedToDepartment, role);
        }

        public async Task<PagedModel<UserDTO>> GetAllUsersAsync(Guid customerId, string[]? role, Guid[]? assignedToDepartment, IList<int>? userStatus, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 100)
        {
            return await _organizationRepository.GetAllUsersAsync(customerId, role, assignedToDepartment, userStatus, cancellationToken, search, page, limit);
        }

        private async Task<string> GetRoleNameForUser(string userEmail)
        {
            var userPermissions = await _userPermissionServices.GetUserPermissionsAsync(userEmail);
            if (userPermissions == null || !userPermissions.Any())
            {
                return string.Empty;
            }

            return userPermissions.FirstOrDefault() == null ? string.Empty : userPermissions.FirstOrDefault()!.Role.Name;
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
            var customerPreferences = await _organizationRepository.GetOrganizationPreferencesAsync(customerId);
            if (customer == null) throw new CustomerNotFoundException();
            if (userPreference == null || userPreference.Language == null)
            {
                // Set a default language setting - try to add organizations primary language 
                if (customerPreferences != null) userPreference = new UserPreference(customerPreferences.PrimaryLanguage ?? "EN", callerId);
                else userPreference = new UserPreference("EN", callerId);
            }

            // Check if email address is used by another user
            var userWithEmail = await _organizationRepository.GetUserByUserName(email);
            var mobileNumberInUse = await _organizationRepository.GetUserByMobileNumber(mobileNumber);

            //Activate the user if the user i soft deleted
            if (userWithEmail != null) {
                if (userWithEmail.IsDeleted == true) 
                {
                    userWithEmail.SetDeleteStatus(false, callerId);

                    //Check if the phone number should be updated
                    if (mobileNumber != default && userWithEmail.MobileNumber?.Trim() != mobileNumber?.Trim())
                    { 
                    //If mobile number is used by someone else then this user
                    if(mobileNumberInUse != null && mobileNumberInUse.UserId != userWithEmail.UserId) throw new InvalidPhoneNumberException("Phone number already in use.");
                        userWithEmail.ChangeMobileNumber(mobileNumber, callerId);
                    }
                    
                    UserPermissions? currentUserPermission = null;
                    var usersRole = await _userPermissionServices.GetUserPermissionsAsync(userWithEmail.Email);
                    if (usersRole != null)
                    {
                        currentUserPermission = usersRole.FirstOrDefault(a => a.Role.Name == role);
                    }

                    if (role != null)
                    {
                        try
                        {
                          //New role - only add the role if the user dont have the role already
                          if (currentUserPermission == null) currentUserPermission = await _userPermissionServices.AssignUserPermissionsAsync(email, role, new List<Guid>() { customerId }, callerId);
                        }
                        catch (InvalidRoleNameException)
                        {
                            currentUserPermission = await _userPermissionServices.AssignUserPermissionsAsync(email, PredefinedRole.EndUser.ToString(), new List<Guid>() { customerId }, callerId);
                        }
                    }

                    //If the user dont have any userpermissions and the role did not get assigned in the request give EndUser
                    if(currentUserPermission == null) currentUserPermission = await _userPermissionServices.AssignUserPermissionsAsync(email, PredefinedRole.EndUser.ToString(), new List<Guid>() { customerId }, callerId);

                    if (firstName != default && userWithEmail.FirstName != firstName) userWithEmail.ChangeFirstName(firstName, callerId);
                    if (lastName != default && userWithEmail.LastName != lastName) userWithEmail.ChangeLastName(lastName, callerId);
                    if (employeeId != default && userWithEmail.EmployeeId != employeeId) userWithEmail.ChangeEmployeeId(employeeId, callerId);
                    if (userPreference != null && userPreference.Language != null &&
                        userPreference.Language != userWithEmail.UserPreference?.Language)
                        userWithEmail.ChangeUserPreferences(userPreference, callerId);

                    //Okta
                    if (customer.AddUsersToOkta)
                    {
                        var userExistInOkta = await _oktaServices.UserExistsInOktaAsync(userWithEmail.OktaUserId);
                        if (userExistInOkta)
                        {
                            await _oktaServices.AddUserToGroup(userWithEmail.OktaUserId);
                        }
                        else //Add new user to Okta
                        {
                            var oktaUserId = await _oktaServices.AddOktaUserAsync(userWithEmail.UserId, userWithEmail.FirstName, userWithEmail.LastName,
                                userWithEmail.Email, userWithEmail.MobileNumber, true);
                            userWithEmail = await AssignOktaUserIdAsync(userWithEmail.Customer.OrganizationId, userWithEmail.UserId, oktaUserId, callerId);
                        }
                    }

                    await _organizationRepository.SaveEntitiesAsync();

                    var activatedUserMapped = _mapper.Map<UserDTO>(userWithEmail);
                    if (currentUserPermission != null)
                    {
                        activatedUserMapped.Role = currentUserPermission.Role.Name.ToString();
                    }
                    else
                    {
                        activatedUserMapped.Role = null;
                    }

                    return activatedUserMapped;

                }
                else throw new UserNameIsInUseException("Email address is already in use.");
            }

            //Check if mobile number is used by another user
            if (mobileNumberInUse != null) throw new InvalidPhoneNumberException("Phone number already in use.");

            var newUser = new User(customer, Guid.NewGuid(), firstName, lastName, email, mobileNumber, employeeId,
                userPreference, callerId);

            //Only send invitation mail if the customer has started their onbaording 
            if (customer.CustomerStatus == CustomerStatus.StartedOnboarding)
            {
                await _emailService.InvitationEmailToUserAsync(new Email.Models.InvitationMail
                {
                    FirstName = firstName,
                    Recipient = new List<string> { email }
                },userPreference.Language ?? "EN");

                newUser.ChangeUserStatus(null, UserStatus.Invited);
            }

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
            user.ChangeUserStatus(oktaUserId,UserStatus.Activated);
            await _organizationRepository.SaveEntitiesAsync();
            return user;
        }

        public async Task<UserDTO> SetUserActiveStatusAsync(Guid customerId, Guid userId, bool isActive, Guid callerId)
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

            if (string.IsNullOrEmpty(user.OktaUserId))
            {
                if (isActive)
                {
                    var oktaUserId = await _oktaServices.AddOktaUserAsync(user.UserId, user.FirstName, user.LastName,
                        user.Email, user.MobileNumber, true);
                    user.ActivateUser(oktaUserId);
                }
                else
                {
                    //Ensure that user with no okta id is set to deactivated
                    if (user.UserStatus == UserStatus.Activated) 
                        user.DeactivateUser();
                    throw new UserNotFoundException("User does not have Okta id and can not be activated.");
                }
            }
            else
            {
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
                        await _oktaServices.RemoveUserFromGroupAsync(user.OktaUserId);
                        user.DeactivateUser();
                    }
                }
                else if (isActive)
                {
                    // User does not exist in Okta, but need to activate the User. Therefore re-creating the user in Okta and updating the User status.
                    // There could be some users whose "OktaUserId" in DB is not null but at the same time do not exist in Okta. They will fall under this condition.
                    var oktaUserId = await _oktaServices.AddOktaUserAsync(user.UserId, user.FirstName, user.LastName,
                        user.Email, user.MobileNumber, true);
                    user.ActivateUser(oktaUserId);
                }
                else
                {
                    // User does not exist in Okta, therefore can not be deactivated.
                    throw new UserNotFoundException("User does not exist in Okta.");
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

            var userDeletedEvent = new UserEvent
            {
                CustomerId = customerId,
                UserId = userId, 
                DepartmentId = user.Department?.ExternalDepartmentId,
                CreatedDate = DateTime.UtcNow
            };
            await PublishEvent("customer-pub-sub", "user-deleted", userDeletedEvent);

            //Get the users role and assign it to the users DTO
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = await GetRoleNameForUser(user.Email);
            return userDTO;
        }

        private async Task PublishEvent<T>(string subscriptionName, string topicName, T userEvent) where T: IUserEvent
        {
            // Publish event
            try
            {
                var source = new CancellationTokenSource();
                var cancellationToken = source.Token;
                using var client = new DaprClientBuilder().Build();
                await client.PublishEventAsync(subscriptionName, topicName, userEvent, cancellationToken);
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
            await PublishEvent("customer-pub-sub", "user-assign-department", new UserChangedDepartmentEvent{CustomerId = customerId, DepartmentId = departmentId, UserId = userId, CreatedDate = DateTime.UtcNow});
            return userDTO;
        }

        public async Task<UserDTO> InitiateOffboarding(Guid customerId, Guid userId, DateTime lastWorkingDay, bool buyoutAllowed, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null) throw new UserNotFoundException($"Unable to find {userId}");

            user.OffboardingInitiated(lastWorkingDay, callerId);
            await _organizationRepository.SaveEntitiesAsync();

            if (buyoutAllowed)
            {
                await _emailService.OffboardingInitiatedWithBuyoutEmailToUserAsync(new Email.Models.OffboardingInitiatedWithBuyout
                {
                    FirstName = $"{user.FirstName}",
                    LastBuyoutDay = user.LastDayForReportingSalaryDeduction!.Value.ToShortDateString(),
                    MyPageLink = "https://www.example.com/",
                    Recipient = new List<string>() { user.Email }
                }, string.IsNullOrEmpty(user.UserPreference!.Language) ? "EN" : user.UserPreference!.Language);
            }
            else
            {
                await _emailService.OffboardingInitiatedWithoutBuyoutEmailToUserAsync(new Email.Models.OffboardingInitiatedWithoutBuyout
                {
                    FirstName = $"{user.FirstName}",
                    MyPageLink = "https://www.example.com/",
                    Recipient = new List<string>() { user.Email }
                }, string.IsNullOrEmpty(user.UserPreference!.Language) ? "EN" : user.UserPreference!.Language);
            }

            var userDTO = _mapper.Map<UserDTO>(user);
            return userDTO;
        }

        public async Task<UserDTO> CancelOffboarding(Guid customerId, Guid userId, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null) throw new UserNotFoundException($"Unable to find {userId}");

            user.OffboardingCancelled(callerId);
            await _organizationRepository.SaveEntitiesAsync();

            var userDeletedEvent = new UserEvent
            {
                CustomerId = customerId,
                UserId = userId,
                DepartmentId = user.Department?.ExternalDepartmentId,
                CreatedDate = DateTime.UtcNow
            };
            await PublishEvent("customer-pub-sub", "offboard-cancelled", userDeletedEvent);

            var userDTO = _mapper.Map<UserDTO>(user);
            return userDTO;
        }

        public async Task<UserDTO> OverdueOffboarding(Guid customerId, Guid userId, Guid callerId)
        {
            var user = await GetUserAsync(customerId, userId);
            if (user == null) throw new UserNotFoundException($"Unable to find {userId}");
            if (user.UserStatus != UserStatus.OffboardInitiated) throw new UserNotFoundException($"Offboarding not initiated for User: {userId}");

            user.OffboardingOverdued(callerId);
            await _organizationRepository.SaveEntitiesAsync();

            var department = await _organizationRepository.GetDepartmentAsync(customerId, user.Department!.ExternalDepartmentId);
            if (department == null) throw new DepartmentNotFoundException($"Unable to find {user.Department!.ExternalDepartmentId}");

            if (department.Managers.Any())
            {
                await _emailService.OffboardingOverdueEmailToManagersAsync(new Email.Models.OffboardingOverdueMail
                {
                    UserName = $"{user.FirstName} {user.LastName}",
                    LastWorkingDays = user.LastWorkingDay!.Value.ToShortDateString(),
                    CustomerId = customerId,
                    UserId = userId,
                    Recipient = department.Managers.Select(x => x.Email).ToList()
                }, "EN");
            }          

            var userDTO = _mapper.Map<UserDTO>(user);
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

            //Find sub departments of the department the user user is to be manager for
            var subDepartmentsList = department.SubDepartments(departments);
            var accessList = subDepartmentsList.Select(a => a.ExternalDepartmentId).ToList();

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
            var subDepartmentsList = department.SubDepartments(departments);
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