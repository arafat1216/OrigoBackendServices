using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class Order
    {
        [JsonPropertyName("CommId")]
        public string? CommId { get; set; }

        // This exists in their API, but not in the documentation, because why would it ¯\_(ツ)_/¯
        [JsonPropertyName("orderNo")]
        public int? OrderNo { get; set; }
    }
}
