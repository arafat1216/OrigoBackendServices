using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class UserPermissionsDTO
    {
        public UserPermissionsDTO(IList<string> permissionNames, IList<Guid> accessList, string role)
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