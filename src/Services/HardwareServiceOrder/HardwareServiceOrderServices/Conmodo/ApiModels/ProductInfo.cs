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
        ///     Available values can be found in /basedata
        /// </summary>
        /// <example> Cellphone </example>
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        /// <summary>
        ///     The brand of the device being sent in or out. They are described in /basedata
        /// </summary>
        /// <example> Samsung </example>
        [Required]
        [JsonPropertyName("brand")]
        public string Brand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <example> Galaxy S7 </example>
        [Required]
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        ///     IMEI number if the device has an IMEI. Required if serialNumber is not set
        /// </summary>
        [JsonPropertyName("imei")]
        [MinLength(14)]
        [MaxLength(15)]
        public string? Imei { get; set; }

        /// <summary>
        ///     Required if imeiNumber is not set
        /// </summary>
        [JsonPropertyName("serial")]
        public string? Serial { get; set; }

        /// <summary>
        ///     A list of accessories accompanying the repair object. Values can be found in /basedata
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
