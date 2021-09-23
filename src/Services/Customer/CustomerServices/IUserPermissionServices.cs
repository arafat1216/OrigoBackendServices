using Common.Enums;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IUserPermissionServices
    {
        Task<IEnumerable<UserPermissions>> GetUserPermissionsAsync(string userName);
        Task<UserPermissions> AssignUserPermissionsAsync(string userName, PredefinedRole predefinedRole, IList<Guid> accessList);
        Task<UserPermissions> RemoveUserPermissionsAsync(string userName, PredefinedRole predefinedRole, IList<Guid> accessList);
    }
}