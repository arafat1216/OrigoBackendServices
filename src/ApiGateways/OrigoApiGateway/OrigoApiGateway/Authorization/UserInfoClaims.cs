using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace OrigoApiGateway.Authorization
{
    public class UserInfoClaims : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                var id = new ClaimsIdentity();
                id.AddClaim(new Claim(ClaimTypes.Role, "DeptAdmin"));
                id.AddClaim(new Claim("AccessList", "14df57bf-e398-41b7-badd-c784cebaa5b5"));
                id.AddClaim(new Claim("AccessList", "0773a632-fe57-11eb-8870-00155d8c99e4"));
                id.AddClaim(new Claim("Permissions", "CanCreateAssetPermission"));
                id.AddClaim(new Claim("Permissions", "CanReadAssetPermission"));
                principal.AddIdentity(id);
            }
            return Task.FromResult(principal);
        }
    }
}
