using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerServices.Models;

namespace CustomerServices
{
    public interface IUserPermissionServices
    {
        Task<IEnumerable<UserPermissions>> GetUserPermissionsAsync(string userName);
    }
}