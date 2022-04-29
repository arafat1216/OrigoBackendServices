using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     Object that contains an image URL, description and Id
    /// </summary>
    internal class Image
    {
        [Required]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("url")]
        public int Url { get; set; }

        [Required]
        [JsonPropertyName("description")]
        public int Description { get; set; }
    }
}
