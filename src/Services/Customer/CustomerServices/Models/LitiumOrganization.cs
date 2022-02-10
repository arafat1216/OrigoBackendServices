using Newtonsoft.Json;
using System;

namespace CustomerServices.Models
{
    //[JsonObject("organization")]
    public class LitiumOrganization
    {
        [JsonProperty("organizationName")]
        public string OrganizationName { get; set; } = string.Empty;

        /// <summary>
        /// Organization Id in Litium.
        /// </summary>
        [JsonProperty("organizationId")]
        public Guid OrganizationId { get; set; } = Guid.Empty;

        /// <summary>
        /// Organization number.
        /// </summary>
        [JsonProperty("legalRegistrationNumber")]
        public string LegalRegistrationNumber { get; set; } = string.Empty;
    }
}
