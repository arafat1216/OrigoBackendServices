using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     Payment on the order, will only be set when the order is finished
    /// </summary>
    internal class Payment
    {
        [Required]
        [JsonPropertyName("lines")]
        public List<Line> Lines { get; set; }

        [JsonPropertyName("sum")]
        public Sum? Sum { get; set; }

        /// <summary>
        ///     Who pays for line, Customer or None
        /// </summary>
        /// <example> Customer </example>
        [Required]
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
