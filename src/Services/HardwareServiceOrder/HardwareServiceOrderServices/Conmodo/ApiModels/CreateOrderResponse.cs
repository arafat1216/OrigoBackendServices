using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class CreateOrderResponse
    {
        // TODO: This is an unknown datatype in Conmodo's docs.. We assume it's a string value
        /// <summary>
        ///     Dealer's ID in Conmodo's system
        /// </summary>
        [Required]
        [JsonPropertyName("dealerId")]
        public string DealerId { get; set; }

        /// <summary>
        ///     Conmodo's order number
        /// </summary>
        [Required]
        [JsonPropertyName("orderNumber")]
        public int OrderNumber { get; set; }

        /// <summary>
        ///     URL for printing the order label
        /// </summary>
        [Required]
        [JsonPropertyName("orderPrintURL")]
        public string OrderPrintURL { get; set; }

        /// <summary>
        ///     Customer URL for order status
        /// </summary>
        [JsonPropertyName("directCustomerLink")]
        public string? DirectCustomerLink { get; set; }

        /// <summary>
        ///     Sorting information used by the workshop, typically brand/model
        /// </summary>
        [JsonPropertyName("sortingLocation1")]
        public string? SortingLocation1 { get; set; }

        [JsonPropertyName("sortingLocation2")]
        public string? SortingLocation2 { get; set; }

        [JsonPropertyName("sortingLocation3")]
        public string? SortingLocation3 { get; set; }
    }
}
