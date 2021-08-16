using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace OrigoApiGateway.Authorization
{
    /// <summary>
    /// From https://codewithmukesh.com/blog/permission-based-authorization-in-aspnet-core/
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        public PermissionAuthorizationHandler() { }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var permissions = context.User.Claims.Where(x => x.Type == "Permission" &&
                                                              x.Value == requirement.Permission);
            if (permissions.Any())
            {
                context.Succeed(requirement);
            }

            return;
        }
    }
}
