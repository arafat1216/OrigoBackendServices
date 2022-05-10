using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class UpdatedOrdersResponse
    {
        [JsonPropertyName("order")]
        public IEnumerable<Order>? Order { get; set; }
    }
}
