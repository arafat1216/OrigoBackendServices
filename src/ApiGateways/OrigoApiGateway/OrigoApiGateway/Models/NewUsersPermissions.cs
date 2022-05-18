using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public record NewUsersPermissions
    {
        public IList<NewUserPermission> UserPermissions { get; set; }
    }
}
