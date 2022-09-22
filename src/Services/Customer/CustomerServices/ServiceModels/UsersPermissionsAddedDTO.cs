using System.Collections.Generic;

namespace CustomerServices.ServiceModels
{
    public class UsersPermissionsAddedDTO
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
