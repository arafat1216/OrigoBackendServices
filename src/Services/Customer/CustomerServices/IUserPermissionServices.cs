using Common.Enums;
using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IUserPermissionServices
    {
        Task<IList<UserPermissions>> GetUserPermissionsAsync(string userName);
        Task<UserPermissions> AssignUserPermissionsAsync(string userName, string roleName, IList<Guid> accessList);
        Task<UserPermissions> RemoveUserPermissionsAsync(string userName, string predefinedRole, IList<Guid> accessList);
        Task<IList<string>> GetAllRolesAsync();
    }
}