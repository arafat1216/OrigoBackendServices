using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class PartnerConfiguration : IBaseGatewayOptions
    {
        public string ApiPath { get; set; }
    }
}
