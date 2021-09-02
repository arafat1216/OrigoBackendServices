using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerServices.Infrastructure;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices
{
    public class UserPermissionServices : IUserPermissionServices
    {
        private readonly CustomerContext _customerContext;

        public UserPermissionServices(CustomerContext customerContext)
        {
            _customerContext = customerContext;
        }

        public async Task<IEnumerable<UserPermissions>> GetUserPermissionsAsync(string userName)
        {
            return await _customerContext.UserPermissions
                .Include(up => up.Role).ThenInclude(r => r.GrantedPermissions)
                .Include(up => up.User)
                .Where(up => up.User.Email == userName).ToListAsync();
        }
    }
}