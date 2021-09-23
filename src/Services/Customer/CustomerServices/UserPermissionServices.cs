using Common.Enums;
using Common.Extensions;
using Common.Logging;
using Common.Utilities;
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

        public async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            int numberOfRecordsSaved = 0;
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

        public async Task<IEnumerable<UserPermissions>> GetUserPermissionsAsync(string userName)
        {
            return await _customerContext.UserPermissions
                .Include(up => up.Role).ThenInclude(r => r.GrantedPermissions).ThenInclude(p => p.Permissions)
                .Include(up => up.User)
                .Where(up => up.User.Email == userName).ToListAsync();
        }

        public async Task<Role> GetRole(PredefinedRole predefinedRole)
        {
            var role = await _customerContext.Roles
                .Include(r => r.GrantedPermissions)
                .ThenInclude(g => g.Permissions)
                .FirstOrDefaultAsync(r => r.Id == (int)predefinedRole);
            return role;
        }

        public async Task<UserPermissions> AssignUserPermissionsAsync(string userName, PredefinedRole predefinedRole, IList<Guid> accessList)
        {
            var user = await _customerContext.Users.FirstOrDefaultAsync(u => u.Email == userName);
            if (user == null)
                return null;
            var role = await GetRole(predefinedRole);
            if (predefinedRole == PredefinedRole.DepartmentManager && !accessList.Any())
            {
                return null;
            }
            var userPermission = new UserPermissions(user, role, accessList);
            _customerContext.UserPermissions.Add(userPermission);
            await SaveEntitiesAsync();
            return userPermission;
        }

        public async Task<UserPermissions> RemoveUserPermissionsAsync(string userName, PredefinedRole predefinedRole, IList<Guid> accessList)
        {
            var user = await _customerContext.Users.FirstOrDefaultAsync(u => u.Email == userName);
            if (user == null)
                return null;
            var userPermissions = await GetUserPermissionsAsync(userName);
            var userPermission = userPermissions.FirstOrDefault(p => p.Role.Id == (int)predefinedRole);
            if (userPermission != null)
            {
                if (Enum.TryParse(typeof(PredefinedRole), userPermission.Role.Name, out object result) &&
                    (PredefinedRole)result == PredefinedRole.DepartmentManager &&
                    accessList.Any())
                {

                }
                else
                {
                    _customerContext.UserPermissions.Remove(userPermission);
                }
                await SaveEntitiesAsync();
            }
            return userPermission;
        }
    }
}