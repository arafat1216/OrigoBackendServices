using Common.Converters;
using System.Text.Json.Serialization;

namespace HardwareServiceOrder.API.ViewModels
{
    public class AssetInfo
    {
        public Guid AssetLifecycleId { get; set; }

        public string? Brand { get; set; }

        // TODO: This should be removed. Use Brand + Model instead.
        public string? AssetName { get; set; }

        [Required]
        public string? Model { get; set; }

        // TODO: This should be moved out to the ServiceOrder to better reflect the EF entities.
        [Required]
        public int? AssetCategoryId { get; set; }

        [RegularExpression("^[0-9]{14,15}$")]
        [MinLength(14, ErrorMessage = "IMEI is too short.")]
        [MaxLength(15, ErrorMessage = "IMEI is too long.")]
        public string? Imei { get; set; }

        public string? SerialNumber { get; set; }

        [JsonConverter(typeof(DateOnlyNullableJsonConverter))]
        public DateOnly? PurchaseDate { get; set; }

        public IEnumerable<string>? Accessories { get; set; }
    }
}
