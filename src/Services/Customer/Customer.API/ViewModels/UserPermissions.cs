using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public class UserPermissions
    {
        public UserPermissions(IReadOnlyCollection<string> permissionNames, IReadOnlyCollection<Guid> accessList, string role, Guid userId)
        {
            PermissionNames = permissionNames;
            AccessList = accessList;
            Role = role;
            UserId = userId;
        }

        public string Role { get; }
        public Guid UserId { get; }
        public IReadOnlyCollection<string> PermissionNames { get; }

        public IReadOnlyCollection<Guid> AccessList { get; }
    }
}