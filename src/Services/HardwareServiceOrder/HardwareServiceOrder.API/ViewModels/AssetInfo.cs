namespace HardwareServiceOrder.API.ViewModels
{
    public class AssetInfo
    {
        public Guid AssetLifecycleId { get; set; }
        public string? Brand { get; set; }

        public string?  AssetName { get; set; }
        public string? Model { get; set; }

        public int? AssetCategoryId { get; set; }

        public string? Imei { get; set; }

        public string? SerialNumber { get; set; }

        public DateOnly? PurchaseDate { get; set; }

        public IEnumerable<string>? Accessories { get; set; }
    }
}
