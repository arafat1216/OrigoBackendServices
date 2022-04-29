using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class Line
    {
        [Required]
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [Required]
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("priceIncVat")]
        public string? PriceIncVat { get; set; }
        
        [JsonPropertyName("vatPercentage")]
        public string? VatPercentage { get; set; }
    }
}
