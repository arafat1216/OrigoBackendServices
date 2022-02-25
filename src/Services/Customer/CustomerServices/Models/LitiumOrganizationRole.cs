using System.Text.Json.Serialization;

namespace CustomerServices.Models
{
    public class LitiumOrganizationRole
    {
        [JsonPropertyName("organization")]
        public LitiumOrganization Organization { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
    }
}
