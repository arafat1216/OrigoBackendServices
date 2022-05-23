using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     The result of an OrderRequest
    /// </summary>
    internal class OrderResponse
    {
        /// <summary>
        ///     Your reference shown on the order label"
        /// </summary>
        [Required]
        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        /// <summary>
        ///     Conmodo's order no.
        /// </summary>
        [Required]
        [JsonPropertyName("deltaOrderNumber")]
        public int DeltaOrderNumber { get; set; }

        /// <summary>
        ///     A description of the work on the order
        /// </summary>
        [Required]
        [JsonPropertyName("workDescription")]
        public string WorkDescription { get; set; }

        /// <summary>
        ///     Conmodo's ID for the Workshop assigned the order
        /// </summary>
        [Required]
        [JsonPropertyName("workshopID")]
        public int WorkshopID { get; set; }

        [JsonPropertyName("productInfoIn")]
        public ProductInfo? ProductInfoIn { get; set; }

        [JsonPropertyName("productInfoOut")]
        public ProductInfo? ProductInfoOut { get; set; }

        /// <summary>
        ///     Events on the order
        /// </summary>
        [Required]
        [JsonPropertyName("events")]
        public List<Event> Events { get; set; }

        /// <summary>
        ///     Images on the order
        /// </summary>
        [Required]
        [JsonPropertyName("images")]
        public List<Image> Images { get; set; }

        /// <summary>
        ///     Messages on the order
        /// </summary>
        [Required]
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }

        /// <summary>
        ///     Packages on the order
        /// </summary>
        [Required]
        [JsonPropertyName("packages")]
        public List<Package> Packages { get; set; }

        /// <summary>
        ///     Documents on the order, receipts etc.
        /// </summary>
        [JsonPropertyName("documents")]
        public List<Document>? Documents { get; set; }

        [JsonPropertyName("payment")]
        public Payment? Payment { get; set; }

        /// <summary>
        ///     URL for order's print label
        /// </summary>
        [JsonPropertyName("orderPrintURL")]
        public string? OrderPrintURL { get; set; }

        /// <summary>
        ///     Included in API responses, but not included in Conmodo's API documentation. Contains the IMEI number of the registered device.
        /// </summary>
        [JsonPropertyName("registeredImeiIn")]
        public string? RegisteredImeiIn { get; set; }
    }
}