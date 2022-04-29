using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class NotifyShipmentResponse
    {
        [Required]
        [JsonPropertyName("packageTransporter")]
        public string PackageTransporter { get; set; }

        [Required]
        [JsonPropertyName("packageIdentifier")]
        public string PackageIdentifier { get; set; }

        [Required]
        [JsonPropertyName("packagePrintURL")]
        public string PackagePrintURL { get; set; }
    }
}
