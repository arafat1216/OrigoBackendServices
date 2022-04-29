using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class Delivery
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }
        
        [JsonPropertyName("postCity")]
        public string? PostCity { get; set; }

        [JsonPropertyName("postNumber")]
        public string? PostNumber { get; set; }
    }
}
