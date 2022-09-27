#nullable enable
using AutoMapper;
using Common.Enums;
using Common.Extensions;
using Common.Logging;
using Common.Utilities;
using CustomerServices.Exceptions;
using CustomerServices.Infrastructure.Context;
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class UserPermissionServices : IUserPermissionServices
    {
        private readonly CustomerContext _customerContext;
        private readonly IFunctionalEventLogService _functionalEventLogService;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IOrganizationServices _organizationServices;

        public UserPermissionServices(CustomerContext customerContext,
            IFunctionalEventLogService functionalEventLogService, IMediator mediator, IMapper mapper, IOrganizationServices organizationServices)
        {
            _customerContext = customerContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
            _mapper = mapper;
            _organizationServices = organizationServices;
        }

        /// <summary>
        /// For testing
        /// </summary>
        public UserPermissionServices()
        {

        }

        private async Task SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            // See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_customerContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                foreach (var @event in _customerContext.GetDomainEventsAsync())
                {
                    if (!_customerContext.IsSQLite && _customerContext.Database.CurrentTransaction != null)
                    {
                        await _functionalEventLogService.SaveEventAsync(@event,
                            _customerContext.Database.CurrentTransaction);
                    }
                }

                await _customerContext.SaveChangesAsync(cancellationToken);
                await _mediator.DispatchDomainEventsAsync(_customerContext);
            });
        }

        private async Task<IList<UserPermissions>> GetUserPermissionEntitiesAsync(string userName)
        {
            return await _customerContext.UserPermissions.Include(up => up.Role)
                .ThenInclude(r => r.GrantedPermissions).ThenInclude(p => p.Permissions).Include(up => up.User)
                .Where(up => up.User.Email == userName).ToListAsync();
        }

        public async Task<IList<UserPermissionsDTO>?> GetUserPermissionsAsync(string userName)
        {
            var userPermissions = await _customerContext.UserPermissions
                                                            .Include(up => up.Role)
                                                            .ThenInclude(r => r.GrantedPermissions)
                                                            .ThenInclude(p => p.Permissions)
                                                            .Include(up => up.User)
                                                            .Where(up => up.User.Email == userName)
                                                            .ToListAsync();
            var returnedUserPermissions = new List<UserPermissionsDTO>();
            foreach (var userPermission in userPermissions)
            {
                if (userPermission.User.UserStatus == UserStatus.Invited)
                    await InitiateOnboarding(userPermission.User);

                var permissionNames = new List<string>();
                foreach (var roleGrantedPermission in userPermission.Role.GrantedPermissions)
                {
                    permissionNames.AddRange(roleGrantedPermission.Permissions.Select(p => p.Name));
                }

                if (userPermission.Role.Name == "PartnerAdmin" && userPermission.AccessList.Count > 0)
                {
                    var partnerId = userPermission.AccessList.FirstOrDefault();
                    var customerIdsForPartner = await _organizationServices.GetOrganizationIdsForPartnerAsync(partnerId);
                    if (customerIdsForPartner != null)
                    {
                        foreach (var customerId in customerIdsForPartner)
                        {
                            userPermission.AccessList.Add(customerId);
                        }
                    }
                }

                returnedUserPermissions.Add(new UserPermissionsDTO(permissionNames, userPermission.AccessList.ToList(),
                    userPermission.Role.Name, userPermission.User.UserId));
            }

            return returnedUserPermissions;
        }

        public async Task<IList<UserPermissions>> GetUserAdminsAsync(Guid? partnerId = null)
        {
            if (partnerId == null)
            {
                return await _customerContext.UserPermissions.Include(up => up.Role).ThenInclude(r => r.GrantedPermissions)
                    .ThenInclude(p => p.Permissions).Include(up => up.User).Where(up =>
                        up.Role.Name == PredefinedRole.SystemAdmin.ToString() ||
                        up.Role.Name == PredefinedRole.PartnerAdmin.ToString() ||
                        up.Role.Name == PredefinedRole.PartnerReadOnlyAdmin.ToString()).ToListAsync();
            }

            var userPermissions = await _customerContext.UserPermissions.Include(up => up.Role).ThenInclude(r => r.GrantedPermissions)
                .ThenInclude(p => p.Permissions).Include(up => up.User).Where(up =>
                    up.Role.Name == PredefinedRole.PartnerAdmin.ToString() ||
                    up.Role.Name == PredefinedRole.PartnerReadOnlyAdmin.ToString()).ToListAsync();
            return userPermissions.Where(up => up.AccessList.Contains(partnerId.Value)).ToList();

        }

        public async Task<IList<UserPermissions>> GetCustomerAdminsAsync(Guid customerId)
        {
            var userPermissions = await _customerContext.UserPermissions.Include(up => up.Role).ThenInclude(r => r.GrantedPermissions)
                .ThenInclude(p => p.Permissions).Include(up => up.User).Where(up =>
                    up.Role.Name == PredefinedRole.CustomerAdmin.ToString())
                .ToListAsync();
            return userPermissions.Where(x => x.AccessList.Contains(customerId)).ToList();

        }

        public async Task UpdateAccessListAsync(User user, List<Guid> accessList, Guid callerId, bool removeMissing = false)
        {
            var userPermissions = await GetUserPermissionEntitiesAsync(user.Email);
            if (userPermissions.Any() && accessList.Any())
            {
                var userPermission = userPermissions.First();
                foreach (var access in accessList.Where(access => !userPermission.AccessList.Contains(access)))
                {
                    userPermission.AddAccess(access, callerId);
                    _customerContext.Entry(userPermission).State = EntityState.Modified;
                }

                if (removeMissing)
                {
                    foreach (var existingPermissionAccess in userPermission.AccessList.ToList().Where(existingPermissionAccess => !accessList.Contains(existingPermissionAccess)))
                    {
                        userPermission.RemoveAccess(existingPermissionAccess, callerId);
                        _customerContext.Entry(userPermission).State = EntityState.Modified;
                    }
                }
            }
            await SaveEntitiesAsync();
        }


        private async Task<Role> GetRole(PredefinedRole predefinedRole)
        {
            var role = await _customerContext.Roles
                .Include(r => r.GrantedPermissions)
                .ThenInclude(g => g.Permissions)
                .FirstOrDefaultAsync(r => r.Id == (int)predefinedRole);
            return role;
        }

        // TODO: Add full and proper docs.
        /// <exception cref="InvalidRoleNameException"></exception>
        /// <exception cref="UserNameDoesNotExistException"></exception>
        public async Task<UserPermissionsDTO?> AssignUserPermissionsAsync(string userName, string roleName,
            IList<Guid> accessList, Guid callerId)
        {
            if (!Enum.TryParse(roleName, out PredefinedRole roleType))
            {
                throw new InvalidRoleNameException();
            }
            var user = await _customerContext.Users.FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == userName.Trim().ToLower());
            if (user == null)
                throw new UserNameDoesNotExistException();

            var userPermissions = await GetUserPermissionEntitiesAsync(userName);

            if (roleType is PredefinedRole.DepartmentManager or PredefinedRole.Manager &&
                accessList.Count == 0) // Check if the lists contains at least one id.
            {
                return null;
            }

            if (roleType is PredefinedRole.PartnerAdmin &&
                accessList.Count == 0) // Check if the lists contains at least one id.
            {
                return null;
            }


            var userPermission = userPermissions.FirstOrDefault(p => p.Role.Name == roleType.ToString());
            //Update permission and remove if the user have more then one permission - users should only have one userPermission
            if (userPermission != null)
            {
                var role = await GetRole(roleType);
                if (role.Name != userPermission.Role.Name)
                {
                    userPermission.UpdateRole(role, callerId);
                }

                var remove = userPermissions.Where(a => a.Id != userPermission.Id).ToList();

                if (remove.Any())
                {
                    foreach (var permission in remove)
                    {
                        _customerContext.Entry(permission).State = EntityState.Deleted;
                    }
                }
            }

            var addNew = false; // Check if we need to add this permission or update it.
            if (userPermission == null)
            {
                if (userPermissions.Any())
                {
                    foreach (var permissions in userPermissions)
                    {
                        _customerContext.Entry(permissions).State = EntityState.Deleted;

                    }
                }

                addNew = true;
                var role = await GetRole(roleType);
                userPermission = new UserPermissions(user, role, accessList, callerId);
            }

            if (addNew)
            {
                _customerContext.UserPermissions.Add(userPermission);
            }
            else
            {
                if (accessList.Any())
                {
                    foreach (var access in accessList)
                    {
                        if (userPermission.AccessList.Contains(access)) continue;
                        userPermission.AddAccess(access, callerId);
                        _customerContext.Entry(userPermission).State = EntityState.Modified;
                    }
                }
            }

            await SaveEntitiesAsync();
            return _mapper.Map<UserPermissionsDTO>(userPermission);
        }

        // TODO: Add full and proper docs.
        /// <exception cref="InvalidRoleNameException"></exception>
        /// <exception cref="UserNameDoesNotExistException"></exception>
        public async Task<UserPermissions> RemoveUserPermissionsAsync(string userName, string roleName, IList<Guid> accessList, Guid callerId)
        {
            if (!Enum.TryParse(roleName, out PredefinedRole roleType))
            {
                throw new InvalidRoleNameException();
            }

            var user = await _customerContext.Users.FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == userName.Trim().ToLower());
            
            if (user == null)
                throw new UserNameDoesNotExistException();

            var userPermissions = await GetUserPermissionEntitiesAsync(userName);
            var userPermission = userPermissions.FirstOrDefault(p => p.Role.Id == (int)roleType);
            if (userPermission != null)
            {
                if (accessList.Any())
                {
                    foreach (Guid access in accessList)
                    {
                        userPermission.RemoveAccess(access,callerId);
                        _customerContext.Entry(userPermission).State = EntityState.Modified;
                    }

                    if ((roleType == PredefinedRole.DepartmentManager || roleType == PredefinedRole.Manager) && !userPermission.AccessList.Any())
                    {
                        userPermission.RemoveRole(callerId);
                        _customerContext.UserPermissions.Remove(userPermission);
                    }
                }
                else
                {
                    userPermission.RemoveRole(callerId);
                    _customerContext.UserPermissions.Remove(userPermission);
                }
                await SaveEntitiesAsync();
            }
            return userPermission;
        }

        public async Task<IList<string>> GetAllRolesAsync()
        {
            return await _customerContext.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task UpdatePermission(UserPermissions userPermission)
        {
            _customerContext.Entry(userPermission).State = EntityState.Modified;
            await SaveEntitiesAsync();
        }

        public async Task<UsersPermissionsAddedDTO> AssignUsersPermissionsAsync(NewUsersPermission newUserPermission, Guid callerId)
        {
            //List of error messages with cause of failure for user(s)
            List<string> errorMessages = new List<string>();

            UsersPermissionsAddedDTO usersPermissionsAdded = new UsersPermissionsAddedDTO
            {
                UserPermissions = new List<NewUserPermissionDTO>()
            };

            //Check if the request have unique userId - a user can only have one role
            List<Guid> users = newUserPermission.UserPermissions.Select(person => person.UserId).Distinct().ToList();
            if (users.Count != newUserPermission.UserPermissions.Count) 
            {
                throw new DuplicateException("Only one permission can be added to the user");
            }
            

            foreach (var userPermission in newUserPermission.UserPermissions)
            {
                try
                {
                    var user = await _customerContext.Users.FirstOrDefaultAsync(u => u.UserId == userPermission.UserId);
                    if (user == null) throw new UserNotFoundException($"Could not find user with id {userPermission.UserId}");
                    var assignedUser = await AssignUserPermissionsAsync(user.Email, userPermission.Role, userPermission.AccessList, callerId);
                    var dto = _mapper.Map<NewUserPermissionDTO>(assignedUser);
                    usersPermissionsAdded.UserPermissions.Add(dto);

                }
                catch (InvalidRoleNameException)
                {
                    errorMessages.Add($"Invalid role name {userPermission.Role} for {userPermission.UserId}");
                }
                catch (UserNotFoundException)
                {
                    errorMessages.Add($"User with id {userPermission.UserId} does not exist");
                }
                catch (UserNameDoesNotExistException)
                {
                    errorMessages.Add($"Invalid role {userPermission.Role} name for {userPermission.UserId}");
                }
            }

            usersPermissionsAdded.ErrorMessages = errorMessages;

            return usersPermissionsAdded;
        }
        /// <summary>
        /// Handles change in user status for the user. Should get OnboardInitiated if the user have the status Invited.
        /// </summary>
        /// <param name="user">User object to get OnboardInitiated user status.</param>
        private async Task InitiateOnboarding(User user)
        {
            user.OnboardingInitiated();
            await SaveEntitiesAsync();

        }
    }
}