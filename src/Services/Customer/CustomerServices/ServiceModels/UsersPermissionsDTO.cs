using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.ServiceModels
{
    public class UsersPermissionsDTO
    {
        /// <summary>
        /// List of user permission to be added for users
        /// </summary>
        public IList<NewUserPermissionDTO> UserPermissions { get; set; }
        /// <summary>
        /// List of strings with errors
        /// </summary>
        public IList<string> ErrorMessages { get; set; }
    }
}
