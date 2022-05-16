using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Gateway request model 
    /// </summary>
    public record NewUserPermission
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
        /// The list of guids to be used to check accsess
        /// </summary>
        public IList<Guid> AccessList { get; set; }

    }
}
