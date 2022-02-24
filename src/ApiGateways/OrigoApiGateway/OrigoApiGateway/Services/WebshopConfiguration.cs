using System.Collections.Generic;

namespace OrigoApiGateway.Services
{
    public class WebshopConfiguration : IBaseGatewayOptions
    {
        public string ApiPath { get; set; }
        public string Issuer { get; set; }
        public IEnumerable<string> Audiences { get; set; }
        public string WebshopRedirectUrl { get; set; }
    }
}
