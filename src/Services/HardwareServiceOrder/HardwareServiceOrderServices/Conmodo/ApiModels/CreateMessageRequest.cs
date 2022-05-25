using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class CreateMessageRequest
    {
        /// <summary>
        ///     Integration partner's order number
        /// </summary>
        [Required]
        [JsonPropertyName("commId")]
        public string CommId { get; set; }

        /// <summary>
        ///     Message text to Conmodo
        /// </summary>
        [Required]
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [Required]
        [JsonPropertyName("customerHandler")]
        public Contact CustomerHandler { get; set; }

        /// <summary>
        ///     For replying to cost-proposals, this object must be included.
        /// </summary>
        [JsonPropertyName("priceSuggestionReply")]
        public PriceSuggestionReply? PriceSuggestionReply { get; set; }
    }
}
