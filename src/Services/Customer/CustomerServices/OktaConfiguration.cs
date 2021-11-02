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
        /// <summary>
        /// Endpoint to okta services/API
        /// </summary>
        public string OktaUrl { get; set; }
        /// <summary>
        /// Authorization token granting access to okta services
        /// </summary>
        public string OktaAuth { get; set; }

        /// <summary>
        /// Id of an application registered at okta
        /// </summary>
        public string OktaAppId { get; set; }

        /// <summary>
        /// Id of a group registered at okta
        /// </summary>
        public string OktaGroupId { get; set; }
    }
}
