using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     Packages on the order
    /// </summary>
    internal class Package
    {
        /// <summary>
        ///     Post or CarDelivery
        /// </summary>
        /// <example> CarDelivery </example>
        [Required]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        ///     Service or Dealer
        /// </summary>
        /// <example> Dealer </example>
        [Required]
        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        /// <summary>
        ///     Full package number, which allows tracking on carrier's service
        /// </summary>
        [Required]
        [JsonPropertyName("packagenumber")]
        public string Packagenumber { get; set; }

        /// <summary>
        ///     Points to packtrack information from for instance www.posten.no about the package
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
