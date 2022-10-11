using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public class UserPermissions
    {
        public UserPermissions(IReadOnlyCollection<string> permissionNames, IReadOnlyCollection<Guid> accessList, string role, Guid userId, Guid mainOrganizationId)
        {
            PermissionNames = permissionNames;
            AccessList = accessList;
            Role = role;
            UserId = userId;
            MainOrganizationId = mainOrganizationId;
        }

        public string Role { get; }
        public Guid UserId { get; }
        public Guid MainOrganizationId { get; }
        public IReadOnlyCollection<string> PermissionNames { get; }

        public IReadOnlyCollection<Guid> AccessList { get; }
    }
}