using System.ComponentModel.DataAnnotations;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     The device-details that is sent to the service-providers.
    /// </summary>
    public class AssetInfoDTO
    {
        /// <summary>
        ///     The asset's brand.
        /// </summary>
        /// <example> Samsung </example>
        [Required]
        public string Brand { get; set; }

        /// <summary>
        ///     The asset's model.
        /// </summary>
        /// <example> Galaxy S7 </example>
        [Required]
        public string Model { get; set; }

        /// <summary>
        ///     The asset's category ID.
        /// </summary>
        /// <example> 1 </example>
        public int? AssetCategoryId { get; set; }

        /// <summary>
        ///     The asset's IMEI number, if available. <para>
        ///     
        ///     IMEI is always required for phones. For other asset-types this is required if <c><see cref="SerialNumber"/></c> is not provided. </para>
        /// </summary>
        /// <example> 498973602928506 </example>
        [RegularExpression("^[0-9]")]
        [MinLength(14, ErrorMessage = "IMEI is too short.")]
        [MaxLength(15, ErrorMessage = "IMEI is too long.")]
        public string? Imei { get; set; }

        /// <summary>
        ///     The asset's serial-number. <para>
        ///     
        ///     Required if <c><see cref="Imei"/></c> is not provided. </para>
        /// </summary>
        public string? SerialNumber { get; set; }

        /// <summary>
        ///     An optional list of accessories that is/will be sent in along with the asset.
        /// </summary>
        /// <example> Charger </example>
        public IEnumerable<string>? Accessories { get; set; }

        /// <summary>
        ///     The original purchase-date.
        /// </summary>
        public DateOnly? PurchaseDate { get; set; }
    }
}
