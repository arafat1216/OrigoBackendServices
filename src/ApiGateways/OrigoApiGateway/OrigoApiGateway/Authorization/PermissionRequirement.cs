using Microsoft.AspNetCore.Authorization;
using System;

namespace OrigoApiGateway.Authorization
{
    /// <summary>
    /// Taken from https://blog.joaograssi.com/posts/2021/asp-net-core-protecting-api-endpoints-with-dynamic-policies/
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public static string ClaimType => AppClaimTypes.Permissions;

        // 1 - The operator
        public PermissionOperator PermissionOperator { get; }

        // 2 - The list of permissions passed
        public string[] Permissions { get; }

        public PermissionRequirement(
            PermissionOperator permissionOperator, string[] permissions)
        {
            if (permissions.Length == 0)
                throw new ArgumentException("At least one permission is required.", nameof(permissions));

            PermissionOperator = permissionOperator;
            Permissions = new string[permissions.Length];
            permissions.CopyTo(Permissions, 0);
        }
    }
}
