using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Frontend viewmodel for multiple user permissions
    /// </summary>
    public record OrigoUsersPermissions
    {
        /// <summary>
        /// List of user permissions
        /// </summary>
        public IList<OrigoUserPermission> UserPermissions { get; set; }
        /// <summary>
        /// List of strings with errors
        /// </summary>
        public IList<string> ErrorMessages { get; set; }
    }
}
