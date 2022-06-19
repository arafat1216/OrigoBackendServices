using Common.Converters;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Models
{
    public class AssetInfo
    {
        [Obsolete("Reserved constructor intended for use by Entity Framework or unit-testing. This should not be called directly in production-code.")]
        public AssetInfo()
        {
        }

        public AssetInfo(string? brand, string? model, ISet<string>? imei, string? serialNumber, DateOnly? purchaseDate, IEnumerable<string>? accessories)
        {
            Brand = brand;
            Model = model;
            Imei = imei?.ToHashSet();
            SerialNumber = serialNumber;
            PurchaseDate = purchaseDate;
            Accessories = accessories?.ToList();
        }


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
        public string? Model { get; set; }

        /// <summary>
        ///     The asset's IMEI number(s), if available. <para>
        ///     
        ///     IMEI is always required for phones. For other asset-types this is required if <c><see cref="SerialNumber"/></c> is not provided. </para>
        /// </summary>
        /// <example> 498973602928506 </example>
        public HashSet<string>? Imei { get; set; }


        [NotMapped]
        [Obsolete("This is a temp. auto-property so we can continue to use single imeis for the existing logic, but store them correctly in the DB. This will soon be removed once we can add in multi IMEI support throught the rest of the implementation.")]
        public string? SingleImei
        {
            get { return Imei?.FirstOrDefault(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Imei is null)
                        Imei = new HashSet<string>();

                    Imei.Add(value);
                }
            }
        }

        /// <summary>
        ///     The asset's serial-number. <para>
        ///     
        ///     Required if no <c><see cref="Imei"/></c> numbers are provided. </para>
        /// </summary>
        public string? SerialNumber { get; set; }

        /// <summary>
        ///     The original purchase-date.
        /// </summary>
        /// <example> 2021-05-30 </example>
        [JsonConverter(typeof(DateOnlyNullableJsonConverter))]
        public DateOnly? PurchaseDate { get; set; }

        /// <summary>
        ///     An optional list of accessories that is/will be sent in along with the asset.
        /// </summary>
        public List<string>? Accessories { get; set; }
    }
}
