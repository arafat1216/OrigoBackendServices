using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record NewUserPermissionsDTO
    {
        public string Role { get; set; }
        public IList<Guid> AccessList { get; set; }
        public Guid CallerId { get; set; }

    }
}
