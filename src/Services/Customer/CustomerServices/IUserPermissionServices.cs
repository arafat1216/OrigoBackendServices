using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IUserPermissionServices
    {
        Task<IList<UserPermissions>> GetUserPermissionsAsync(string userName);
        Task<UserPermissions> AssignUserPermissionsAsync(string userName, string roleName, IList<Guid> accessList, Guid callerId);
        Task<UserPermissions> RemoveUserPermissionsAsync(string userName, string predefinedRole, IList<Guid> accessList, Guid callerId);
        Task<IList<string>> GetAllRolesAsync();
        Task<IList<UserPermissions>> GetUserAdminsAsync();
    }
}