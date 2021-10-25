using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class UserPermissionsConfigurations : IBaseGatewayOptions
    {
        public string ApiBaseUrl { get; set; }
        public string ApiPort { get; set; }
        public string ApiPath { get; set; }
    }
}
