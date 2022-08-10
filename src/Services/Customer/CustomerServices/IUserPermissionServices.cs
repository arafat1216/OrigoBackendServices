using CustomerServices.Models;
using CustomerServices.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IUserPermissionServices
    {
        Task<IList<UserPermissions>> GetUserPermissionsAsync(string userName);
        Task<UserPermissions> AssignUserPermissionsAsync(string userName, string roleName, IList<Guid> accessList, Guid callerId);
        Task<UsersPermissionsDTO> AssignUsersPermissionsAsync(NewUsersPermission newUserPermission, Guid callerId);

        Task<UserPermissions> RemoveUserPermissionsAsync(string userName, string predefinedRole, IList<Guid> accessList, Guid callerId);
        Task<IList<string>> GetAllRolesAsync();
        Task<IList<UserPermissions>> GetUserAdminsAsync();
        Task UpdatePermission(UserPermissions userPermission);
        Task<IList<UserPermissions>> GetCustomerAdminsAsync(Guid customerId);
        Task UpdateAccessListAsync(User user, List<Guid> accessList, Guid callerId);
        /// <summary>
        /// Only handles the change in user status from invited to onboardInitiated for the user.
        /// </summary>
        /// <param name="user">User object that should get user status onboardInitiated.</param>
        Task InitiateOnboarding(User user);
    }
}