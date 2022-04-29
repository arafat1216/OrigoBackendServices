using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class Order
    {
        [JsonPropertyName("CommId")]
        public string? CommId { get; set; }
    }
}
