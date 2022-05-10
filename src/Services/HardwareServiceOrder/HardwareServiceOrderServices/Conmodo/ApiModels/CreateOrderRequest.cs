using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.Converters;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class CreateOrderRequest
    {
        /// <summary>
        ///     Your reference shown on the order label
        /// </summary>
        [Required]
        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        /// <summary>
        ///     Integration partner's order number
        /// </summary>
        [Required]
        [JsonPropertyName("commId")]
        public string CommId { get; set; }

        [Required]
        [JsonPropertyName("startStatus")]
        public StartStatus StartStatus { get; set; }

        [Required]
        [MaxLength(1000)]
        [MinLength(0)]
        [JsonPropertyName("errorDescription")]
        public string ErrorDescription { get; set; }

        [JsonConverter(typeof(DateOnlyNullableJsonConverter))]
        [JsonPropertyName("boughtDate")]
        public DateOnly? BoughtDate { get; set; }

        [JsonPropertyName("maxRepairPrice")]
        public float? MaxRepairPrice { get; set; }

        [JsonPropertyName("productInfo")]
        public ProductInfo? ProductInfo { get; set; }

        // Conmodo's openapi definition has this marked as "uniqueItems: true", so we use ISet to enforce the uniqueness. 
        /// <summary>
        ///     IDs to valid extra services are supplied by Conmodo
        /// </summary>
        [JsonPropertyName("extraServices")]
        public ISet<int>? ExtraServices { get; set; }

        [JsonPropertyName("owner")]
        public Contact? Owner { get; set; }

        [Required]
        [JsonPropertyName("customerHandler")]
        public Contact CustomerHandler { get; set; }

        // TODO: Why is this not implemented in Conmodo when it is added to their docs?
        /// <summary>
        ///     The order number in Conmodo's service system
        /// </summary>
        [JsonIgnore]
        [JsonPropertyName("orderNumber")]
        public int? OrderNumber { get; set; }

        /// <summary>
        ///     Optional field for cost place
        /// </summary>
        [JsonPropertyName("costPlace")]
        public string? CostPlace { get; set; }

        /// <summary>
        ///     Optional field for customers reference
        /// </summary>
        [JsonPropertyName("customerReference")]
        public string? CustomerReference { get; set; }

        /// <summary>
        ///     Optional field for invoice reference
        /// </summary>
        [JsonPropertyName("invoiceReference")]
        public string? InvoiceReference { get; set; }


        /// <summary>
        ///     System-reserved constructor. Should not be used!
        /// </summary>
        [Obsolete("Reserved constructor for the JSON serializers.", error: true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public CreateOrderRequest()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }


        public CreateOrderRequest(string commId, string reference, Contact customerHandler, StartStatus startStatus, string errorDescription)
        {
            CommId = commId;
            Reference = reference;
            CustomerHandler = customerHandler;
            StartStatus = startStatus;
            ErrorDescription = errorDescription;
        }

        public CreateOrderRequest(string commId, string reference, Contact customerHandler, StartStatus startStatus, string errorDescription, ProductInfo productInfo, DateOnly? purchaseDate, Contact serviceRequestOwner)
        {
            CommId = commId;
            Reference = reference;
            CustomerHandler = customerHandler;
            StartStatus = startStatus;
            ErrorDescription = errorDescription;

            ProductInfo = productInfo;
            BoughtDate = purchaseDate;
            Owner = serviceRequestOwner;
        }
    }
}
