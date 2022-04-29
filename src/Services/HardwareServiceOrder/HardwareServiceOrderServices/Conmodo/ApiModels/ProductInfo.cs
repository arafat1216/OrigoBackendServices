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
        public List<string>? Accessories { get; set; }
    }
}
