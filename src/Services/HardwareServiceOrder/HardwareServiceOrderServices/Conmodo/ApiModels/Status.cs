using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class Status
    {
        /// <summary>
        ///     Initial status
        /// </summary>
        [Required]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        ///     Status description
        /// </summary>
        [Required]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        ///     Sub-status
        /// </summary>
        [JsonPropertyName("subStatusId")]
        public int? SubStatusId { get; set; }

        /// <summary>
        ///     Detailing the status
        /// </summary>
        [JsonPropertyName("subStatusDescription")]
        public string? SubStatusDescription { get; set; }

        /// <summary>
        ///     Status type
        /// </summary>
        [JsonPropertyName("typeid")]
        public int? TypeId { get; set; }

        /// <summary>
        ///     Status type description
        /// </summary>
        [JsonPropertyName("typeDescription")]
        public string? TypeDescription { get; set; }
    }
}
