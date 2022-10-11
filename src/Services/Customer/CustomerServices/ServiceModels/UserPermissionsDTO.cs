using System.Collections.Generic;
using System;

namespace CustomerServices.ServiceModels
{
    public class UserPermissionsDTO
    {
        public UserPermissionsDTO(IReadOnlyCollection<string> permissionNames, IReadOnlyCollection<Guid> accessList, string role, Guid userId, Guid mainOrganizationId)
        {
            PermissionNames = permissionNames;
            AccessList = accessList;
            Role = role;
            UserId = userId;
            MainOrganizationId = mainOrganizationId;
        }

        public UserPermissionsDTO()
        {

        }

        public string Role { get; set; }
        public Guid UserId { get; set; }
        public IReadOnlyCollection<string> PermissionNames { get; set; }

        public IReadOnlyCollection<Guid> AccessList { get; set; }
        public Guid MainOrganizationId { get; set; }
    }
}
