using System.Collections.Generic;
using System;

namespace CustomerServices.ServiceModels
{
    public class UserPermissionsDTO
    {
        public UserPermissionsDTO(IReadOnlyCollection<string> permissionNames, IReadOnlyCollection<Guid> accessList, string role, Guid userId)
        {
            PermissionNames = permissionNames;
            AccessList = accessList;
            Role = role;
            UserId = userId;
        }

        public UserPermissionsDTO()
        {

        }

        public string Role { get; set; }
        public Guid UserId { get; set; }
        public IReadOnlyCollection<string> PermissionNames { get; set; }

        public IReadOnlyCollection<Guid> AccessList { get; set; }
    }
}
