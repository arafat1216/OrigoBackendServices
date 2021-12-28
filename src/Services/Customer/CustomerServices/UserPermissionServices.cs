using Common.Enums;
using Common.Extensions;
using Common.Logging;
using Common.Utilities;
using CustomerServices.Exceptions;
using CustomerServices.Infrastructure;
using CustomerServices.Models;
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

        public UserPermissionServices(CustomerContext customerContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator)
        {
            _customerContext = customerContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
        }

        private async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_customerContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                foreach (var @event in _customerContext.GetDomainEventsAsync())
                {
                    await _functionalEventLogService.SaveEventAsync(@event, _customerContext.Database.CurrentTransaction);
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

        private async Task<Role> GetRole(PredefinedRole predefinedRole)
        {
            var role = await _customerContext.Roles
                .Include(r => r.GrantedPermissions)
                .ThenInclude(g => g.Permissions)
                .FirstOrDefaultAsync(r => r.Id == (int)predefinedRole);
            return role;
        }

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
            var departments = await _customerContext.Departments.Where(d => d.Customer == user.Customer).ToListAsync();
            if (roleType == PredefinedRole.DepartmentManager && !accessList.Any()) // Can't be department manager without access to a department.
            {
                return null;
            }
            if (roleType == PredefinedRole.DepartmentManager &&
                     (departments.Any(d => accessList.Contains(d.ExternalDepartmentId)) || 
                      (userPermission != null && departments.Any(d => userPermission.AccessList.Contains(d.ExternalDepartmentId))))) // Check if the lists contains at least one department id.
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
    }
}