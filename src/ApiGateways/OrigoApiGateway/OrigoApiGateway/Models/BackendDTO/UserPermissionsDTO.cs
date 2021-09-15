using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class UserPermissionsDTO
    {
        public UserPermissionsDTO(IReadOnlyCollection<string> permissionNames, IReadOnlyCollection<Guid> accessList, string role)
        {
            PermissionNames = permissionNames;
            AccessList = accessList;
            Role = role;
        }

        public string Role { get; }
        public IReadOnlyCollection<string> PermissionNames { get; }

        public IReadOnlyCollection<Guid> AccessList { get; }
    }
}