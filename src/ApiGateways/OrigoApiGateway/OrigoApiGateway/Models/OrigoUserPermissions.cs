using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Response object
    /// </summary>
    public class OrigoUserPermissions
    {
        public string Role { get; init; }

        public IList<string> PermissionNames { get; init; }

        public IList<Guid> AccessList { get; init; }
    }
}
