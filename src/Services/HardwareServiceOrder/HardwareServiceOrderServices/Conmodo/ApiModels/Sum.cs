using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class Sum
    {
        [Required]
        [JsonPropertyName("PriceIncVat")]
        public string PriceIncVat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <example>NOK</example>
        [Required]
        [JsonPropertyName("Currency")]
        public string Currency { get; set; }
    }
}
