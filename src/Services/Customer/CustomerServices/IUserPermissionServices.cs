#nullable enable
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices;

public interface IUserPermissionServices
{
    Task<IList<UserPermissionsDTO>?> GetUserPermissionsAsync(string userName);
    Task<UserPermissionsDTO?> AssignUserPermissionsAsync(string userName, string roleName, IList<Guid> accessList,
        Guid callerId);
    Task<UsersPermissionsAddedDTO> AssignUsersPermissionsAsync(NewUsersPermission newUserPermission, Guid callerId);

    Task<UserPermissions> RemoveUserPermissionsAsync(string userName, string predefinedRole, IList<Guid> accessList, Guid callerId);
    Task<IList<string>> GetAllRolesAsync();
    Task<IList<UserPermissions>> GetUserAdminsAsync(Guid? partnerId = null);
    Task UpdatePermission(UserPermissions userPermission);
    Task<IList<UserPermissions>> GetCustomerAdminsAsync(Guid customerId);
    Task UpdateAccessListAsync(User user, List<Guid> accessList, Guid callerId, bool removeMissing = false);
}