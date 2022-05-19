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
        ///     This is required when creating a new service-request, but may be <see langword="null"/> when it's retrieved from the external service-provider.
        /// </summary>
        /// <example> Samsung </example>
        public string? Brand { get; set; }

        /// <summary>
        ///     The asset's model.
        ///     This is required when creating a new service-request, but may be <see langword="null"/> when it's retrieved from the external service-provider.
        /// </summary>
        /// <example> Galaxy S7 </example>
        [Required]
        public string? Model { get; set; }

        /// <summary>
        ///     The asset's category ID.
        /// </summary>
        /// <example> 1 </example>
        [Required]
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
        ///     The original purchase-date.
        /// </summary>
        public DateOnly? PurchaseDate { get; set; }

        /// <summary>
        ///     An optional list of accessories that is/will be sent in along with the asset.
        /// </summary>
        /// <example> Charger </example>
        public IEnumerable<string>? Accessories { get; set; }


        /// <summary>
        ///     System-reserved constructor. Should not be used!
        /// </summary>
        [Obsolete("Reserved constructor for the JSON serializers.")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public AssetInfoDTO()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="AssetInfoDTO"/>. 
        ///     This simple version is reserved for use with external response-messages, such as information about replacement devices.
        /// </summary>
        /// <param name="brand"> The asset's brand. </param>
        /// <param name="model"> The asset's model. </param>
        /// <param name="imei"> If available, the asset's IMEI number. The IMEI number is always required for phones. 
        ///     For other asset-types this is required if <c><see cref="SerialNumber"/></c> is not provided. </param>
        /// <param name="serialNumber"> The asset's serial-number. This is required if <c><see cref="Imei"/></c> is not provided. </param>
        public AssetInfoDTO(string? brand, string? model, string? imei, string? serialNumber)
        {
            Brand = brand;
            Model = model;
            Imei = imei;
            SerialNumber = serialNumber;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AssetInfoDTO"/>. 
        ///     This simple version is reserved for use with external response-messages, such as information about replacement devices.
        /// </summary>
        /// <param name="brand"> The asset's brand. </param>
        /// <param name="model"> The asset's model. </param>
        /// <param name="imei"> If available, the asset's IMEI number. The IMEI number is always required for phones. 
        ///     For other asset-types this is required if <c><see cref="SerialNumber"/></c> is not provided. </param>
        /// <param name="serialNumber"> The asset's serial-number. This is required if <c><see cref="Imei"/></c> is not provided. </param>
        /// <param name="accessories"> An optional list of accessories that is/will be sent in along with the asset. </param>
        public AssetInfoDTO(string? brand, string? model, string? imei, string? serialNumber, IEnumerable<string>? accessories)
        {
            Brand = brand;
            Model = model;
            Imei = imei;
            SerialNumber = serialNumber;
            Accessories = accessories;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="AssetInfoDTO"/> class.
        /// </summary>
        /// <param name="brand"> The asset's brand. </param>
        /// <param name="model"> The asset's model. </param>
        /// <param name="assetCategoryId"> The asset's category ID. </param>
        /// <param name="imei"> If available, the asset's IMEI number. The IMEI number is always required for phones. 
        ///     For other asset-types this is required if <c><see cref="SerialNumber"/></c> is not provided. </param>
        /// <param name="serialNumber"> The asset's serial-number. This is required if <c><see cref="Imei"/></c> is not provided. </param>
        /// <param name="purchaseDate"> The original purchase-date. </param>
        /// <param name="accessories"> An optional list of accessories that is/will be sent in along with the asset. </param>
        /// <exception cref="ArgumentException"> Thrown if both <paramref name="imei"/> and <paramref name="serialNumber"/> is missing. </exception>
        public AssetInfoDTO(string brand, string model, int? assetCategoryId, string? imei, string? serialNumber, DateOnly purchaseDate, IEnumerable<string>? accessories = null)
        {
            if (string.IsNullOrEmpty(imei) && string.IsNullOrEmpty(serialNumber))
                throw new ArgumentException("Missing both IMEI and serial-number. Please provide at least one of the values.");

            Brand = brand;
            Model = model;
            AssetCategoryId = assetCategoryId;
            Imei = imei;
            SerialNumber = serialNumber;
            PurchaseDate = purchaseDate;
            Accessories = accessories;
        }
    }
}
