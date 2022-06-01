using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.HardwareServiceOrder
{
    public class AssetInfo
    {
        public Guid AssetLifecycleId { get; set; }
        public string? Brand { get; set; }

        public string? Model { get; set; }

        public int? AssetCategoryId { get; set; }

        public string? Imei { get; set; }

        public string? SerialNumber { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public IEnumerable<string>? Accessories { get; set; }
    }
}
