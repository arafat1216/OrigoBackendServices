using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     Object that contains an order event, in /basedata
    /// </summary>
    internal class Event
    {
        /// <summary>
        /// 
        /// </summary>
        /// <example> Ferdig </example>
        [JsonPropertyName("eventName")]
        public string? EventName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <example> 13 </example>
        [JsonPropertyName("eventId")]
        public int? EventId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <example> Detailing the event </example>
        [JsonPropertyName("subEventName")]
        public string? SubEventName { get; set; }

        [JsonPropertyName("subEventId")]
        public int? SubEventId { get; set; }

        // TODO: Does this have to be DateTime instead, or can we use DateTimeOffset?
        [Required]
        [JsonPropertyName("eventDateTime")]
        public DateTimeOffset EventDateTime { get; set; }
    }
}
