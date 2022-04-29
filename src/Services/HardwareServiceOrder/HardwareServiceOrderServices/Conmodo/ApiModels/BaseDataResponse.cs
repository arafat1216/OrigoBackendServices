using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class BaseDataResponse
    {
        [JsonPropertyName("categories")]
        public List<string>? Categories { get; set; }

        [JsonPropertyName("brands")]
        public List<string>? Brands { get; set; }

        [JsonPropertyName("statuses")]
        public List<Status>? Statuses { get; set; }

        [JsonPropertyName("accessories")]
        public List<AccessoryData>? Accessories { get; set; }
    }
}
