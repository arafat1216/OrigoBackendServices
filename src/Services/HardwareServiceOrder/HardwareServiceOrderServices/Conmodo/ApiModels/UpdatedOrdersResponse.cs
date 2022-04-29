using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class UpdatedOrdersResponse
    {
        [JsonPropertyName("order")]
        public List<Order>? Order { get; set; }
    }
}
