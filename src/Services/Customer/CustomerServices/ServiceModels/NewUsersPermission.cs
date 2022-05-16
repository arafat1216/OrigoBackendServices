using System;
using System.Collections.Generic;

namespace CustomerServices.ServiceModels
{
    /// <summary>
    /// Request model
    /// </summary>
    public class NewUsersPermission
    {
        /// <summary>
        /// List of user permission to be added for users
        /// </summary>
        public IList<NewUserPermissionDTO> UserPermissions { get; set; }
        /// <summary>
        /// Identification of the user making this changes
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
