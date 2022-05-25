using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class PriceSuggestionReply
    {
        /// <summary>
        ///     MessageId is the id for the cost-proposal given from Conmodo. 
        /// </summary>
        [Required]
        [JsonPropertyName("MessageID")]
        public int MessageID { get; set; }

        /// <summary>
        ///     Actual reply to the cost-proposal
        /// </summary>
        /// <example>
        ///     REPAIR, EXCHANGE_UNIT, RETURN_UNREPAIRED or KEEP
        /// </example>
        [Required]
        [JsonPropertyName("PurchaseOrderType")]
        public string PurchaseOrderType { get; set; }
    }
}
