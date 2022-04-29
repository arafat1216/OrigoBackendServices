using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class PriceSuggestionReply
    {
        [Required]
        [JsonPropertyName("MessageID")]
        public int MessageID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        ///     REPAIR, EXCHANGE_UNIT, RETURN_UNREPAIRED or KEEP
        /// </example>
        [Required]
        [JsonPropertyName("PurchaseOrderType")]
        public string PurchaseOrderType { get; set; }
    }
}
