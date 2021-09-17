using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IUserPermissionServices
    {
        Task<IEnumerable<UserPermissions>> GetUserPermissionsAsync(string userName);
        Task<UserPermissions> AssignUserPermissions(Guid userId, Role role, IList<Guid> accessList);
        Task<UserPermissions> RemoveUserPermissions(Guid userId, Role role, IList<Guid> accessList);
    }
}