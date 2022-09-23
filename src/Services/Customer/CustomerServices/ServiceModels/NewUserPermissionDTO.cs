using System;
using System.Collections.Generic;

namespace CustomerServices.ServiceModels
{
    /// <summary>
    /// Request model
    /// </summary>
    public class NewUserPermissionDTO
    {
        /// <summary>
        /// Identification of user
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Role name adding to the users permission
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// The list of guids to be used to check access
        /// </summary>
        public IList<Guid> AccessList { get; set; }
    }
}
