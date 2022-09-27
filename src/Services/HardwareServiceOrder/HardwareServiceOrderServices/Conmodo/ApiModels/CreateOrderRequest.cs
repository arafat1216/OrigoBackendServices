using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.Converters;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class CreateOrderRequest
    {
        /// <summary>
        ///     Used as the customers/users reference. The field will appear on repair documents and invoices.
        /// </summary>
        /// <example> "CustomersReference1" </example>
        [Required]
        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        /// <summary>
        ///     The integration partner's unique identifier or order-number. This is a set by the partner (in this case our solution), and can be used as
        ///     an alternative identifier when retrieving orders in the API.
        /// </summary>
        /// <example> "PartnersCommId_001" </example>
        [Required]
        [JsonPropertyName("commId")]
        public string CommId { get; set; }

        [Required]
        [JsonPropertyName("startStatus")]
        public StartStatus StartStatus { get; set; }

        /// <summary>
        ///     Describes what's wrong with the device.
        /// </summary>
        [Required]
        [MaxLength(1000)]
        [MinLength(0)]
        [JsonPropertyName("errorDescription")]
        public string ErrorDescription { get; set; }

        [JsonConverter(typeof(DateOnlyNullableJsonConverter))]
        [JsonPropertyName("boughtDate")]
        public DateOnly? BoughtDate { get; set; }

        /// <summary>
        ///     If set, Conmdo will repair up to that price without providing a cost estimate. If repair exceeds this value, a cost estimate is provided.
        /// </summary>
        [JsonPropertyName("maxRepairPrice")]
        public float? MaxRepairPrice { get; set; }

        /// <summary>
        ///     Information about the provided device.
        /// </summary>
        [JsonPropertyName("productInfo")]
        public ProductInfo? ProductInfo { get; set; }

        // Note: Conmodo's OpenAPI documentation has this marked as "uniqueItems: true", so we should use ISet to ensure/enforce uniqueness.
        /// <summary>
        ///     Defines additional service that should to be added to a repair, such as freight to/from customer, preswap, backups, etc.
        /// </summary>
        /// <example> [272,279] </example>
        [JsonPropertyName("extraServices")]
        public ISet<int>? ExtraServices { get; set; }

        /// <summary>
        ///     Owner is the user of the device (from Conmodo's perspective, meaning the person handling the service)
        ///     This information is used for transportation etc.
        /// </summary>
        [JsonPropertyName("owner")]
        public Contact? Owner { get; set; }

        [Required]
        [JsonPropertyName("customerHandler")]
        public Contact CustomerHandler { get; set; }

        // TODO: This is specified in their documentation, but don't seem to be accepted by their endpoint.
        // Why is this not implemented in Conmodo when it is added to their docs?!
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commId"></param>
        /// <param name="reference"></param>
        /// <param name="customerHandler"></param>
        /// <param name="startStatus"></param>
        /// <param name="userDescription"></param>
        /// <param name="productInfo"></param>
        /// <param name="purchaseDate"></param>
        /// <param name="serviceRequestOwner"></param>
        /// <param name="extraServices"></param>
        public CreateOrderRequest(string commId, string reference, Contact customerHandler, StartStatus startStatus, string userDescription, ProductInfo productInfo, DateOnly? purchaseDate, Contact serviceRequestOwner, ISet<int>? extraServices)
        {
            CommId = commId;
            Reference = reference;
            CustomerHandler = customerHandler;
            StartStatus = startStatus;
            ErrorDescription = userDescription;

            ProductInfo = productInfo;
            BoughtDate = purchaseDate;
            Owner = serviceRequestOwner;

            ExtraServices = extraServices;
        }
    }
}
