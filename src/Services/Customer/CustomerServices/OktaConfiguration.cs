using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices
{
    /// <summary>
    /// Okta settings are in Customer.API.Appsettings. This class is set to those values, and injected into the OktaServices class,
    /// which allows OktaServices access to those values, despite being in a different project.
    /// </summary>
    public class OktaConfiguration
    {
        public string OktaUrl { get; set; }
        public string OktaAuth { get; set; }
        public string OktaAppId { get; set; }
    }
}
