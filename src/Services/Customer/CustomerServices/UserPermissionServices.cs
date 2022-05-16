using AutoMapper;
using Common.Enums;
using Common.Extensions;
using Common.Logging;
using Common.Utilities;
using CustomerServices.Exceptions;
using CustomerServices.Infrastructure;
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

        public UserPermissionServices(CustomerContext customerContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator, IMapper mapper)
        {
            _customerContext = customerContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
            _mapper = mapper;

        }

        private async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var numberOfRecordsSaved = 0;
            // Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            // See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_customerContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                foreach (var @event in _customerContext.GetDomainEventsAsync())
                {
                    if (!_customerContext.IsSQLite) 
                    {
                        await _functionalEventLogService.SaveEventAsync(@event, _customerContext.Database.CurrentTransaction);
                    }
                }
                await _customerContext.SaveChangesAsync(cancellationToken);
                numberOfRecordsSaved = await _customerContext.SaveChangesAsync(cancellationToken);
                await _mediator.DispatchDomainEventsAsync(_customerContext);
            });
            return numberOfRecordsSaved;
        }

        public async Task<IList<UserPermissions>> GetUserPermissionsAsync(string userName)
        {
            return await _customerContext.UserPermissions
                .Include(up => up.Role).ThenInclude(r => r.GrantedPermissions).ThenInclude(p => p.Permissions)
                .Include(up => up.User)
                .Where(up => up.User.Email == userName).ToListAsync();
        }
        public async Task<IList<UserPermissions>> GetUserAdminsAsync()
        {
            return await _customerContext.UserPermissions
                .Include(up => up.Role).ThenInclude(r => r.GrantedPermissions).ThenInclude(p => p.Permissions)
                .Include(up => up.User)
                .Where(up => up.Role.Name == PredefinedRole.SystemAdmin.ToString() ||
                        up.Role.Name == PredefinedRole.PartnerAdmin.ToString() ||
                        up.Role.Name == PredefinedRole.PartnerReadOnlyAdmin.ToString()).ToListAsync();
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
        public async Task<UserPermissions> AssignUserPermissionsAsync(string userName, string roleName, IList<Guid> accessList, Guid callerId)
        {
            if (!Enum.TryParse(roleName, out PredefinedRole roleType))
            {
                throw new InvalidRoleNameException();
            }
            var user = await _customerContext.Users.FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == userName.Trim().ToLower());

            if (user == null)
                throw new UserNameDoesNotExistException();

            var userPermissions = await GetUserPermissionsAsync(userName);
            var userPermission = userPermissions.FirstOrDefault(p => p.Role.Id == (int)roleType);

            if (roleType == PredefinedRole.DepartmentManager && accessList.Count == 0)// Check if the lists contains at least one id.
            {
                return null;
            }

            var addNew = false; // Check if we need to add this permission or update it.
            if (userPermission == null)
            {
                addNew = true;
                var role = await GetRole(roleType);
                userPermission = new UserPermissions(user, role, accessList, callerId);
            }
            if (addNew)
            {
                _customerContext.UserPermissions.Add(userPermission);
            }
            else if (accessList.Any())
            {
                foreach (var access in accessList)
                {
                    if (!userPermission.AccessList.Contains(access))
                    {
                        userPermission.AddAccess(access,callerId);
                        _customerContext.Entry(userPermission).State = EntityState.Modified;
                    }
                }
            }
            await SaveEntitiesAsync();
            return userPermission;
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

            var userPermissions = await GetUserPermissionsAsync(userName);
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

                    if (roleType == PredefinedRole.DepartmentManager && !userPermission.AccessList.Any())
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

        public async Task<UsersPermissionsDTO> AssignUsersPermissionsAsync(NewUsersPermission newUserPermissions, Guid callerId)
        {
            //List of error messages with cause of failure for user(s)
            List<string> errorMessages = new List<string>();

            UsersPermissionsDTO usersPermissions = new UsersPermissionsDTO
            {
                UserPermissions = new List<NewUserPermissionDTO>()
            };

            foreach (var userPermission in newUserPermissions.UserPermissions)
            {
                try
                {
                    var user = await _customerContext.Users.FirstOrDefaultAsync(u => u.UserId == userPermission.UserId);
                    if (user == null) throw new UserNotFoundException($"Could not find user with id {userPermission.UserId}");
                    var assigne = await AssignUserPermissionsAsync(user.Email, userPermission.Role, userPermission.AccessList, callerId);
                    var dto = _mapper.Map<NewUserPermissionDTO>(assigne);
                    usersPermissions.UserPermissions.Add(dto);

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

            usersPermissions.ErrorMessages = errorMessages;

            return usersPermissions;
        }
    }
}