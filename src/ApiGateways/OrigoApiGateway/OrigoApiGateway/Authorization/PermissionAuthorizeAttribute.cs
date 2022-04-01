using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace OrigoApiGateway.Authorization
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        internal const string PolicyPrefix = "PERMISSION_";
        private const string Separator = "_";

        /// <summary>
        /// Initializes the attribute with multiple permissions
        /// </summary>
        /// <param name="permissionOperator">The operator to use when verifying the permissions provided</param>
        /// <param name="permissions">The list of permissions</param>
        public PermissionAuthorizeAttribute(PermissionOperator permissionOperator, params Permission[] permissions)
        {
            // E.g: PERMISSION_1_Create_Update..
            string[] permissionsAsStringValues = permissions.Select(p => p.ToString()).ToArray();
            Policy = $"{PolicyPrefix}{(int)permissionOperator}{Separator}{string.Join(Separator, permissionsAsStringValues)}";
        }

        /// <summary>
        /// Initializes the attribute with a single permission
        /// </summary>
        /// <param name="permission">The permission</param>
        public PermissionAuthorizeAttribute(Permission permission)
        {
            // E.g: PERMISSION_1_Create..
            Policy = $"{PolicyPrefix}{(int)PermissionOperator.And}{Separator}{permission}";
        }

        public static PermissionOperator GetOperatorFromPolicy(string policyName)
        {
            var @operator = int.Parse(policyName.AsSpan(PolicyPrefix.Length, 1));
            return (PermissionOperator)@operator;
        }

        public static string[] GetPermissionsFromPolicy(string policyName)
        {
            return policyName.Substring(PolicyPrefix.Length + 2)
                .Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}