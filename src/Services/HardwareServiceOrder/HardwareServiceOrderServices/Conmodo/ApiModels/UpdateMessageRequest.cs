using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class UpdateMessageRequest
    {
        [Required]
        [JsonPropertyName("messageId")]
        public int MessageId { get; set; }

        [JsonPropertyName("read")]
        public bool? Read { get; set; }
    }
}
