using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     A suggestion containing available options for the order: repair, return or swap etc.
    /// </summary>
    internal class PurchaseOrderSuggestion
    {
        [JsonPropertyName("RepairPrice")]
        public decimal? RepairPrice { get; set; }

        [JsonPropertyName("ExchangeUnitPrice")]
        public decimal? ExchangeUnitPrice { get; set; }

        [JsonPropertyName("ExchangeSimilarUnitName")]
        public string? ExchangeSimilarUnitName { get; set; }

        [JsonPropertyName("ExchangeSimilarUnitPrice")]
        public decimal? ExchangeSimilarUnitPrice { get; set; }

        [JsonPropertyName("ExchangeUpgradeUnitName")]
        public string? ExchangeUpgradeUnitName { get; set; }

        [JsonPropertyName("ExchangeUpgradeUnitPrice")]
        public decimal? ExchangeUpgradeUnitPrice { get; set; }

        [Required]
        [JsonPropertyName("ReturnUnrepairedPrice")]
        public decimal ReturnUnrepairedPrice { get; set; }

        [Required]
        [JsonPropertyName("KeepPrice")]
        public decimal KeepPrice { get; set; }

        /// <summary>
        ///     The format of the currency follows ISO 4217.
        ///     Link: http://www.xe.com/iso4217.htm
        /// </summary>
        [Required]
        [JsonPropertyName("Currency")]
        public string Currency { get; set; }

    }
}
