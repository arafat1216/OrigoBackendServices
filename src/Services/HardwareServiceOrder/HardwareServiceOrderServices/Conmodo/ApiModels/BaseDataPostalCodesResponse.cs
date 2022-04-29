using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class BaseDataPostalCodesResponse
    {
        [JsonPropertyName("list")]
        public List<BaseDataPostalCodeData>? List { get; set; }
    }
}
