using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     Legal values can be found in /basedata
    /// </summary>
    internal class ProductInfo
    {
        /// <summary>
        ///     The type of device.
        /// </summary>
        /// <remarks>
        ///     Available values can be found in '/basedata' (API endpoint)
        /// </remarks>
        /// <example> Mobiltelefon </example>
        /// <example> Tablet </example>
        /// <example> Bærbar </example>
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        /// <summary>
        ///     The brand of the device.
        /// </summary>
        /// <remarks>
        ///     Supported values are described in '/basedata' (API endpoint)
        /// </remarks>
        /// <example> Samsung </example>
        [Required]
        [JsonPropertyName("brand")]
        public string Brand { get; set; }

        /// <summary>
        ///     The model of the device.
        /// </summary>
        /// <example> Galaxy S7 </example>
        [Required]
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        ///     IMEI number if the device has an IMEI. Required if the <see cref="Serial"/>-number is not set.
        /// </summary>
        /// <example> "356782119717695" </example>
        [JsonPropertyName("imei")]
        [MinLength(14)]
        [MaxLength(15)]
        public string? Imei { get; set; }

        /// <summary>
        ///     Required if the <see cref="Imei"/> is not set.
        /// </summary>
        [JsonPropertyName("serial")]
        public string? Serial { get; set; }

        /// <summary>
        ///     A list of accessories accompanying the repair object. Values can be found in '/basedata' (API call)
        /// </summary>
        [JsonPropertyName("accessories")]
        public IEnumerable<string>? Accessories { get; set; }


        /// <summary>
        ///     System-reserved constructor. Should not be used!
        /// </summary>
        [Obsolete("Reserved constructor for unit-testing and the JSON serializers.")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ProductInfo()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public ProductInfo(string? category, string brand, string model, string? imei, string? serial, IEnumerable<string>? accessories)
        {
            Category = category;
            Brand = brand;
            Model = model;
            Imei = imei;
            Serial = serial;
            Accessories = accessories;
        }
    }
}
