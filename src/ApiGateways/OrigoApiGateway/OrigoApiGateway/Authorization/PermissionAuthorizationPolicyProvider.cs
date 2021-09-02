#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using static OrigoApiGateway.Authorization.PermissionAuthorizeAttribute;

namespace OrigoApiGateway.Authorization
{
    /// <summary>
    /// Taken from https://blog.joaograssi.com/posts/2021/asp-net-core-protecting-api-endpoints-with-dynamic-policies/
    /// </summary>
    public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options) { }

        /// <inheritdoc />
        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // it's not one of our dynamic policies, so we fallback to the base behavior
                // this will load policies added in Startup.cs (AddPolicy..)
                return await base.GetPolicyAsync(policyName);
            }

            var @operator = GetOperatorFromPolicy(policyName);
            var permissions = GetPermissionsFromPolicy(policyName);

            // extract the info from the policy name and create our requirement
            var requirement = new PermissionRequirement(@operator, permissions);

            // create and return the policy for our requirement
            return new AuthorizationPolicyBuilder().AddRequirements(requirement).Build();
        }
    }
}
