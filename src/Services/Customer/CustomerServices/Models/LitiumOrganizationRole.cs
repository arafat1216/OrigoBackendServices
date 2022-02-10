using Newtonsoft.Json;

namespace CustomerServices.Models
{
    public class LitiumOrganizationRole
    {
        [JsonProperty("organization")]
        public LitiumOrganization Organization { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;
    }
}
