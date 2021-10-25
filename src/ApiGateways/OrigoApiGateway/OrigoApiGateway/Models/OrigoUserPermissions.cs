using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class OrigoUserPermissions
    {
        public OrigoUserPermissions(UserPermissionsDTO userPermissions)
        {
            PermissionNames = userPermissions.PermissionNames;
            AccessList = userPermissions.AccessList;
            Role = userPermissions.Role;
        }

        public OrigoUserPermissions(IList<string> permissionNames, IList<Guid> accessList, string role)
        {
            PermissionNames = permissionNames;
            AccessList = accessList;
            Role = role;
        }

        public string Role { get; }
        public IList<string> PermissionNames { get; }

        public IList<Guid> AccessList { get; }
    }
}
