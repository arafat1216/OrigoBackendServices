using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     Documents on the order, receipts etc.
    /// </summary>
    internal class Document
    {
        /// <summary>
        ///     ID in Conmodo's system
        /// </summary>
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        /// <summary>
        ///     URL where the file is publicly available
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        ///     Description of the file
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
