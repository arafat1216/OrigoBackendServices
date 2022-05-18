using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    /// <summary>
    /// Response model for users permissions
    /// </summary>
    public record UsersPermissions
    {
        /// <summary>
        /// List of user permission to be added for users
        /// </summary>
        public IList<UserPermission> UserPermissions { get; set; }
        /// <summary>
        /// List of strings with errors
        /// </summary>
        public IList<string> ErrorMessages { get; set; }

    }
}
