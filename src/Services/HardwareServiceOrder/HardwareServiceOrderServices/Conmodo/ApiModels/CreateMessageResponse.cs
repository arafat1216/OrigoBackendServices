using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class CreateMessageResponse
    {
        [Required]
        [JsonPropertyName("messageID")]
        public int MessageID { get; set; }
    }
}
