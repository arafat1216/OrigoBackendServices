using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     This is used to sort the orders at Conmodo. 
    ///     A StartStatus is for instance Warranty, Insurance or similar. See "basedata API" for valid values.
    /// </summary>
    internal class StartStatus
    {
        [JsonPropertyName("startStatusID")]
        public int? StartStatusID { get; set; }

        [JsonPropertyName("startStatusName")]
        public string? StartStatusName { get; set; }

        [JsonPropertyName("subStartStatusID")]
        public int? SubStartStatusID { get; set; }

        [JsonPropertyName("subStartStatusName")]
        public string? SubStartStatusName { get; set; }

        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

    }
}
