using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class NotifyShipmentRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <example> Post, CarDelivery, ThirdParty, PostNord or DHL </example>
        [Required]
        [JsonPropertyName("packageTransporter")]
        public string PackageTransporter { get; set; }

        [Required]
        [JsonPropertyName("packageIdentifier")]
        public string PackageIdentifier { get; set; }

        [Required]
        [JsonPropertyName("orders")]
        public List<Order> Orders { get; set; }
    }
}
