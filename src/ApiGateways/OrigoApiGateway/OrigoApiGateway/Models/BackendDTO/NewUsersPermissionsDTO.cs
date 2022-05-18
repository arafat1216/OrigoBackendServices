using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record NewUsersPermissionsDTO
    {
        public IList<NewUserPermission> UserPermissions { get; set; }
        public Guid CallerId { get; set; }
    }
}
