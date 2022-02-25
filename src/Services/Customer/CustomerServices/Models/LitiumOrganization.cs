using System;
using System.Text.Json.Serialization;

namespace CustomerServices.Models
{
    //[JsonObject("organization")]
    public class LitiumOrganization
    {
        [JsonPropertyName("organizationName")]
        public string OrganizationName { get; set; } = string.Empty;

        /// <summary>
        /// Organization Id in Litium.
        /// </summary>
        [JsonPropertyName("organizationId")]
        public Guid OrganizationId { get; set; } = Guid.Empty;

        /// <summary>
        /// Organization number.
        /// </summary>
        [JsonPropertyName("legalRegistrationNumber")]
        public string LegalRegistrationNumber { get; set; } = string.Empty;
    }
}
