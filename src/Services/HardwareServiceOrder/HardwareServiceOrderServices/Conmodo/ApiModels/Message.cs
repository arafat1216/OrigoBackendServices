using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     Object that contains message data, can contain purchase order suggestion
    /// </summary>
    internal class Message
    {
        /// <summary>
        ///     ID in Conmodo's system
        /// </summary>
        [Required]
        [JsonPropertyName("messageID")]
        public int MessageID { get; set; }

        [Required]
        [JsonPropertyName("message")]
        public string message { get; set; }

        // TODO: Do we need to change this to a DateTime?
        [Required]
        [JsonPropertyName("createdDateTime")]
        public DateTimeOffset CreatedDateTime { get; set; }

        /// <summary>
        ///     Read by the recipient
        /// </summary>
        [JsonPropertyName("read")]
        public bool? Read { get; set; }

        [Required]
        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("purchaseOrderSuggestion")]
        public PurchaseOrderSuggestion? PurchaseOrderSuggestion { get; set; }
    }
}
