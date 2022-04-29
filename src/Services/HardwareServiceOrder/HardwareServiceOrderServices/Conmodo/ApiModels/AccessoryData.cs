using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class AccessoryData
    {
        [JsonPropertyName("accessoryName")]
        public string? AccessoryName { get; set; }

        [JsonPropertyName("categoryName")]
        public string? CategoryName { get; set; }
    }
}
